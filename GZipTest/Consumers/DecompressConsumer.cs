using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace GZipTest.Consumers
{
    /// <summary>
    /// Consumer для декомпрессии данных.
    /// </summary>
    internal class DecompressConsumer : BaseCompressConsumer<DataPortion>
    {
        public DecompressConsumer(IEnumerable<DataPortion> monitoredTaskQueue)
            : base(monitoredTaskQueue, "DecompressConsumer")
        {
        }

        protected override void ConsumerAction()
        {
            foreach (var dataItem in MonitoredTaskQueue)
            {
                // Распаковываем данные
                var decompressed = DecompressData(dataItem);

                // Сообщаем наблюдателям, что появились новые данные
                Storage?.Put(decompressed);
            }
        }

        private DataPortion DecompressData(DataPortion dataPortion)
        {
            try
            {
                using (var stream = new MemoryStream(dataPortion.Data))
                {
                    using (var decompressedStream = new MemoryStream())
                    {
                        using (var gZipStream = new GZipStream(stream, CompressionMode.Decompress, true))
                        {
                            // распаковали данные
                            gZipStream.CopyTo(decompressedStream);
                        }
                        return new DataPortion(dataPortion.Sequence, decompressedStream.ToArray());
                    }
                }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Ошибка при распаковке данных", exception);
            }
        }
    }
}