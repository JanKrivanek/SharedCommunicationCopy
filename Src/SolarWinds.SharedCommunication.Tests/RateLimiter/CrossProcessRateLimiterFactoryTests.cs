using FluentAssertions;
using Moq;
using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.RateLimiter;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.RateLimiter;
using System;

namespace SolarWinds.SharedCommunication.Tests.RateLimiter
{
    public class CrossProcessRateLimiterFactoryTests
    {
        private ICrossProcessRateLimiterFactory crossProcessRateLimiterFactory;
        private Mock<IDateTime> dateTime;
        private Mock<IKernelObjectsPrivilegesChecker> privilegesChecker;

        [SetUp]
        public void SetUp()
        {
            dateTime = new Mock<IDateTime>();
            privilegesChecker = new Mock<IKernelObjectsPrivilegesChecker>();
            crossProcessRateLimiterFactory = new CrossProcessRateLimiterFactory(dateTime.Object, privilegesChecker.Object);
        }

        [Test]
        public void OpenOrCreate_WithValidArguments_ReturnsRingMemoryBufferRateLimiter()
        {
            //Arrange
            const string identifier = "Test identifier";
            var measureTime = new TimeSpan(0, 2, 0);
            const int maxOccurencesPerTime = 1;
            const string kernelObjectsPrefixTest = "Global\\";
            privilegesChecker.Setup(k => k.KernelObjectsPrefix).Returns(kernelObjectsPrefixTest);
            //Act
            var result = crossProcessRateLimiterFactory.OpenOrCreate(identifier, measureTime, maxOccurencesPerTime);
            //Assert
            privilegesChecker.Verify(k => k.KernelObjectsPrefix, Times.Once);
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IRateLimiter>();
            result.Should().BeOfType<RingMemoryBufferRateLimiter>();
        }
    }
}
