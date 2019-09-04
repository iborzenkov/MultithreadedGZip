using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace GZipTest.Consumers
{
    /// <summary>
    /// Consumer для компрессии данных.
    /// </summary>
    internal class CompressConsumer : BaseCompressConsumer<DataPortion>
    {
        public CompressConsumer(IEnumerable<DataPortion> monitoredTaskQueue)
            : base(monitoredTaskQueue, "CompressConsumer")
        {
        }

        protected override void ConsumerAction()
        {
            foreach (var dataItem in MonitoredTaskQueue)
            {
                // Упаковываем данные
                var compressed = CompressData(dataItem);

                // Сообщаем наблюдателям, что появились новые данные
                Storage?.Put(compressed);
            }
        }

        private DataPortion CompressData(DataPortion dataPortion)
        {
            try
            {
                using (var compressedStream = new MemoryStream())
                {
                    using (var gZipStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
                    {
                        using (var sourceStream = new MemoryStream(dataPortion.Data))
                        {
                            // запаковали данные
                            sourceStream.CopyTo(gZipStream);
                        }
                    }

                    var data = new List<byte>();
                    // записали в выходной поток размер содержимого
                    var compressedLength = checked((int)compressedStream.Length);
                    data.AddRange(BitConverter.GetBytes(compressedLength));
                    // потом само содержимое
                    data.AddRange(compressedStream.ToArray());

                    return new DataPortion(dataPortion.Sequence, data.ToArray());
                }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Ошибка при упаковке данных", exception);
            }
        }
    }
}