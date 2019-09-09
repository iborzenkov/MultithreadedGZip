using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GZipTest
{
    /// <summary>
    /// Очередь заданий на обработку на основе BlockingCollection.
    /// </summary>
    /// <remarks>
    /// Является наблюдателем за своими поставщиками данных (consummer'ами).
    /// </remarks>
    internal class TaskQueue<T> : ITaskQueue<T>
    {
        public TaskQueue(int capacity)
        {
            BlockingCollection = new BlockingCollection<T>(capacity);
        }

        public void Add(T data)
        {
            if (!BlockingCollection.TryAdd(data, _timeout))
            {
                throw new TimeoutException("Отменено по таймауту");
            }
        }

        public void Completed()
        {
            BlockingCollection.CompleteAdding();
        }

        public IEnumerable<T> Tasks => BlockingCollection.GetConsumingEnumerable();

        public void Dispose()
        {
            BlockingCollection.Dispose();
        }

        private BlockingCollection<T> BlockingCollection { get; }

        /// <summary>
        /// Таймаут обработки
        /// </summary>
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
    }
}