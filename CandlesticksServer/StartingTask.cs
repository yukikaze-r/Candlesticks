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
				this.eventServer = new EventServer();
				new OrderBookCollector(eventServer);
				new Thread(new ThreadStart(() => { eventServer.Execute(); })).Start();
			} catch(Exception e) {
				Trace.WriteLine(e.ToString());
				throw e;
			}
		}

		public void Stop() {
			this.eventServer.Close();
		}
	}
}
