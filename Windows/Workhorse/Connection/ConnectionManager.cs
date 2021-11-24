using System.Collections.Generic;
using System.Linq;

namespace HardwareMonitor.Connection
{
    public class Manager
	{
		private List<ActiveConnection> _activeConnections = new List<ActiveConnection>();

		public bool IsConnected => VerifyConnections();

		public static IEnumerable<AvailableConnection> EnumerateAvailableConnections()
        {
            foreach (AvailableConnection availableConnection in Serial.EnumeratePorts())
            {
                yield return availableConnection;
            }
        }

        public delegate void NewActiveConnectionHandler(ActiveConnection connection);
        public event NewActiveConnectionHandler NewActiveConnection;

        public void Connect(AvailableConnection availableConnection)
        {
            AvailableSerialConnection asc = availableConnection as AvailableSerialConnection;
            if(asc is not null)
            {
                ActiveSerialConnection activeSerialConnection = new ActiveSerialConnection(asc);
                _activeConnections.Add(activeSerialConnection);

                NewActiveConnection?.Invoke(activeSerialConnection);
            } 
        }

        public void Send(params dynamic[] args)
        {
            SimplePacket packet = new SimplePacket();
            packet.Connections = _activeConnections.ToArray();
            packet.Send(args);
        }

        public async void SendGuaranteed(params dynamic[] args)
        {
            GuaranteedPacket packet = new GuaranteedPacket();
            packet.Connections = _activeConnections.ToList();
            await packet.SendAsync(args);
        }

        public GuaranteedPacket SendGuaranteed()
        {
            GuaranteedPacket packet = new GuaranteedPacket();
            packet.Connections = _activeConnections.ToList();

            return packet;
        }

		private bool VerifyConnections()
		{
			_activeConnections.RemoveAll(connection => !connection.IsOpen);

			return _activeConnections.Count > 0;
		}
    }
}
