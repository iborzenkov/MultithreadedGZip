using System.IO.Compression;

namespace GZipTest.Configuration
{
    /// <summary>
    /// Настройки программы.
    /// </summary>
    public class Settings
    {
        public Settings(string sourceFilename, string destinationFilename, CompressionMode mode)
            : this(sourceFilename, destinationFilename, mode, TechnologyMode.BlockingCollection)
        {
        }

        public Settings(string sourceFilename, string destinationFilename, CompressionMode mode, TechnologyMode technology)
        {
            SourceFilename = sourceFilename;
            DestinationFilename = destinationFilename;
            Mode = mode;
            Technology = technology;
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

        /// <summary>
        /// Режим работы (компрессия/декомпресиия).
        /// </summary>
        public TechnologyMode Technology { get; }
    }
}