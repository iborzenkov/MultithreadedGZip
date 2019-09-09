using GZipTest.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GZipTest.Tests
{
    public class CompressTest
    {
        public CompressTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(1, 1, 50)] // ОДИН поток сделает МНОГО работы
        [InlineData(5, 1, 50)] // НЕСКОЛЬКО потоков сделают МНОГО работы
        [InlineData(100, 1, 50)] // ОГРОМНОЕ число потоков сделают МНОГО работы
        [InlineData(100, 5, 3)] // ОГРОМНОЕ число потоков сделают ЕДИНСТВЕННОЕ задание (достанется только одному)
        [InlineData(5, 5, 3)] // НЕСКОЛЬКО потоков выполнят ЕДИНСТВЕННОЕ задание (достанется только одному)
        [InlineData(1, 1, 5)] // ОДИН поток сделает НЕСКОЛЬКО заданий
        [InlineData(1, 5, 3)] // ОДИН поток выполнит ЕДИНСТВЕННОЕ задание
        //[InlineData(10, 1, 3000)] // НЕСКОЛЬКО потоков сделают ОГРОМНОЕ количество работы
        public void TestBlockingCollection(int threadCount, int blockSizeInMb, int sourceFileSizeInMb)
        {
            InternalTest(threadCount, blockSizeInMb, sourceFileSizeInMb, TechnologyMode.BlockingCollection);
        }

        [Theory]
        [InlineData(1, 1, 50)] // ОДИН поток сделает МНОГО работы
        [InlineData(5, 1, 50)] // НЕСКОЛЬКО потоков сделают МНОГО работы
        [InlineData(100, 1, 50)] // ОГРОМНОЕ число потоков сделают МНОГО работы
        [InlineData(100, 5, 3)] // ОГРОМНОЕ число потоков сделают ЕДИНСТВЕННОЕ задание (достанется только одному)
        [InlineData(5, 5, 3)] // НЕСКОЛЬКО потоков выполнят ЕДИНСТВЕННОЕ задание (достанется только одному)
        [InlineData(1, 1, 5)] // ОДИН поток сделает НЕСКОЛЬКО заданий
        [InlineData(1, 5, 3)] // ОДИН поток выполнит ЕДИНСТВЕННОЕ задание
        //[InlineData(10, 1, 3000)] // НЕСКОЛЬКО потоков сделают ОГРОМНОЕ количество работы
        public void TestMonitor(int threadCount, int blockSizeInMb, int sourceFileSizeInMb)
        {
            InternalTest(threadCount, blockSizeInMb, sourceFileSizeInMb, TechnologyMode.Monitor);
        }

        [Theory]
        [InlineData(1, 1, 50)] // ОДИН поток сделает МНОГО работы
        [InlineData(5, 1, 50)] // НЕСКОЛЬКО потоков сделают МНОГО работы
        [InlineData(100, 1, 50)] // ОГРОМНОЕ число потоков сделают МНОГО работы
        [InlineData(100, 5, 3)] // ОГРОМНОЕ число потоков сделают ЕДИНСТВЕННОЕ задание (достанется только одному)
        [InlineData(5, 5, 3)] // НЕСКОЛЬКО потоков выполнят ЕДИНСТВЕННОЕ задание (достанется только одному)
        [InlineData(1, 1, 5)] // ОДИН поток сделает НЕСКОЛЬКО заданий
        [InlineData(1, 5, 3)] // ОДИН поток выполнит ЕДИНСТВЕННОЕ задание
        //[InlineData(10, 1, 3000)] // НЕСКОЛЬКО потоков сделают ОГРОМНОЕ количество работы
        public void TestSemaphore(int threadCount, int blockSizeInMb, int sourceFileSizeInMb)
        {
            InternalTest(threadCount, blockSizeInMb, sourceFileSizeInMb, TechnologyMode.Semaphore);
        }

        private void InternalTest(int threadCount, int blockSizeInMb, int sourceFileSizeInMb, TechnologyMode technology)
        {
            var uniqueFilePart = $"{threadCount}${blockSizeInMb}${sourceFileSizeInMb}";

            var sourceFilename = GetFullFilename($"source{uniqueFilePart}.file");
            var compressedFilename = GetFullFilename($"compressed{uniqueFilePart}.gz");
            var decompressedFilename = GetFullFilename($"decompressed{uniqueFilePart}.file");

            GenerateSourceFile(sourceFilename, sourceFileSizeInMb);
            try
            {
                var watch = Stopwatch.StartNew();

                // Архивируем
                Compress(sourceFilename, compressedFilename, threadCount, blockSizeInMb, technology);

                watch.Stop();
                _testOutputHelper.WriteLine($"Архив: {watch.ElapsedMilliseconds} ms");

                watch.Restart();

                // Разархивируем
                Decompress(compressedFilename, decompressedFilename, threadCount, blockSizeInMb, technology);

                watch.Stop();
                _testOutputHelper.WriteLine($"Разар: {watch.ElapsedMilliseconds} ms");

                Validate(sourceFilename, decompressedFilename);
            }
            finally
            {
                RemoveTempFiles(new[] { sourceFilename, compressedFilename, decompressedFilename });
            }
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

        private static void GenerateSourceFile(string filename, int sizeInMb)
        {
            var fileGenerator = new RandomFileContentGenerator();
            fileGenerator.Build(filename, sizeInMb);
        }

        private static void Compress(string sourceFilename, string compressedFilename,
            int threadCount, int blockSizeInMb, TechnologyMode technology)
        {
            using (var compressingTaskQueue = GetCompressingTaskQueue(technology, threadCount))
            using (var writeTaskQueue = GetWriteTaskQueue(technology, threadCount))
            {
                var blockSize = blockSizeInMb * 1024 * 1024 - 10;
                var coordinator = new Coordinator(threadCount, compressingTaskQueue, writeTaskQueue)
                {
                    BlockSize = blockSize
                };

                var settingsProvider = new StubSettingsProvider(
                    new Settings(sourceFilename, compressedFilename, CompressionMode.Compress));

                coordinator.Run(settingsProvider.GetSettings());
            }
        }

        private static void Decompress(string compressedFilename, string decompressedFilename,
            int threadCount, int blockSizeInMb, TechnologyMode technology)
        {
            using (var compressingTaskQueue = GetCompressingTaskQueue(technology, threadCount))
            using (var writeTaskQueue = GetWriteTaskQueue(technology, threadCount))
            {
                var blockSize = blockSizeInMb / 1024 - 10;
                var coordinator = new Coordinator(threadCount, compressingTaskQueue, writeTaskQueue)
                {
                    BlockSize = blockSize
                };

                var settingsProvider = new StubSettingsProvider(
                    new Settings(compressedFilename, decompressedFilename, CompressionMode.Decompress));

                coordinator.Run(settingsProvider.GetSettings());
            }
        }

        private static void Validate(string filename1, string filename2)
        {
            // Простая проверка на совпадение размеров файлов. По содержимому проверять в некоторых случаях очень накладно.
            var fileSize1 = new FileInfo(filename1).Length;
            var fileSize2 = new FileInfo(filename2).Length;

            Assert.Equal(fileSize1, fileSize2);
        }

        private static void RemoveTempFiles(string[] files)
        {
            foreach (var filename in files.Where(File.Exists))
            {
                try
                {
                    File.Delete(filename);
                }
                catch (Exception)
                {
                    // глушим exception
                }
            }
        }

        private static string GetFullFilename(string filename)
        {
            return Path.Combine(Path.GetTempPath(), filename);
        }

        private readonly ITestOutputHelper _testOutputHelper;
    }
}