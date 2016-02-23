using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Candlesticks {
	class StartingTask {
		private EventServer eventServer;

		public void Execute() {
			try {
				Trace.WriteLine("Candlesticks Server starting...");
				this.eventServer = new EventServer();
				new OrderBookCollector(eventServer);
				new Thread(new ThreadStart(() => { eventServer.Execute(); })).Start();
			} catch(Exception e) {
				Trace.WriteLine(e.ToString());
				throw e;
			}
			Trace.WriteLine("Candlesticks Server started");
		}

		public void Stop() {
			Trace.WriteLine("Candlesticks Server stopping...");
			try {
				this.eventServer.Close();
			} catch (Exception e) {
				Trace.WriteLine(e.ToString());
				throw e;
			}
			Trace.WriteLine("Candlesticks Server setopped");
		}
	}
}
