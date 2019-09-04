namespace GZipTest.Configuration
{
    /// <summary>
    /// Stub для настроек.
    /// </summary>
    internal class StubSettingsProvider : ISettingsProvider
    {
        public StubSettingsProvider(Settings settings)
        {
            _settings = settings;
        }

        public Settings GetSettings()
        {
            return _settings;
        }

        private readonly Settings _settings;
    }
}