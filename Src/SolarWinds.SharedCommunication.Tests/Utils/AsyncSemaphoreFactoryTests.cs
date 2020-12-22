using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.Utils;

namespace SolarWinds.SharedCommunication.Tests.Utils
{
    public class AsyncSemaphoreFactoryTests
    {
        private IAsyncSemaphoreFactory asyncSemaphoreFactory;
        private Mock<ILogger> logger;
        private const string TestName = "Test Name";

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger>();
            asyncSemaphoreFactory = new AsyncSemaphoreFactory(logger.Object);
        }

        [Test]
        public void Create_WithValidName_ReturnsAsyncSemaphore()
        {
            //Act
            var result = asyncSemaphoreFactory.Create(TestName);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IAsyncSemaphore>().And.BeOfType<AsyncSemaphore>();
        }

        [Test]
        public void Create_WithValidNameAndKernelObjectsPrivilegesChecker_ReturnsAsyncSemaphore()
        {
            //Arrange
            var kernelObjectsPrefixTest = "Global\\";
            var kernelObjectsPrivilegesChecker = new Mock<IKernelObjectsPrivilegesChecker>();
            kernelObjectsPrivilegesChecker.Setup(k => k.KernelObjectsPrefix).Returns(kernelObjectsPrefixTest);
            //Act
            var result = asyncSemaphoreFactory.Create(TestName, kernelObjectsPrivilegesChecker.Object);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IAsyncSemaphore>().And.BeOfType<AsyncSemaphore>();
        }

    }
}
