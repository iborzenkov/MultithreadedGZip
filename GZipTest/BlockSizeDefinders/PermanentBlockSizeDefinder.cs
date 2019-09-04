using System.IO;

namespace GZipTest.BlockSizeDefinders
{
    /// <summary>
    /// Определитель размера очередного блока из НЕСЖАТОГО файла.
    /// </summary>
    /// <remarks>
    /// Размер блока задаётся снаружи и постоянен.
    /// </remarks>
    internal class PermanentBlockSizeDefinder : IBlockSizeDefinder
    {
        public PermanentBlockSizeDefinder(int blockSize)
        {
            _blockSize = blockSize;
        }

        public int BlockSize(Stream stream)
        {
            return _blockSize;
        }

        private readonly int _blockSize;
    }
}