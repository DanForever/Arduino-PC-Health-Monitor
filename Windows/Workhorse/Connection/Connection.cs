namespace HardwareMonitor.Connection
{
	public abstract class AvailableConnection
    {
        public abstract string Name { get; }

		public abstract ActiveConnection Connect();
    }

    public interface ActiveConnection
    {
        public delegate void DataRecievedHandler(ActiveConnection connection, byte[] data, int dataLength);
        public event DataRecievedHandler DataRecieved;

        public bool IsOpen { get; }

        public void Send(byte[] data);
    }
}
