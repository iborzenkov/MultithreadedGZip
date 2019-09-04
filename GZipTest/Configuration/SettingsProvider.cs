using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest.Configuration
{
    /// <summary>
    /// Провайдер настроек из аргументов командной строки.
    /// </summary>
    internal class SettingsProvider : ISettingsProvider
    {
        public SettingsProvider(string[] args)
        {
            _args = args;
        }

        public Settings GetSettings()
        {
            if (_args == null)
                throw new ArgumentException("Параметры не определены");

            if (_args.Length != 3)
                throw new ArgumentException("Количество параметров должно быть 3");

            var mode = GetMode(_args[0]);
            var sourceFilename = GetSourceFilename(_args[1]);
            var destinationFilename = GetDestinationFilename(_args[2]);

            return new Settings(sourceFilename, destinationFilename, mode);
        }

        private static CompressionMode GetMode(string argument)
        {
            var modeStr = argument;
            if (string.IsNullOrEmpty(modeStr))
                throw new ArgumentException("Не указан режим работы программы");

            switch (modeStr)
            {
                case "compress":
                    return CompressionMode.Compress;

                case "decompress":
                    return CompressionMode.Decompress;

                default:
                    throw new ArgumentException("Режим может быть либо \"compress\", либо \"decompress\"");
            }
        }

        private static string GetSourceFilename(string argument)
        {
            var sourceFilename = argument;
            if (string.IsNullOrEmpty(sourceFilename))
                throw new ArgumentException("Не указан исходный файл");
            if (!File.Exists(sourceFilename))
                throw new FileNotFoundException("Исходный файл не существует");
            return sourceFilename;
        }

        private static string GetDestinationFilename(string argument)
        {
            var destinationFilename = argument;
            if (string.IsNullOrEmpty(destinationFilename))
                throw new ArgumentException("Не указан результирующий файл");

            string directory;
            try
            {
                directory = Path.GetDirectoryName(destinationFilename);
            }
            catch (PathTooLongException)
            {
                throw new IOException("Имя пути превышает максимально допустимую длину");
            }
            catch (Exception exception)
            {
                throw new IOException("Имя пути недопустимое", exception);
            }

            if (!string.IsNullOrEmpty(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception exception)
                {
                    throw new IOException("Не удалось создать директорию для результирующего файла", exception);
                }
            }

            return destinationFilename;
        }

        private readonly string[] _args;
    }
}