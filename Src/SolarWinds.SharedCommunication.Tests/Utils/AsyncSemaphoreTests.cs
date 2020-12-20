using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolarWinds.SharedCommunication.Tests.Utils
{
    public class AsyncSemaphoreTests
    {
        private IAsyncSemaphore _asyncSemaphore;

        [SetUp]
        public void AsyncSemaphoreTests_SetUp()
        {
            _asyncSemaphore = new AsyncSemaphore(new Semaphore(1,1));
        }

        [Test]
        public void WaitAsync_Succeeds()
        {
            //Act
            var result = _asyncSemaphore.WaitAsync();
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Task>());
        }

        [Test]
        public void LockAsync_Succeeds()
        {
            //Act
            var result = _asyncSemaphore.LockAsync();
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Task<IDisposable>>());
        }
    }
}
