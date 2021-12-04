using System;

namespace HardwareMonitor.Connection
{
	[Serializable]
	public class ConnectionFailedException : Exception
	{
		#region Public Properties

		public AvailableConnection AvailableConnection { get; set; }

		#endregion Public Properties

		#region C-Tor

		public ConnectionFailedException(string reason) : base(reason)
		{
		}

		#endregion C-Tor
	}
}
