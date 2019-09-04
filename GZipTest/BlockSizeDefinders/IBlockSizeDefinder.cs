using System.IO;

namespace GZipTest.BlockSizeDefinders
{
    /// <summary>
    /// Определитель размера очередного блока при чтении из потока.
    /// </summary>
    internal interface IBlockSizeDefinder
    {
        /// <summary>
        /// Размер очередного блока при чтении из потока.
        /// </summary>
        int BlockSize(Stream stream);
    }
}