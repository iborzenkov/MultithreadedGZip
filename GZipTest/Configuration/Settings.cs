using System.IO.Compression;

namespace GZipTest.Configuration
{
    /// <summary>
    /// Настройки программы.
    /// </summary>
    public class Settings
    {
        public Settings(string sourceFilename, string destinationFilename, CompressionMode mode)
        {
            SourceFilename = sourceFilename;
            DestinationFilename = destinationFilename;
            Mode = mode;
        }

        /// <summary>
        /// Имя исходного файла.
        /// </summary>
        public string SourceFilename { get; }

        /// <summary>
        /// Имя выходного файла.
        /// </summary>
        public string DestinationFilename { get; }

        /// <summary>
        /// Режим работы (компрессия/декомпресиия).
        /// </summary>
        public CompressionMode Mode { get; }
    }
}