namespace GZipTest
{
    /// <summary>
    /// Порция данных.
    /// </summary>
    public class DataPortion
    {
        public DataPortion(int sequence, byte[] data)
        {
            Sequence = sequence;
            Data = data;
        }

        /// <summary>
        /// Последовательный номер порции данных.
        /// </summary>
        public int Sequence { get; }

        /// <summary>
        /// Данные.
        /// </summary>
        public byte[] Data { get; }
    }
}