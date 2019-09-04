using GZipTest.Configuration;
using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GZipTest.Tests")]

namespace GZipTest
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var threadCount = Math.Max(1, Environment.ProcessorCount);

            using (var compressingTaskQueue = new TaskQueue<DataPortion>(threadCount))
            using (var writeTaskQueue = new TaskQueue<byte[]>(threadCount))
            {
                var coordinator = new Coordinator(threadCount, compressingTaskQueue, writeTaskQueue);

#if DEBUG
                // Для отладки
                var compressedSettingsProvider = new StubSettingsProvider(
                    new Settings("data\\source.xml", "data\\compressed.gz", CompressionMode.Compress));

                var decompressedSettingsProvider = new StubSettingsProvider(
                    new Settings("data\\compressed.gz", "data\\decompressed.xml", CompressionMode.Decompress));

                var watch = Stopwatch.StartNew();

                coordinator.Run(compressedSettingsProvider.GetSettings());
                Console.WriteLine($"Время выполнения упаковки: {watch.ElapsedMilliseconds} мс");

                watch.Restart();
                coordinator.Run(decompressedSettingsProvider.GetSettings());
                Console.WriteLine($"Время выполнения распаковки: {watch.ElapsedMilliseconds} мс");
#else
            var settingsProvider = new SettingsProvider(args);
            coordinator.Run(settingsProvider.GetSettings());

#endif
            }

            Environment.Exit(0);
        }

        /// <summary>
        /// Обработчик всех exception'ов.
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            Console.WriteLine(exception.Message);

            Environment.Exit(1);
        }
    }
}