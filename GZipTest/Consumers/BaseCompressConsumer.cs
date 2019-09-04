using GZipTest.Producers;
using System.Collections.Generic;

namespace GZipTest.Consumers
{
    /// <summary>
    /// ������� ����� ��� Consumer (������������) ������ � ������������ ���������� ������������ ������ � ���������.
    /// </summary>
    internal abstract class BaseCompressConsumer<T> : BaseConsumer<T>
    {
        protected BaseCompressConsumer(IEnumerable<T> monitoredTaskQueue, string name = null) : base(monitoredTaskQueue, name)
        {
        }

        /// <summary>
        /// ����������� ���������, � ������� ����� ��������� ����� �������� ������.
        /// </summary>
        /// <param name="storage"></param>
        public void RegisterOutputStorage(IStorage<T> storage)
        {
            Storage = storage;
        }

        protected IStorage<T> Storage;
    }
}