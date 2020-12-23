using System;

namespace SolarWinds.SharedCommunication.DataCache
{
    /// <summary>
    /// interface for shared memory segment
    /// </summary>
    public interface ISharedMemorySegment: IDisposable
    {
        DateTime LastChangedUtc { get; }

        //long ContentSize { get; }
        //long Capacity { get; }
        T ReadData<T>();
        void WriteData<T>(T data);
        byte[] ReadBytes();
        void WriteBytes(byte[] bytes);
        void Clear();
    }
}