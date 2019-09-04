using System.Collections.Generic;

namespace GZipTest
{
    /// <summary>
    /// Очередь данных на обработку.
    /// </summary>
    public interface ITaskQueue<T>
    {
        /// <summary>
        /// Добавление данных в очередь.
        /// </summary>
        /// <param name="data"></param>
        void Add(T data);

        /// <summary>
        /// Вызывается, когда закончено размещение данных в очереди.
        /// </summary>
        void Completed();

        /// <summary>
        /// Данные на обработку.
        /// </summary>
        IEnumerable<T> Tasks { get; }
    }
}