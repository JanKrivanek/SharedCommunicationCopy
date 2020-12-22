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
        public void PlatformDateTimeTests_SetUp() =>
            synchronizationIdentifiersProvider = new SynchronizationIdentifiersProvider();

        [TestCase("", "apiKey", "orgId")]
        [TestCase("BaseAddress", "", "orgId")]
        public void GetSynchronizationIdentifier_Fails(string apiBaseAddress, string apiKey, string orgId)
        {
            //Act
            Action action = () =>
                synchronizationIdentifiersProvider.GetSynchronizationIdentifier(apiBaseAddress, apiKey, orgId);
            //Assert
            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void GetSynchronizationIdentifier_Succeeds()
        {
            //Arrange
            var apiBaseAddress = "net.tcp://localhost:17777/SolarWinds/PollerCache";
            var apiKey = "123";
            var orgId = "5";
            var expectedResult = "OH6GkFGoxY2Q+yyXlr2CdYxd3vEnFsmrJP87gO5wehs=";
            //Act
            var result =
                synchronizationIdentifiersProvider.GetSynchronizationIdentifier(apiBaseAddress, apiKey, orgId);
            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Be(expectedResult);
        }
    }
}
