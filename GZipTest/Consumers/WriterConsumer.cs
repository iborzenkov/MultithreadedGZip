using GZipTest.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace GZipTest.Consumers
{
    /// <summary>
    /// Consumer для блока записи данных в файл.
    /// </summary>
    internal class WriterConsumer : BaseConsumer<byte[]>
    {
        public WriterConsumer(Writer writer, IEnumerable<byte[]> monitoredTaskQueue)
            : base(monitoredTaskQueue, "WriteConsumer")
        {
            _writer = writer;
        }

        protected override void ConsumerAction()
        {
            try
            {
                foreach (var data in MonitoredTaskQueue)
                {
                    _writer.Write(data);
                }
            }
            catch (Exception exception)
            {
                throw new IOException($"Ошибка при записи в файл: \"{_writer.Filename}\"", exception);
            }
        }

        private readonly Writer _writer;
    }
}