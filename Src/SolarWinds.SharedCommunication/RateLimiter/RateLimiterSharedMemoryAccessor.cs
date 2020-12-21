using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security.AccessControl;
using System.Threading;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.RateLimiter
{
    internal class RateLimiterSharedMemoryAccessor : IRateLimiterDataAccessor
    {
        private const long interlockLatchOffset = 0;

        //padding for rest of long
        private const long capacityOffset = interlockLatchOffset + sizeof(long);
        private const long spanOffset = capacityOffset + sizeof(long);
        private const long sizeOffset = spanOffset + sizeof(long);
        private const long currentIdxOffset = sizeOffset + sizeof(long);
        private const long contentAddressOffset = currentIdxOffset + sizeof(long);
        private const int lockTaken = 1;
        private const int lockFree = 0;

        //need to GC root this as view accessor doesn't take full ownership
        private readonly MemoryMappedFile mmf;
        private readonly MemoryMappedViewAccessor memoryAccessor;
        private readonly MemoryMappedViewAccessor ringBuffermemoryAccessor;

        //we need the raw pointer for CAS operations
        private readonly IntPtr latchAddress;

        public RateLimiterSharedMemoryAccessor(
            string segmentName, 
            int capacity, 
            long spanTicks, 
            IKernelObjectsPrivilegesChecker privilegesChecker)
        {
            //TODO: should acquire mutext (or better just the write latch)

            segmentName = privilegesChecker.KernelObjectsPrefix + segmentName;

            //this would be preventing code 
            var security = new MemoryMappedFileSecurity();
            security.AddAccessRule(new AccessRule<MemoryMappedFileRights>(
                "everyone",
                MemoryMappedFileRights.ReadWrite,
                AccessControlType.Allow));
            mmf = MemoryMappedFile.CreateOrOpen(segmentName, contentAddressOffset + capacity,
                MemoryMappedFileAccess.ReadWrite,
                MemoryMappedFileOptions.None, security, HandleInheritability.None);

            memoryAccessor = mmf.CreateViewAccessor(interlockLatchOffset, contentAddressOffset);

            Capacity = (int)memoryAccessor.ReadInt64(capacityOffset);
            SpanTicks = memoryAccessor.ReadInt64(spanOffset);

            if (Capacity == 0 && SpanTicks == 0)
            {
                memoryAccessor.Write(capacityOffset, (long)capacity);
                memoryAccessor.Write(spanOffset, spanTicks);
                Capacity = capacity;
                SpanTicks = spanTicks;
            }
            else if (Capacity != capacity || SpanTicks != spanTicks)
            {
                throw new Exception(
                    $"Mismatch during RateLimiterSharedMemoryAccessor creation. [{segmentName}] had capacity set to {Capacity} and SpanTicks to {SpanTicks}, while caller requested {capacity}, {spanTicks}");
            }

            ringBuffermemoryAccessor = mmf.CreateViewAccessor(contentAddressOffset, capacity * sizeof(long));

            //this points to start of MMF, not the view! (in our same luckily the same as offset is )
            latchAddress = memoryAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
        }

        public unsafe bool TryEnterSynchronizedRegion()
        {
            return Interlocked.CompareExchange(ref (*((int*)(latchAddress))), lockTaken, lockFree) == lockFree;
        }

        public void ExitSynchronizedRegion()
        {
            memoryAccessor.Write(interlockLatchOffset, (int)0);
            //.net guarantees writes not to be reordered - but just for sure lets explicitly issue mem barrier
            Thread.MemoryBarrier();
        }

        public int Capacity { get; }
        public int Size
        {
            get => memoryAccessor.ReadInt32(sizeOffset);
            set => memoryAccessor.Write(sizeOffset, value >= Capacity ? Capacity : value);
        }

        private int GetWrapIndex(int i)
        {
            if (i < 0)
                return Capacity - 1;
            if (i >= Capacity)
                return 0;
            return i;
        }

        public long SpanTicks { get; }

        public long OldestTimestampTicks =>
            ringBuffermemoryAccessor.ReadInt64(CurrentIndex * sizeof(long));

        private int CurrentIndex
        {
            get => memoryAccessor.ReadInt32(currentIdxOffset);
            set => memoryAccessor.Write(currentIdxOffset, GetWrapIndex(value));
        }

        public long CurrentTimestampTicks
        {
            get => ringBuffermemoryAccessor.ReadInt64(GetWrapIndex(CurrentIndex - 1) * sizeof(long));

            set
            {
                ringBuffermemoryAccessor.Write(CurrentIndex * sizeof(long), value);
                //properties handle wrapping appropriately
                Size++;
                CurrentIndex++;
            }
        }
    }
}