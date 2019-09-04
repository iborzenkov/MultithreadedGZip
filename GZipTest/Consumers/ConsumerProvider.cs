using System.Collections.Generic;

namespace GZipTest.Consumers
{
    /// <summary>
    /// ��������� ��������� Consumer'�� (������������).
    /// </summary>
    internal class ConsumerProvider<T>
    {
        /// <summary>
        /// �������� Consumer (�����������) � ���������.
        /// </summary>
        /// <param name="consumer"></param>
        public void Add(BaseConsumer<T> consumer)
        {
            _consumers.Add(consumer);
        }

        /// <summary>
        /// ������������� �������� ����������� ������� �� ��������� ���� ������� ��������� ������.
        /// </summary>
        public void WaitTillTaskCompleted()
        {
            foreach (var consumer in _consumers)
                consumer.Wait();
        }

        private readonly List<BaseConsumer<T>> _consumers = new List<BaseConsumer<T>>();
    }
}