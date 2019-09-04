using GZipTest.BlockSizeDefinders;
using System;
using System.IO;
using System.Linq;

namespace GZipTest.IO
{
    /// <summary>
    /// Поблочный "читатель" исходного файла.
    /// </summary>
    internal class Reader : IDisposable
    {
        public Reader(string filename, IBlockSizeDefinder blockSizeDefinder)
        {
            _fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            _blockSizeDefinder = blockSizeDefinder;
        }

        public bool Read(out byte[] data)
        {
            try
            {
                var blockSize = _blockSizeDefinder.BlockSize(_fileStream);

                var buffer = new byte[blockSize];
                var offset = 0;
                var actualSize = _fileStream.Read(buffer, offset, buffer.Length);

                // Закончился файл
                if (actualSize == 0)
                {
                    data = new byte[0];
                    return false;
                }

                // Последний кусочек из файла, который может быть меньше размера блока
                if (actualSize < blockSize)
                {
                    buffer = buffer.Take(actualSize).ToArray();
                }

                data = buffer;
                return true;
            }
            catch (Exception exception)
            {
                throw new IOException("Ошибка при чтении исходного файла", exception);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Реализация "Dispose pattern".
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            // Если вызвали из метода Dispose, а не Gabbage Collector вызвал финализатор ...
            if (disposing)
            {
                // тогда очищаем управляемые ресурсы
                _fileStream.Dispose();
            }

            // Производим очистку неуправляемых ресурсов
            // ...

            _disposed = true;
        }

        private readonly Stream _fileStream;
        private readonly IBlockSizeDefinder _blockSizeDefinder;
        private bool _disposed;
    }
}