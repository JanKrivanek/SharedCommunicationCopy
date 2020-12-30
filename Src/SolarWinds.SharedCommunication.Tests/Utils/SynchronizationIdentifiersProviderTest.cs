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
            const string expectedResult = "OH6GkFGoxY2Q+yyXlr2CdYxd3vEnFsmrJP87gO5wehs=";
            //Act
            var result =
                synchronizationIdentifiersProvider.GetSynchronizationIdentifier(apiBaseAddress, apiKey, orgId);
            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Be(expectedResult);
        }
    }
}
