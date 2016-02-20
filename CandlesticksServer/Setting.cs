using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {
	class Setting {
		public static Setting Instance = new Setting();

		private Setting() {
			var registryKey = Registry.CurrentUser.CreateSubKey(@"Software\CandleSticks\Setting");
			this.OandaBearerToken = (string)registryKey.GetValue("oandaBearerToken");
			this.DataFilePath = (string)registryKey.GetValue("dataFilePath");
			registryKey.Close();
		}

		public void Save() {
			var registryKey = Registry.CurrentUser.CreateSubKey(@"Software\CandleSticks\Setting");
			registryKey.SetValue("oandaBearerToken", this.OandaBearerToken);
			registryKey.SetValue("dataFilePath", this.DataFilePath);
			registryKey.Close();
		}

		public string OandaBearerToken {
			get;
			set;
		}

		public string DataFilePath {
			get;
			set;
		}
	}
}
