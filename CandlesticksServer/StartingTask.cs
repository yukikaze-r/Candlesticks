using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Candlesticks {
	class StartingTask {
		private EventServer eventServer;

		public void Execute() {
			this.eventServer = new EventServer();
			new OrderBookCollector(eventServer);
			new Thread(new ThreadStart(()=> { eventServer.Execute(); })).Start();
		}

		public void Stop() {
			this.eventServer.Close();
		}
	}
}
