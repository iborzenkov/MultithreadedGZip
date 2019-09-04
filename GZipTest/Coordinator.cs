using GZipTest.BlockSizeDefinders;
using GZipTest.Configuration;
using GZipTest.Consumers;
using GZipTest.IO;
using GZipTest.Producers;
using System.IO.Compression;

namespace GZipTest
{
    /// <summary>
    /// Блок, который инициализирует все зависимые части программы.
    /// </summary>
    public class Coordinator
    {
        public Coordinator(int threadCount,
            ITaskQueue<DataPortion> compressingTaskQueue, ITaskQueue<byte[]> writeTaskQueue)
        {
            ThreadCount = threadCount;

            _compressingTaskQueue = compressingTaskQueue;
            _writeTaskQueue = writeTaskQueue;
        }

        /// <summary>
        /// Запуск в работу всех блоков.
        /// </summary>
        public void Run(Settings settings)
        {
            // Хранилище готовых (сжатых) для записи в файл данных.
            var compressedDataStorage = new CompressedDataStorage(_writeTaskQueue);

            // Блок обработки (компресии/декомпрессии) исходных данных.
            var compressConsumers = new ConsumerProvider<DataPortion>();
            for (var i = 0; i < ThreadCount; i++)
            {
                var compressConsumer = GetConsumer(settings.Mode, _compressingTaskQueue);
                compressConsumer.RegisterOutputStorage(compressedDataStorage);
                compressConsumers.Add(compressConsumer);
            }

            using (var writer = new Writer(settings.DestinationFilename))
            {
                // Блок сохранения в файл готовых данных
                var writerConsumer = new WriterConsumer(writer, _writeTaskQueue.Tasks);

                // Блок чтения исходных данных
                var blockSizeDefinder = GetBlockSizeDefinder(settings.Mode);
                var sourceProducer = new SourceDataProducer(_compressingTaskQueue);
                sourceProducer.Run(settings.SourceFilename, blockSizeDefinder);

                // Ждём пока блок компресии не закончит работу
                compressConsumers.WaitTillTaskCompleted();

                // Говорим, что от блока компресии в блок сохранения больше заданий не будет
                _writeTaskQueue.Completed();

                // Ждём пока не закончится сохранение в файл
                writerConsumer.Wait();
            }
        }

        private static BaseCompressConsumer<DataPortion> GetConsumer(
            CompressionMode mode, ITaskQueue<DataPortion> forCompressingTaskQueue)
        {
            BaseCompressConsumer<DataPortion> consumer;
            if (mode == CompressionMode.Compress)
                consumer = new CompressConsumer(forCompressingTaskQueue.Tasks);
            else
                consumer = new DecompressConsumer(forCompressingTaskQueue.Tasks);
            return consumer;
        }

        private IBlockSizeDefinder GetBlockSizeDefinder(CompressionMode mode)
        {
            if (mode == CompressionMode.Compress)
            {
                return new PermanentBlockSizeDefinder(BlockSize);
            }
            return new CompressedBlockSizeDefinder();
        }

        /// <summary>
        /// Размер блока данных, считываемых из исходного файла.
        /// </summary>
        public int BlockSize { get; set; } = 1024 * 1024; // 1 мб

        /// <summary>
        /// Количество рабочих поток компресии. По умолчанию, по числу процессоров
        /// </summary>
        public int ThreadCount { get; }

        /// <summary>
        /// Очередь на компресию
        /// </summary>
        private readonly ITaskQueue<DataPortion> _compressingTaskQueue;

        /// <summary>
        /// Очередь на запись в выходной файл
        /// </summary>
        private readonly ITaskQueue<byte[]> _writeTaskQueue;
    }
}