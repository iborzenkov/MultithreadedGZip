namespace GZipTest.Configuration
{
    /// <summary>
    /// ��������� ��������.
    /// </summary>
    internal interface ISettingsProvider
    {
        /// <summary>
        /// ���������� ���������.
        /// </summary>
        Settings GetSettings();
    }
}