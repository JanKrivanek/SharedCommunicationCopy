using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.DataCache
{
    /// <summary>
    /// a class that represents shared memory segment
    /// </summary>
    public class SharedMemorySegment : ISharedMemorySegment
    {
        private const long stampOffset = 0;
        private const long sizeOffset = stampOffset + sizeof(long);
        private const long capacityOffset = sizeOffset + sizeof(long);
        private const long contentAddressOffset = capacityOffset + sizeof(long);
        private static readonly long headersSize = contentAddressOffset + Marshal.SizeOf<Guid>(); //Guid doesn't have sizeof constant


        //need to GC root this as view accessor doesn't take full ownership
        private readonly MemoryMappedFile mmf;
        private readonly MemoryMappedViewAccessor memoryAccessor;
        //need to GC root this as stream accessor doesn't take full ownership
        private MemoryMappedFile contentMmf;
        private MemoryMappedViewStream contentMemoryStream;
        private Guid lastKnownContentAddress = Guid.NewGuid();

        private readonly string segmentName;
        private readonly string contnetSegmentNamePreffix;

        public SharedMemorySegment(string segmentName, IKernelObjectsPrivilegesChecker kernelObjectsPrivilegesChecker)
        {
            this.segmentName = kernelObjectsPrivilegesChecker.KernelObjectsPrefix + segmentName;
            contnetSegmentNamePreffix = this.segmentName + "_content_";

            var security = new MemoryMappedFileSecurity();
            security.AddAccessRule(new AccessRule<MemoryMappedFileRights>(
                "everyone",
                MemoryMappedFileRights.ReadWrite,
                AccessControlType.Allow));

            mmf = MemoryMappedFile.CreateOrOpen(this.segmentName, headersSize, MemoryMappedFileAccess.ReadWrite,
                MemoryMappedFileOptions.DelayAllocatePages, security, HandleInheritability.None);
            memoryAccessor = mmf.CreateViewAccessor(0, headersSize);
        }


        public DateTime LastChangedUtc => new DateTime(memoryAccessor.ReadInt64(stampOffset), DateTimeKind.Utc);

        public long ContentSize => memoryAccessor.ReadInt64(sizeOffset);
        public long Capacity => memoryAccessor.ReadInt64(capacityOffset);

        public Guid ContentAddress
        {
            get
            {
                int sz = Marshal.SizeOf<Guid>(); //16
                byte[] guidData = new byte[sz];
                memoryAccessor.ReadArray(contentAddressOffset, guidData, 0, sz);
                return new Guid(guidData);
            }
        }

        /// <summary>
        /// method for opening the memory mapped view stream with needed rights
        /// </summary>
        /// <returns></returns>
        private MemoryMappedViewStream EnsureContentStream()
        {
            if (lastKnownContentAddress == ContentAddress)
            {
                return contentMemoryStream;
            }

            //old MMF is obsolete - we need to release it
            contentMemoryStream?.Dispose();
            contentMmf?.Dispose();

            var security = new MemoryMappedFileSecurity();
            security.AddAccessRule(new AccessRule<MemoryMappedFileRights>(
                "everyone",
                MemoryMappedFileRights.ReadWrite,
                AccessControlType.Allow));

            Guid newAddressGuid = ContentAddress;
            long newSize = ContentSize;
            if (newSize > 0)
            {
                contentMmf = MemoryMappedFile.CreateOrOpen(contnetSegmentNamePreffix + newAddressGuid, newSize,
                    MemoryMappedFileAccess.ReadWrite,
                    MemoryMappedFileOptions.DelayAllocatePages, security, HandleInheritability.None);
                contentMemoryStream = contentMmf.CreateViewStream(0, newSize);
            }
            else
            {
                contentMmf = null;
                contentMemoryStream = null;
            }

            lastKnownContentAddress = newAddressGuid;

            return contentMemoryStream;
        }

        /// <summary>
        /// method for reading data from memory segment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReadData<T>()
        {
            var stream = EnsureContentStream();

            if (stream == null) return default;

            var ds = new DataContractSerializer(typeof(T));
            return (T)ds.ReadObject(stream);
        }

        /// <summary>
        /// method for writing data to memory segment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"> data to write </param>
        public void WriteData<T>(T data)
        {
            if (data == null)
            {
                Clear();
                return;
            }

            var ds = new DataContractSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ds.WriteObject(ms, data);
            byte[] bytes = ms.ToArray();
            this.WriteBytes(bytes);
        }

        /// <summary>
        /// method for reading bytes from a memory segment
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes()
        {
            var stream = EnsureContentStream();
            if (stream == null) return new byte[0];

            byte[] data = new byte[ContentSize];
            MemoryStream ms = new MemoryStream(data);
            stream.CopyTo(ms);
            return data;
        }

        /// <summary>
        /// method for writing bytes to memory segment
        /// </summary>
        /// <param name="bytes"> bytes to write </param>
        public void WriteBytes(byte[] bytes)
        {
            //if we have too small or too big segment
            if (bytes.Length > this.Capacity || GetPaddedSize(bytes.Length) < this.Capacity)
            {
                ReserveMemorySegment(GetPaddedSize(bytes.Length));
            }

            EnsureContentStream().Write(bytes, 0, bytes.Length);
            memoryAccessor.Write(sizeOffset, (long)bytes.Length);
            memoryAccessor.Write(stampOffset, (long)DateTime.UtcNow.Ticks);
        }

        /// <summary>
        /// method for clearing the memory segment
        /// </summary>
        public void Clear()
        {
            ReserveMemorySegment(0);
            memoryAccessor.Write(sizeOffset, (long)0);
            memoryAccessor.Write(stampOffset, (long)0);
        }

        private void ReserveMemorySegment(int capacity)
        {
            memoryAccessor.Write(capacityOffset, (long)capacity);
            memoryAccessor.WriteArray(contentAddressOffset, Guid.NewGuid().ToByteArray(), 0,
                Marshal.SizeOf<Guid>());

            EnsureContentStream();
        }

        private static int CeilingToPowerOfTwo(int value)
        {
            //source: https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value++;
            return value;
        }

        private static int GetPaddedSize(int size)
        {
            const int megabyte = 1024 * 1024; //1 MB
            const int doublingThreshold = 20 * megabyte; //20 MBs

            return size < doublingThreshold ? CeilingToPowerOfTwo(size) : (size + megabyte);
        }

        public void Dispose()
        {
            mmf?.Dispose();
            memoryAccessor?.Dispose();
            contentMmf?.Dispose();
            contentMemoryStream?.Dispose();
        }
    }
}
