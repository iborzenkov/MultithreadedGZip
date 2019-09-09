namespace GZipTest.Configuration
{
    /// <summary>
    /// Технология представления и синхронизация очереди
    /// </summary>
    public enum TechnologyMode
    {
        /// <summary>
        /// с помощью System.Collections.Concurrent.BlockingCollection
        /// </summary>
        BlockingCollection,

        /// <summary>
        /// с помощью монитора System.Threading.Monitor
        /// </summary>
        Monitor,

        /// <summary>
        /// с помощью семафора System.Threading.Semaphore
        /// </summary>
        Semaphore
    }
}