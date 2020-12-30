using FluentAssertions;
using Moq;
using NUnit.Framework;
using SolarWinds.SharedCommunication.Contracts.DataCache;
using SolarWinds.SharedCommunication.Contracts.Utils;
using SolarWinds.SharedCommunication.DataCache;
using SolarWinds.SharedCommunication.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using System.IO.MemoryMappedFiles;

namespace SolarWinds.SharedCommunication.Tests
{
    public class SharedMemorySegmentTest
    {
        private string segmentName;
        private Mock<IKernelObjectsPrivilegesChecker> privilegesChecker;
        private SharedMemorySegment sharedMemorySegment;

        [SetUp]
        public void SetUp()
        {
            segmentName = "testSegment";
            privilegesChecker = new Mock<IKernelObjectsPrivilegesChecker>();
            sharedMemorySegment = new SharedMemorySegment(segmentName, privilegesChecker.Object);
        }

        [Test]
        public void ReadBytes_ReturnBytes()
        {
            var result = sharedMemorySegment.ReadBytes();
            result.Should().NotBeNull();
        }
    }
}
