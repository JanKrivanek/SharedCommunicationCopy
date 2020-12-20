using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.Utils;
using System;

namespace SolarWinds.SharedCommunication.Tests.Utils
{
    public class PlatformDateTimeTests
    {
        private IDateTime _platformDateTime;

        [SetUp]
        public void PlatformDateTimeTests_SetUp()
        {
            _platformDateTime = new PlatformDateTime();
        }

        [Test]
        public void UtcNowCorrect()
        {
            Assert.That(_platformDateTime, Is.Not.Null);
            Assert.That(_platformDateTime.UtcNow, Is.EqualTo(DateTime.UtcNow));
        }
    }
}
