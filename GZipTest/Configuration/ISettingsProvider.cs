namespace GZipTest.Configuration
{
    /// <summary>
    /// Провайдер настроек.
    /// </summary>
    internal interface ISettingsProvider
    {
        /// <summary>
        /// Возвращает настройки.
        /// </summary>
        Settings GetSettings();
    }
}