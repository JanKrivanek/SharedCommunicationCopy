using System;
using System.Threading;
using System.Threading.Tasks;
using SolarWinds.SharedCommunication.Contracts.RateLimiter;
using SolarWinds.SharedCommunication.Contracts.Utils;

namespace SolarWinds.SharedCommunication.RateLimiter
{
    public class RingMemoryBufferRateLimiter : IRateLimiter
    {
        private static readonly Task<bool> success = Task.FromResult(true);
        private static readonly Task<bool> failure = Task.FromResult(false);
        private readonly IRateLimiterDataAccessor rateLimiterDataAccessor;
        private readonly IDateTime dateTime;
        private readonly int rateLimiterCapacity;
        private readonly TimeSpan rateLimiterSpan;

        public RingMemoryBufferRateLimiter(
            IRateLimiterDataAccessor rateLimiterDataAccessor,
            IDateTime dateTime)
        {
            this.rateLimiterDataAccessor = rateLimiterDataAccessor;
            this.dateTime = dateTime;

            rateLimiterCapacity = this.rateLimiterDataAccessor.Capacity;
            rateLimiterSpan = new TimeSpan(this.rateLimiterDataAccessor.SpanTicks);
        }

        private void EnterSynchronization()
        {
            SpinWait.SpinUntil(rateLimiterDataAccessor.TryEnterSynchronizedRegion);
        }

        //Depending on version of OS and .NET framework, the granularity of timer and timer events can 1-15ms (15ms being the usual)
        // this could lead to 'false wake-up' issues during contention (leading to resonated contention)
        private TimeSpan GetRandomSaltSpan()
        {
            return TimeSpan.FromMilliseconds(new Random().Next(20));
        }

        private bool ClaimSlotAndGetWaitingTime(TimeSpan maxAcceptableWaitingTime, out TimeSpan waitSpan)
        {
            DateTime utcNow;
            waitSpan = TimeSpan.Zero;
            bool isAcceptable = true;
            try
            {
                EnterSynchronization();
                utcNow = dateTime.UtcNow;
                bool isFull = rateLimiterDataAccessor.Size == rateLimiterCapacity;

                if (isFull)
                {
                    DateTime oldestEventUtc =
                        new DateTime(rateLimiterDataAccessor.OldestTimestampTicks);
                    waitSpan = rateLimiterSpan - (utcNow - oldestEventUtc);
                    //prevent false wake ups by randomness
                    waitSpan = waitSpan <= TimeSpan.Zero ? TimeSpan.Zero : (waitSpan + GetRandomSaltSpan());
                }

                isAcceptable = waitSpan <= maxAcceptableWaitingTime;
                if (isAcceptable)
                {
                    rateLimiterDataAccessor.CurrentTimestampTicks = (utcNow + waitSpan).Ticks;
                }
            }
            finally
            {
                rateLimiterDataAccessor.ExitSynchronizedRegion();
            }

            return isAcceptable;
        }

        public Task<bool> WaitTillNextFreeSlotAsync(TimeSpan maxAcceptableWaitingTime, CancellationToken cancellationToken = default)
        {
            TimeSpan waitSpan;
            if (!ClaimSlotAndGetWaitingTime(maxAcceptableWaitingTime, out waitSpan))
            {
                return failure;
            }

            if (waitSpan <= TimeSpan.Zero)
                return success;
            else
                return Task.Delay(waitSpan, cancellationToken).ContinueWith(t => !t.IsCanceled);
        }

        public bool BlockTillNextFreeSlot(TimeSpan maxAcceptableWaitingTime)
        {
            TimeSpan waitSpan;
            if (!ClaimSlotAndGetWaitingTime(maxAcceptableWaitingTime, out waitSpan))
            {
                return false;
            }

            if (waitSpan > TimeSpan.Zero)
                Thread.Sleep(waitSpan);

            return true;
        }
    }
}