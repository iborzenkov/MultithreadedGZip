using GZipTest.BlockSizeDefinders;
using GZipTest.IO;

namespace GZipTest.Producers
{
    /// <summary>
    /// Поставщик исходных данных поблочным чтением исходного файла.
    /// </summary>
    internal class SourceDataProducer
    {
        public SourceDataProducer(ITaskQueue<DataPortion> taskQueue)
        {
            _taskQueue = taskQueue;
        }

        /// <summary>
        /// Запуск чтения файла.
        /// </summary>
        /// <param name="filename">Имя исходного файла.</param>
        /// <param name="blockSizeDefinder">Тот, кто умеет определять размер очередной порции данных для чтения.</param>
        public void Run(string filename, IBlockSizeDefinder blockSizeDefinder)
        {
            using (var reader = new Reader(filename, blockSizeDefinder))
            {
                var index = 0;

                byte[] data;
                while (reader.Read(out data))
                {
                    var item = new DataPortion(index, data);
                    _taskQueue.Add(item);

                    index++;
                }

                _taskQueue.Completed();
            }
        }

        private readonly ITaskQueue<DataPortion> _taskQueue;
    }
}