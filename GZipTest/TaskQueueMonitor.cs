using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Очередь заданий на обработку с использование монитора для синхронизации потоков.
    /// </summary>
    /// <remarks>
    /// Является наблюдателем за своими поставщиками данных (consummer'ами).
    /// </remarks>
    internal class TaskQueueMonitor<T> : ITaskQueue<T>
    {
        public TaskQueueMonitor(int capacity)
        {
            _capacity = capacity;
            Queue = new Queue<T>();
        }

        public void Add(T data)
        {
            if (_isCompleted)
                throw new InvalidOperationException("Добавление в очередь ПОСЛЕ завершения");

            lock (_locker)
            {
                // Останавливаем добавление заданий в очередь, если превышен размер очереди.
                if (Queue.Count >= _capacity)
                {
                    if (!Monitor.Wait(_locker, _timeout))
                    {
                        throw new TimeoutException("Отменено по таймауту");
                    }
                }

                Queue.Enqueue(data);
                Monitor.PulseAll(_locker);
            }
        }

        public void Completed()
        {
            lock (_locker)
            {
                _isCompleted = true;
                Monitor.PulseAll(_locker);
            }
        }

        public IEnumerable<T> Tasks
        {
            get
            {
                lock (_locker)
                {
                    // До тех пор, пока в очереди есть задания и нет признака завершённости, возвращаем задания из очереди
                    while (Queue.Any() || !_isCompleted)
                    {
                        if (Queue.Any())
                        {
                            var task = Queue.Dequeue();
                            // Уведомляем монитор, что задание из очереди забрано.
                            // Если есть кто-то ждёт освобождения очереди, он может начать в неё теперь добавлять данные.
                            Monitor.PulseAll(_locker);

                            yield return task;
                        }
                        else
                        {
                            // Если пока что в очереди ничего нет, ждём
                            Monitor.Wait(_locker);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            // пока нечего уничтожать
        }

        private Queue<T> Queue { get; }
        private bool _isCompleted;
        private readonly int _capacity;
        private readonly object _locker = new object();

        /// <summary>
        /// Таймаут обработки
        /// </summary>
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
    }
}