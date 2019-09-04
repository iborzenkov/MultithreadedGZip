using System.Collections.Generic;

namespace GZipTest.Consumers
{
    /// <summary>
    /// Провайдер множества Consumer'ов (потребителей).
    /// </summary>
    internal class ConsumerProvider<T>
    {
        /// <summary>
        /// Добавить Consumer (потребителя) в коллекцию.
        /// </summary>
        /// <param name="consumer"></param>
        public void Add(BaseConsumer<T> consumer)
        {
            _consumers.Add(consumer);
        }

        /// <summary>
        /// Останавливает ожидание вызывающего клиента до окончания всех потоков обработки данных.
        /// </summary>
        public void WaitTillTaskCompleted()
        {
            foreach (var consumer in _consumers)
                consumer.Wait();
        }

        private readonly List<BaseConsumer<T>> _consumers = new List<BaseConsumer<T>>();
    }
}