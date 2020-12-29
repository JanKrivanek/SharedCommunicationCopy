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
       // private Mock<MemoryMappedFile> mmf;
       // private Mock<MemoryMappedViewAccessor> memoryAccessor;
        //private Mock<MemoryMappedFile> contentMmf;
        //private Mock<MemoryMappedViewStream> contentMemoryStream;
        private string segmentName;
        private Mock<IKernelObjectsPrivilegesChecker> privilegesChecker;
        private SharedMemorySegment sharedMemorySegment;

        [SetUp]
        public void SetUp()
        {
            segmentName = "testSegment";
            privilegesChecker = new Mock<IKernelObjectsPrivilegesChecker>();
          //  memoryAccessor = new Mock<MemoryMappedViewAccessor>();

            sharedMemorySegment = new SharedMemorySegment(segmentName, privilegesChecker.Object);
        }

        [Test]
        public void ReadBytes_ReturnBytes()
        {
           // memoryAccessor.Setup(m => m.ReadInt64(sizeof(long))).Returns(10);
           // memoryAccessor.Setup(m => m.ReadArray(3 * sizeof(long), new byte[16], 0, 16)).Returns(25);

            var result = sharedMemorySegment.ReadBytes();

            result.Should().NotBeNull();
        }
    }
}
