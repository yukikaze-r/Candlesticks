using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

namespace Candlesticks
{
    class EventReceiver : IDisposable {
		public static EventReceiver Instance = new EventReceiver();

		private TcpClient tcpClient = null;

		public event Action<OrderBookUpdated> OrderBookUpdatedEvent = delegate { };

		public void Execute() {

			try {
				Console.WriteLine("receive start");
				tcpClient = new TcpClient("localhost", 4444);
				var reader = new BinaryReader(tcpClient.GetStream());

				while (true) {
					int length = reader.ReadInt32();
					byte[] body = reader.ReadBytes(length);
					using (var memStream = new MemoryStream(body)) {
						DataContractSerializer ser = new DataContractSerializer(typeof(ServerEvent), new Type[] { typeof(OrderBookUpdated) });
						var receivedObject = ((ServerEvent)ser.ReadObject(memStream)).Body;
						Console.WriteLine("received: " + receivedObject);
						if (receivedObject is OrderBookUpdated) {
							OrderBookUpdatedEvent((OrderBookUpdated)receivedObject);
						}
					}
				}

			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // 重複する呼び出しを検出するには

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					if(tcpClient != null) {
						tcpClient.Close();
						tcpClient = null;
						OrderBookUpdatedEvent = null;
					}
				}
				disposedValue = true;
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
}
