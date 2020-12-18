using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.Utils;

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
            Assert.NotNull(_synchronizationIdentifiersProvider);
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
            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(expectedResult,result);
        }
    }
}
