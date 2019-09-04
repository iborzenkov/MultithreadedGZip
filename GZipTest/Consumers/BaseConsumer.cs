using System.Collections.Generic;
using System.Threading;

namespace GZipTest.Consumers
{
    /// <summary>
    /// ������� ����� ��� ���� Consumer (������������) ������.
    /// </summary>
    internal abstract class BaseConsumer<T>
    {
        protected BaseConsumer(IEnumerable<T> monitoredTaskQueue, string name = null)
        {
            MonitoredTaskQueue = monitoredTaskQueue;

            _thread = new Thread(ConsumerAction) { Name = name };
            _thread.Start();
        }

        /// <summary>
        /// ������������ �������� ����������� ������� �� ��������� ������ ��������� ������.
        /// </summary>
        public void Wait()
        {
            _thread.Join();
        }

        protected abstract void ConsumerAction();

        protected IEnumerable<T> MonitoredTaskQueue;

        private readonly Thread _thread;
    }
}