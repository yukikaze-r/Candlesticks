using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Candlesticks {
	class Program {

		static void Main(string[] args) {
			Setting.LoadInstance(args[1]);
			Trace.Listeners.Add(new TextLineTraceListener(Setting.Instance.LogFilePath));
			if (args[0] == "--service") {
				ServiceBase.Run(new Service());
			} else {
				new StartingTask().Execute();
				Thread.Sleep(int.MaxValue);
			}


		}
	}
}
