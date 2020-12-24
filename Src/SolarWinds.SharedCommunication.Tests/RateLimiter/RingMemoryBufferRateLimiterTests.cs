using FluentAssertions;
using Moq;
using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.RateLimiter;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.RateLimiter;
using System;
using System.Threading.Tasks;

namespace SolarWinds.SharedCommunication.Tests.RateLimiter
{
    public class RingMemoryBufferRateLimiterTests
    {
        private Mock<IRateLimiterDataAccessor> rateLimiterDataAccessor;
        private Mock<IDateTime> dateTime;
        private IRateLimiter ringMemoryBufferRateLimiter;

        [SetUp]
        public void SetUp()
        {
            int spanTicks = 30;
            int capacity = 1;
            dateTime = new Mock<IDateTime>();
            rateLimiterDataAccessor = new Mock<IRateLimiterDataAccessor>();
            dateTime.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);
            rateLimiterDataAccessor.Setup(r => r.Capacity).Returns(capacity);
            rateLimiterDataAccessor.Setup(r => r.SpanTicks).Returns(spanTicks);
            ringMemoryBufferRateLimiter = new RingMemoryBufferRateLimiter(rateLimiterDataAccessor.Object, dateTime.Object);
        }

        [Test]
        public async Task WaitTillNextFreeSlotAsync_WithValidArguments_ReturnsTrue()
        {
            //Arrange
            var maxAcceptableWaitingTime = new TimeSpan(0, 0, 5);
            rateLimiterDataAccessor.Setup(r => r.TryEnterSynchronizedRegion()).Returns(true);
            //Act
            var result = await ringMemoryBufferRateLimiter.WaitTillNextFreeSlotAsync(maxAcceptableWaitingTime);
            //Assert
            rateLimiterDataAccessor.Verify(r => r.TryEnterSynchronizedRegion(), Times.AtLeastOnce);
            rateLimiterDataAccessor.Verify(r => r.ExitSynchronizedRegion(), Times.Once);
            result.Should().BeTrue();
        }

        [Test]
        public async Task WaitTillNextFreeSlotAsync_WithValidArguments_ReturnsFalse()
        {
            //Arrange
            var timeShift = new TimeSpan(2, 0, 0, 0);
            var oldestTimestamp = DateTime.UtcNow - timeShift;
            const int capacity = 1;
            const int size = 1;
            rateLimiterDataAccessor.Setup(r => r.Capacity).Returns(capacity);
            rateLimiterDataAccessor.Setup(r => r.Size).Returns(size);
            rateLimiterDataAccessor.Setup(r => r.SpanTicks).Returns(DateTime.UtcNow.Ticks);
            rateLimiterDataAccessor.Setup(r => r.OldestTimestampTicks).Returns(oldestTimestamp.Ticks);
            ringMemoryBufferRateLimiter = new RingMemoryBufferRateLimiter(rateLimiterDataAccessor.Object, dateTime.Object);
            var maxAcceptableWaitingTime = new TimeSpan(0, 0, 1);
            rateLimiterDataAccessor.Setup(r => r.TryEnterSynchronizedRegion()).Returns(false);
            //Act
            var result = await ringMemoryBufferRateLimiter.WaitTillNextFreeSlotAsync(maxAcceptableWaitingTime);
            //Assert
            rateLimiterDataAccessor.Verify(r => r.Size, Times.Once);
            rateLimiterDataAccessor.Verify(r => r.Capacity, Times.Exactly(2));
            rateLimiterDataAccessor.Verify(r => r.SpanTicks, Times.Exactly(2));
            rateLimiterDataAccessor.Verify(r => r.OldestTimestampTicks, Times.Once);
            rateLimiterDataAccessor.Verify(r => r.TryEnterSynchronizedRegion(), Times.AtLeastOnce);
            rateLimiterDataAccessor.Verify(r => r.ExitSynchronizedRegion(), Times.Once);
            result.Should().BeFalse();
        }

        [Test]
        public void BlockTillNextFreeSlot_WithValidArguments_ReturnsTrue()
        {
            //Arrange
            var maxAcceptableWaitingTime = new TimeSpan(0, 0, 5);
            rateLimiterDataAccessor.Setup(r => r.TryEnterSynchronizedRegion()).Returns(true);
            //Act
            var result = ringMemoryBufferRateLimiter.BlockTillNextFreeSlot(maxAcceptableWaitingTime);
            //Assert
            rateLimiterDataAccessor.Verify(r => r.TryEnterSynchronizedRegion(), Times.AtLeastOnce);
            rateLimiterDataAccessor.Verify(r => r.ExitSynchronizedRegion(), Times.Once);
            result.Should().BeTrue();
        }

        [Test]
        public void BlockTillNextFreeSlot_WithValidArguments_ReturnsFalse()
        {
            //Arrange
            var timeShift = new TimeSpan(2, 0, 0, 0);
            var oldestTimestamp = DateTime.UtcNow - timeShift;
            const int capacity = 1;
            const int size = 1;
            rateLimiterDataAccessor.Setup(r => r.Capacity).Returns(capacity);
            rateLimiterDataAccessor.Setup(r => r.Size).Returns(size);
            rateLimiterDataAccessor.Setup(r => r.SpanTicks).Returns(DateTime.UtcNow.Ticks);
            rateLimiterDataAccessor.Setup(r => r.OldestTimestampTicks).Returns(oldestTimestamp.Ticks);
            ringMemoryBufferRateLimiter = new RingMemoryBufferRateLimiter(rateLimiterDataAccessor.Object, dateTime.Object);
            var maxAcceptableWaitingTime = new TimeSpan(0, 0, 1);
            rateLimiterDataAccessor.Setup(r => r.TryEnterSynchronizedRegion()).Returns(false);
            //Act
            var result = ringMemoryBufferRateLimiter.BlockTillNextFreeSlot(maxAcceptableWaitingTime);
            //Assert
            rateLimiterDataAccessor.Verify(r => r.Size, Times.Once);
            rateLimiterDataAccessor.Verify(r => r.Capacity, Times.Exactly(2));
            rateLimiterDataAccessor.Verify(r => r.SpanTicks, Times.Exactly(2));
            rateLimiterDataAccessor.Verify(r => r.OldestTimestampTicks, Times.Once);
            rateLimiterDataAccessor.Verify(r => r.TryEnterSynchronizedRegion(), Times.AtLeastOnce);
            rateLimiterDataAccessor.Verify(r => r.ExitSynchronizedRegion(), Times.Once);
            result.Should().BeFalse();
        }
    }
}
