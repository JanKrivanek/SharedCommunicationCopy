using FluentAssertions;
using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.Utils;
using System;

namespace SolarWinds.SharedCommunication.Tests.Utils
{
    public class SynchronizationIdentifiersProviderTest
    {
        private ISynchronizationIdentifiersProvider synchronizationIdentifiersProvider;

        [SetUp]
        public void SetUp() =>
            synchronizationIdentifiersProvider = new SynchronizationIdentifiersProvider();

        [TestCase("", "apiKey", "orgId")]
        [TestCase("BaseAddress", "", "orgId")]
        public void GetSynchronizationIdentifier_WithInvalidArguments_ThrowsArgumentException(string apiBaseAddress, string apiKey, string orgId)
        {
            //Act
            Action action = () =>
                synchronizationIdentifiersProvider.GetSynchronizationIdentifier(apiBaseAddress, apiKey, orgId);
            //Assert
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void GetSynchronizationIdentifier_WithValidArguments_ReturnsIdentifier()
        {
            //Arrange
            const string apiBaseAddress = "net.tcp://localhost:17777/SolarWinds/PollerCache";
            const string apiKey = "123";
            const string orgId = "5";
            //Act
            var result1 =
                synchronizationIdentifiersProvider.GetSynchronizationIdentifier(apiBaseAddress, apiKey, orgId);
            var result2 =
                synchronizationIdentifiersProvider.GetSynchronizationIdentifier(apiBaseAddress, apiKey, orgId);
            //Assert
            result1.Should().NotBeNullOrEmpty();
            result2.Should().NotBeNullOrEmpty();
            result1.Should().Be(result2);
        }

        [Test]
        public void GetSynchronizationIdentifier_WithValidArguments_ReturnsDifferentIdentifiers()
        {
            //Arrange
            const string apiBaseAddress = "net.tcp://localhost:17777/SolarWinds/PollerCache";
            const string apiKey1 = "123";
            const string apiKey2 = "124";
            const string orgId = "5";
            //Act
            var result1 =
                synchronizationIdentifiersProvider.GetSynchronizationIdentifier(apiBaseAddress, apiKey1, orgId);
            var result2 =
                synchronizationIdentifiersProvider.GetSynchronizationIdentifier(apiBaseAddress, apiKey2, orgId);
            //Assert
            result1.Should().NotBeNullOrEmpty();
            result2.Should().NotBeNullOrEmpty();
            result1.Should().NotBe(result2);
        }
    }
}
