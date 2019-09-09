using GZipTest.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using Xunit;

namespace GZipTest.Tests
{
    public class SettingsProviderTest
    {
        [Fact]
        public void TestNull()
        {
            var provider = new SettingsProvider(null);
            Assert.Throws<ArgumentException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestFewParametersCount()
        {
            var args = new string[2];
            var provider = new SettingsProvider(args);
            Assert.Throws<ArgumentException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestManyParametersCount()
        {
            var args = new string[5];
            var provider = new SettingsProvider(args);
            Assert.Throws<ArgumentException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestIncorrectMode()
        {
            var args = new string[3];
            args[0] = "qwe";
            args[1] = GetCorrectFilename;
            args[2] = GetCorrectFilename;
            var provider = new SettingsProvider(args);
            Assert.Throws<ArgumentException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestIncorrectTechnology()
        {
            var args = new string[4];
            args[0] = "qwe";
            args[1] = GetCorrectFilename;
            args[2] = GetCorrectFilename;
            args[3] = "unknown technology";
            var provider = new SettingsProvider(args);
            Assert.Throws<ArgumentException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestUnexistingSourceFilename()
        {
            var args = new string[3];
            args[0] = "compress";
            args[1] = GetRandomFilename;
            args[2] = GetCorrectFilename;
            var provider = new SettingsProvider(args);
            Assert.Throws<FileNotFoundException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestNullSourceFilename()
        {
            var args = new string[3];
            args[0] = "compress";
            args[1] = null;
            args[2] = GetCorrectFilename;
            var provider = new SettingsProvider(args);
            Assert.Throws<ArgumentException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestEmptySourceFilename()
        {
            var args = new string[3];
            args[0] = "compress";
            args[1] = "";
            args[2] = GetCorrectFilename;
            var provider = new SettingsProvider(args);
            Assert.Throws<ArgumentException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestNullDestinationFilename()
        {
            var args = new string[3];
            args[0] = "compress";
            args[1] = GetCorrectFilename;
            args[2] = null;
            var provider = new SettingsProvider(args);
            Assert.Throws<ArgumentException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestEmptyDestinationFilename()
        {
            var args = new string[3];
            args[0] = "compress";
            args[1] = GetCorrectFilename;
            args[2] = "";
            var provider = new SettingsProvider(args);
            Assert.Throws<ArgumentException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestInvalidCharactersDestinationFilename()
        {
            var args = new string[3];
            args[0] = "compress";
            args[1] = GetCorrectFilename;
            args[2] = "e:\\qwe*?\\file.txt";
            var provider = new SettingsProvider(args);
            Assert.Throws<IOException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestPathTooLongDestinationFilename()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < 5000; i++)
            {
                sb.Append("1");
            }

            var args = new string[3];
            args[0] = "compress";
            args[1] = GetCorrectFilename;
            args[2] = sb.ToString();
            var provider = new SettingsProvider(args);
            Assert.Throws<IOException>(() => provider.GetSettings());
        }

        [Fact]
        public void TestCorrectCompress()
        {
            var sourceFilename = GetCorrectFilename;
            var destinationFilename = GetCorrectFilename;

            var args = new string[3];
            args[0] = "compress";
            args[1] = sourceFilename;
            args[2] = destinationFilename;
            var provider = new SettingsProvider(args);
            var settings = provider.GetSettings();

            Assert.NotNull(settings);
            Assert.Equal(CompressionMode.Compress, settings.Mode);
            Assert.Equal(sourceFilename, settings.SourceFilename);
            Assert.Equal(destinationFilename, settings.DestinationFilename);
        }

        [Fact]
        public void TestCorrectCompressWithTechnology()
        {
            var sourceFilename = GetCorrectFilename;
            var destinationFilename = GetCorrectFilename;

            var args = new string[4];
            args[0] = "compress";
            args[1] = sourceFilename;
            args[2] = destinationFilename;
            args[3] = "monitor";
            var provider = new SettingsProvider(args);
            var settings = provider.GetSettings();

            Assert.NotNull(settings);
            Assert.Equal(CompressionMode.Compress, settings.Mode);
            Assert.Equal(sourceFilename, settings.SourceFilename);
            Assert.Equal(destinationFilename, settings.DestinationFilename);
            Assert.Equal(TechnologyMode.Monitor, settings.Technology);
        }

        [Fact]
        public void TestCorrectDecompress()
        {
            var sourceFilename = GetCorrectFilename;
            var destinationFilename = GetCorrectFilename;

            var args = new string[3];
            args[0] = "decompress";
            args[1] = sourceFilename;
            args[2] = destinationFilename;
            var provider = new SettingsProvider(args);
            var settings = provider.GetSettings();

            Assert.NotNull(settings);
            Assert.Equal(CompressionMode.Decompress, settings.Mode);
            Assert.Equal(sourceFilename, settings.SourceFilename);
            Assert.Equal(destinationFilename, settings.DestinationFilename);
        }

        [Fact]
        public void TestCorrectDecompressWithTechnology()
        {
            var sourceFilename = GetCorrectFilename;
            var destinationFilename = GetCorrectFilename;

            var args = new string[4];
            args[0] = "decompress";
            args[1] = sourceFilename;
            args[2] = destinationFilename;
            args[3] = "semaphore";
            var provider = new SettingsProvider(args);
            var settings = provider.GetSettings();

            Assert.NotNull(settings);
            Assert.Equal(CompressionMode.Decompress, settings.Mode);
            Assert.Equal(sourceFilename, settings.SourceFilename);
            Assert.Equal(destinationFilename, settings.DestinationFilename);
            Assert.Equal(TechnologyMode.Semaphore, settings.Technology);
        }

        private string GetCorrectFilename => Process.GetCurrentProcess().MainModule.FileName;
        private string GetRandomFilename => new Random().Next().ToString();
    }
}