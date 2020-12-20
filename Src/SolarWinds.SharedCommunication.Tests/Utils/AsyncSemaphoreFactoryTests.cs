using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.Utils;

namespace SolarWinds.SharedCommunication.Tests.Utils
{
    public class AsyncSemaphoreFactoryTests
    {
        private IAsyncSemaphoreFactory _asyncSemaphoreFactory;
        private Mock<ILogger> _logger;
        private const string TestName = "Test Name";

        [SetUp]
        public void AsyncSemaphoreFactoryTestsTests_SetUp()
        {
            _logger = new Mock<ILogger>();
            _asyncSemaphoreFactory = new AsyncSemaphoreFactory(_logger.Object);
        }

        [Test]
        public void CreateAsyncSemaphore_Succeeds()
        {
            //Act
            var result = _asyncSemaphoreFactory.Create(TestName);
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.AssignableTo<IAsyncSemaphore>());
            Assert.That(result, Is.InstanceOf<AsyncSemaphore>());
        }

        [Test]
        public void CreateAsyncSemaphore_With_KernelObjectsPrivilegesChecker_Succeeds()
        {
            //Arrange
            var kernelObjectsPrefixTest = "Global\\";
            var kernelObjectsPrivilegesChecker = new Mock<IKernelObjectsPrivilegesChecker>();
            kernelObjectsPrivilegesChecker.Setup(k => k.KernelObjectsPrefix).Returns(kernelObjectsPrefixTest);
            //Act
            var result = _asyncSemaphoreFactory.Create(TestName, kernelObjectsPrivilegesChecker.Object);
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.AssignableTo<IAsyncSemaphore>());
            Assert.That(result, Is.InstanceOf<AsyncSemaphore>());
        }

    }
}
