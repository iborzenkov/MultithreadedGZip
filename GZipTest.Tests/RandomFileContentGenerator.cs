using System;
using System.IO;

namespace GZipTest.Tests
{
    internal class RandomFileContentGenerator
    {
        /// <summary>
        /// Возвращает файл случайного содержимого указанного размера.
        /// </summary>
        public void Build(string filename, int sizeInMb)
        {
            const int blockSize = 1024 * 8;
            const int blocksPerMb = (1024 * 1024) / blockSize;

            var data = new byte[blockSize];
            var random = new Random();
            using (var stream = File.OpenWrite(filename))
            {
                for (var i = 0; i < sizeInMb * blocksPerMb; i++)
                {
                    random.NextBytes(data);
                    stream.Write(data, 0, data.Length);
                }
            }
        }
    }
}