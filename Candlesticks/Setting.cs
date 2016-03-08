using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {

	class Setting {
		public static Setting Instance = new Setting();

		private Setting() {
			var registryKey = Registry.CurrentUser.CreateSubKey(@"Software\CandleSticks\Setting");
			this.OandaAccountId = (string)registryKey.GetValue("oandaAccountId");
			this.OandaBearerToken = (string)registryKey.GetValue("oandaBearerToken");
			this.DataFilePath = (string)registryKey.GetValue("dataFilePath");
			this.DBConnection = new DBConnectionSetting() {
				Host = (string)registryKey.GetValue("dbHost","localhost"),
				Port = (int)registryKey.GetValue("dbUserName",5432),
				Password = (string)registryKey.GetValue("dbPassword"),
			};
			this.MouseClickPosition = new MouseClikPositoin() {
				Bid = new Point((int)registryKey.GetValue("bidPosX", 0), (int)registryKey.GetValue("bidPosY", 0)),
				Ask = new Point((int)registryKey.GetValue("askPosX", 0), (int)registryKey.GetValue("askPosY", 0)),
				Settle = new Point((int)registryKey.GetValue("settlePosX", 0), (int)registryKey.GetValue("settlePosY", 0)),
			};
			registryKey.Close();
		}

		public void Save() {
			var registryKey = Registry.CurrentUser.CreateSubKey(@"Software\CandleSticks\Setting");
			registryKey.SetValue("oandaAccountId", this.OandaAccountId);
			registryKey.SetValue("oandaBearerToken", this.OandaBearerToken);
			registryKey.SetValue("dataFilePath", this.DataFilePath);
			registryKey.SetValue("dbHost", this.DBConnection.Host);
			registryKey.SetValue("dbPort", this.DBConnection.Port);
			registryKey.SetValue("dbPassword", this.DBConnection.Password);
			registryKey.SetValue("bidPosX", this.MouseClickPosition.Bid.X);
			registryKey.SetValue("bidPosY", this.MouseClickPosition.Bid.Y);
			registryKey.SetValue("askPosX", this.MouseClickPosition.Ask.X);
			registryKey.SetValue("askPosY", this.MouseClickPosition.Ask.Y);
			registryKey.SetValue("settlePosX", this.MouseClickPosition.Settle.X);
			registryKey.SetValue("settlePosY", this.MouseClickPosition.Settle.Y);
			registryKey.Close();
		}

		public string OandaAccountId {
			get;
			set;
		}

		public string OandaBearerToken {
			get;
			set;
		}

		public string DataFilePath {
			get;
			set;
		}

		public DBConnectionSetting DBConnection {
			get;
			set;
		}

		public class DBConnectionSetting {
			public string Host = "localhost";
			public int Port = 5432;
			public string UserName = "candlesticks";
			public string Password = null;
			public string Database = "candlesticks";
		}

		public MouseClikPositoin MouseClickPosition {
			get;
			set;
		}

		public class MouseClikPositoin {
			public Point Bid;
			public Point Ask;
			public Point Settle;
		}
	}
}