using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.Utils;
using System;

namespace SolarWinds.SharedCommunication.Tests.Utils
{
    public class SynchronizationIdentifiersProviderTest
    {
        private ISynchronizationIdentifiersProvider _synchronizationIdentifiersProvider;

        [SetUp]
        public void PlatformDateTimeTests_SetUp()
        {
            _synchronizationIdentifiersProvider = new SynchronizationIdentifiersProvider();
        }

        [TestCase("", "apiKey", "orgId")]
        [TestCase("BaseAddress", "", "orgId")]
        public void GetSynchronizationIdentifier_Fails(string apiBaseAddress, string apiKey, string orgId)
        {
            //Assert
            Assert.That(_synchronizationIdentifiersProvider, Is.Not.Null);
            Assert.Throws<ArgumentException>(
                () => _synchronizationIdentifiersProvider.GetSynchronizationIdentifier(apiBaseAddress, apiKey, orgId));
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
                _synchronizationIdentifiersProvider.GetSynchronizationIdentifier(apiBaseAddress, apiKey, orgId);
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
