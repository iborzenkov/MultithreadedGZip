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

#if DEBUG
            var technology = TechnologyMode.Semaphore;
            
            // Компрессия
            using (var compressingTaskQueue = GetCompressingTaskQueue(technology, threadCount))
            using (var writeTaskQueue = GetWriteTaskQueue(technology, threadCount))
            {
                var coordinator = new Coordinator(threadCount, compressingTaskQueue, writeTaskQueue);
                var watch = Stopwatch.StartNew();

                var compressedSettingsProvider = new StubSettingsProvider(
                    new Settings("data\\source.xml", "data\\compressed.gz", CompressionMode.Compress));
                coordinator.Run(compressedSettingsProvider.GetSettings());
                Console.WriteLine($"Время выполнения упаковки: {watch.ElapsedMilliseconds} мс");
            }

            // Декомпрессия
            using (var compressingTaskQueue = GetCompressingTaskQueue(technology, threadCount))
            using (var writeTaskQueue = GetWriteTaskQueue(technology, threadCount))
            {
                var coordinator = new Coordinator(threadCount, compressingTaskQueue, writeTaskQueue);
                var watch = Stopwatch.StartNew();

                var decompressedSettingsProvider = new StubSettingsProvider(
                    new Settings("data\\compressed.gz", "data\\decompressed.xml", CompressionMode.Decompress));
                coordinator.Run(decompressedSettingsProvider.GetSettings());
                Console.WriteLine($"Время выполнения распаковки: {watch.ElapsedMilliseconds} мс");
            }
#else
            var settingsProvider = new SettingsProvider(args);
            var settings = settingsProvider.GetSettings();

            var technology = settings.Technology;
            using (var compressingTaskQueue = GetCompressingTaskQueue(technology, threadCount))
            using (var writeTaskQueue = GetWriteTaskQueue(technology, threadCount))
            {
                var coordinator = new Coordinator(threadCount, compressingTaskQueue, writeTaskQueue);
                coordinator.Run(settings);
            }
#endif

            Environment.Exit(0);
        }

        private static ITaskQueue<byte[]> GetWriteTaskQueue(TechnologyMode technology, int threadCount)
        {
            switch (technology)
            {
                case TechnologyMode.BlockingCollection:
                    return new TaskQueue<byte[]>(threadCount);

                case TechnologyMode.Monitor:
                    return new TaskQueueMonitor<byte[]>(threadCount);

                case TechnologyMode.Semaphore:
                    return new TaskQueueSemaphore<byte[]>(threadCount);

                default:
                    throw new ArgumentOutOfRangeException(nameof(technology), technology, null);
            }
        }

        private static ITaskQueue<DataPortion> GetCompressingTaskQueue(TechnologyMode technology, int threadCount)
        {
            switch (technology)
            {
                case TechnologyMode.BlockingCollection:
                    return new TaskQueue<DataPortion>(threadCount);

                case TechnologyMode.Monitor:
                    return new TaskQueueMonitor<DataPortion>(threadCount);

                case TechnologyMode.Semaphore:
                    return new TaskQueueSemaphore<DataPortion>(threadCount);

                default:
                    throw new ArgumentOutOfRangeException(nameof(technology), technology, null);
            }
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