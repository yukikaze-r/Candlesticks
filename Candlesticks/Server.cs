using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {
	class Server {
		public static readonly int PORT = 4444;

		private TcpClient client;
		private NetworkStream stream;
		

		public Server() {
			var listener = new TcpListener(IPAddress.Parse("127.0.0.1"),PORT);
			listener.Start();

			this.client = listener.AcceptTcpClient();
			this.stream = this.client.GetStream();
		}

		public void Send(object packet) {
			BinaryFormatter f = new BinaryFormatter();
			f.Serialize(stream,packet);
		}

	}
}
