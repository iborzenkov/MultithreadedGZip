using GZipTest.Producers;
using System.Collections.Generic;

namespace GZipTest.Consumers
{
    /// <summary>
    /// Базовый класс для Consumer (потребителей) данных с возможностью публикации обработанных данных в хранилище.
    /// </summary>
    internal abstract class BaseCompressConsumer<T> : BaseConsumer<T>
    {
        protected BaseCompressConsumer(IEnumerable<T> monitoredTaskQueue, string name = null) : base(monitoredTaskQueue, name)
        {
        }

        /// <summary>
        /// Регистрация хранилища, в которое после обработки будут помещены данные.
        /// </summary>
        /// <param name="storage"></param>
        public void RegisterOutputStorage(IStorage<T> storage)
        {
            Storage = storage;
        }

        protected IStorage<T> Storage;
    }
}