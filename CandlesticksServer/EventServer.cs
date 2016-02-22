using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {
	class EventServer {
		private HashSet<EventSession> sessions = new HashSet<EventSession>();
		private TcpListener listener;

		public EventServer() {
		}

		public void Execute() {
			listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Setting.Instance.ListenPort);
			listener.Start();
			Trace.WriteLine("EventServer listen start port: "+ Setting.Instance.ListenPort);
			while (true) {
				EventSession session = new EventSession(this, listener.AcceptTcpClient());
				lock(sessions) {
					sessions.Add(session);
				}
			}
		}

		public void Send(object packet) {
			lock (sessions) {
				foreach (var session in sessions) {
					session.Send(packet);
				}
			}
		}

		private void CloseSession(EventSession session) {
			lock (sessions) {
				sessions.Remove(session);
			}
		}

		public void Close() {
			lock (sessions) {
				foreach(var session in sessions) {
					session.Close();
				}
				sessions.Clear();
			}
			listener.Stop();
		}

		class EventSession {
			private EventServer server;
			private TcpClient client;
			private NetworkStream stream;

			public EventSession(EventServer server, TcpClient tcpClient) {
				this.server = server;
				this.client = tcpClient;
				this.stream = this.client.GetStream();
				Trace.WriteLine("EventServer Session start " + tcpClient.Client.RemoteEndPoint);
			}

			public void Send(object packet) {
				try {
					BinaryFormatter f = new BinaryFormatter();
					f.Serialize(stream, packet);
					stream.Flush();

				} catch(Exception e) {
					Trace.WriteLine("EventServer Session close: " + e);
					server.CloseSession(this);
					client.Close();
				}
			}

			public void Close() {
				client.Close();
			}

		}
	}
}
