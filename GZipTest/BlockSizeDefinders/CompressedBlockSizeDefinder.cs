using System;
using System.IO;

namespace GZipTest.BlockSizeDefinders
{
    /// <summary>
    /// Определитель размера очередного блока из СЖАТОГО файла.
    /// </summary>
    /// <remarks>
    /// Размер блока хранится в самом потоке.
    /// </remarks>
    internal class CompressedBlockSizeDefinder : IBlockSizeDefinder
    {
        public int BlockSize(Stream stream)
        {
            var sizeBuffer = new byte[Size];
            var offset = 0;
            // читаем размер блока, хранящийся в очередных 4 байтах в самом потоке
            stream.Read(sizeBuffer, offset, Size);

            return BitConverter.ToInt32(sizeBuffer, 0);
        }

        private const int Size = sizeof(Int32);
    }
}