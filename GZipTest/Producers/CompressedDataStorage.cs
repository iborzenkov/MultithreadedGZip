using System.Collections.Generic;
using System.Linq;

namespace GZipTest.Producers
{
    /// <summary>
    /// Хранилище данных.
    /// </summary>
    internal interface IStorage<T>
    {
        /// <summary>
        /// Добавить очередную порцию данных в хранилище.
        /// </summary>
        void Put(T data);
    }

    /// <summary>
    /// Хранилище обработанных (запакованных/распакованных) данных, готовящихся для сохранения в файл.
    /// </summary>
    internal class CompressedDataStorage : IStorage<DataPortion>
    {
        public CompressedDataStorage(ITaskQueue<byte[]> taskQueue)
        {
            _taskQueue = taskQueue;
        }

        public void Put(DataPortion data)
        {
            lock (_locker)
            {
                _innerList.Add(data.Sequence, data.Data);

                CheckReadyData();
            }
        }

        /// <summary>
        /// Проверяет есть ли готовые данные для сохранения.
        /// </summary>
        /// <remarks>
        /// Признаком готовности данных является наличие не пустой непрерывающейся цепочки от текущего элемента.
        /// </remarks>
        private void CheckReadyData()
        {
            var readyData = new List<byte[]>();
            while (_innerList.Any() && _innerList.First().Key == _sequence)
            {
                readyData.Add(_innerList.First().Value);
                _innerList.RemoveAt(0);
                _sequence++;
            }

            foreach (var data in readyData)
            {
                _taskQueue.Add(data);
            }
        }

        private readonly object _locker = new object();

        private readonly SortedList<long, byte[]> _innerList = new SortedList<long, byte[]>();
        private long _sequence;

        private readonly ITaskQueue<byte[]> _taskQueue;
    }
}