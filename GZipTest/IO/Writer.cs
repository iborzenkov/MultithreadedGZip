using System;
using System.IO;

namespace GZipTest.IO
{
    /// <summary>
    /// "Писатель" данных в файл.
    /// </summary>
    internal class Writer : IDisposable
    {
        public Writer(string filename)
        {
            Filename = filename;
        }

        /// <summary>
        /// Записывает порцию данных в файл.
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data)
        {
            // Отложенная инициализация потока записи. Может до записи дело и не дойдёт никогда.
            EnsureFileStreamReady();

            _fileStream.Write(data, 0, data.Length);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void EnsureFileStreamReady()
        {
            if (_fileStream != null)
                return;

            try
            {
                _fileStream = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            }
            catch (Exception exception)
            {
                throw new IOException("Ошибка при записи в выходной файл", exception);
            }
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
                _fileStream?.Dispose();
            }

            // Производим очистку неуправляемых ресурсов
            // ...

            _disposed = true;
        }


        public string Filename { get; }
        private Stream _fileStream;
        private bool _disposed;
    }
}