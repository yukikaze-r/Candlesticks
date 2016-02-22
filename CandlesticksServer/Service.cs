using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {
	class Service : ServiceBase {
		private StartingTask task = null;

		public Service() {
			this.ServiceName = "Cnadlesticks Server";
			this.CanStop = true;
			this.CanPauseAndContinue = true;
			this.AutoLog = true;
		}

		protected override void OnStart(string[] args) {
			task = new StartingTask();
			task.Execute();
		}

		protected override void OnStop() {
			task.Stop();
		}
	}
}
