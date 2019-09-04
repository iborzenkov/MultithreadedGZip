using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Очередь заданий на обработку с использование светофора и событий для синхронизации потоков.
    /// </summary>
    /// <remarks>
    /// Является наблюдателем за своими поставщиками данных (consummer'ами).
    /// </remarks>
    internal class TaskQueueSemaphore<T> : ITaskQueue<T>, IDisposable
    {
        public TaskQueueSemaphore(int capacity)
        {
            _capacity = capacity;
            _semaphore = new Semaphore(0, _semaphoreCapacity);

            Queue = new Queue<T>();
        }

        public void Add(T data)
        {
            if (_isCompleted)
                throw new InvalidOperationException("Добавление в очередь ПОСЛЕ завершения");

            // Не впускаем внуть критической секции другой поток, который будет добавлять данные
            lock (_lockerAdd)
            {
                Queue.Enqueue(data);
                _semaphore.Release();

                // Останавливаем добавление заданий в очередь, если превышен размер очереди.
                if (Queue.Count >= _capacity)
                {
                    _eventTaskIsTaken.Reset();
                }

                if (!_eventTaskIsTaken.WaitOne(_timeout))
                {
                    throw new TimeoutException("Отменено по таймауту");
                }
            }
        }

        public void Completed()
        {
            _isCompleted = true;
            _semaphore.Release(_semaphoreCapacity);
        }

        public IEnumerable<T> Tasks
        {
            get
            {
                while (true)
                {
                    if (!_semaphore.WaitOne(_timeout))
                    {
                        throw new TimeoutException("Отменено по таймауту");
                    }

                    lock (_lockerGet)
                    {
                        if (Queue.Any())
                        {
                            var task = Queue.Dequeue();
                            // Уведомляем, что задание из очереди забрано.
                            // Если есть кто-то ждёт освобождения очереди, он может начать в неё теперь добавлять данные.
                            _eventTaskIsTaken.Set();

                            yield return task;
                        }
                        else
                        {
                            if (_isCompleted)
                            {
                                yield break;
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            _eventTaskIsTaken.Dispose();
            _semaphore.Dispose();
        }

        private Queue<T> Queue { get; }
        private bool _isCompleted;
        private readonly int _capacity;
        private readonly object _lockerGet = new object();
        private readonly object _lockerAdd = new object();
        private readonly AutoResetEvent _eventTaskIsTaken = new AutoResetEvent(true);
        private readonly Semaphore _semaphore;
        private readonly int _semaphoreCapacity = Int32.MaxValue;

        /// <summary>
        /// Таймаут обработки
        /// </summary>
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
    }
}