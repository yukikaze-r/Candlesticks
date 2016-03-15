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
			registryKey.Close();
			this.MouseClickPositionUSD_JPY = LoadMouseClickPos("MousePosUsdJpy");
			this.MouseClickPositionEUR_USD = LoadMouseClickPos("MousePosEurUsd");
		}

		private MouseClikPositoin LoadMouseClickPos(string subKeyName) {
			var registryKey = Registry.CurrentUser.CreateSubKey(@"Software\CandleSticks\Setting\"+ subKeyName);
			var result = new MouseClikPositoin() {
				Bid = new Point((int)registryKey.GetValue("bidPosX", 0), (int)registryKey.GetValue("bidPosY", 0)),
				Ask = new Point((int)registryKey.GetValue("askPosX", 0), (int)registryKey.GetValue("askPosY", 0)),
				Settle = new Point((int)registryKey.GetValue("settlePosX", 0), (int)registryKey.GetValue("settlePosY", 0)),
			};
			registryKey.Close();
			return result;
		}

		public void Save() {
			var registryKey = Registry.CurrentUser.CreateSubKey(@"Software\CandleSticks\Setting");
			registryKey.SetValue("oandaAccountId", this.OandaAccountId);
			registryKey.SetValue("oandaBearerToken", this.OandaBearerToken);
			registryKey.SetValue("dataFilePath", this.DataFilePath);
			registryKey.SetValue("dbHost", this.DBConnection.Host);
			registryKey.SetValue("dbPort", this.DBConnection.Port);
			registryKey.SetValue("dbPassword", this.DBConnection.Password);
			registryKey.Close();

			SaveMouseClickPos(this.MouseClickPositionUSD_JPY, "MousePosUsdJpy");
			SaveMouseClickPos(this.MouseClickPositionEUR_USD, "MousePosEurUsd");
		}

		private void SaveMouseClickPos(MouseClikPositoin mouseClickPos, string subKeyName) {

			var registryKeyMousePos = Registry.CurrentUser.CreateSubKey(@"Software\CandleSticks\Setting\"+subKeyName);
			registryKeyMousePos.SetValue("bidPosX", mouseClickPos.Bid.X);
			registryKeyMousePos.SetValue("bidPosY", mouseClickPos.Bid.Y);
			registryKeyMousePos.SetValue("askPosX", mouseClickPos.Ask.X);
			registryKeyMousePos.SetValue("askPosY", mouseClickPos.Ask.Y);
			registryKeyMousePos.SetValue("settlePosX", mouseClickPos.Settle.X);
			registryKeyMousePos.SetValue("settlePosY", mouseClickPos.Settle.Y);
			registryKeyMousePos.Close();
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

		public MouseClikPositoin MouseClickPositionUSD_JPY {
			get;
			set;
		}

		public MouseClikPositoin MouseClickPositionEUR_USD {
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