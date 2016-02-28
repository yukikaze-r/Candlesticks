using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Candlesticks {
	public partial class SettingDialog : Form {
		public SettingDialog() {
			InitializeComponent();
		}

		private void SettingDialog_Load(object sender, EventArgs e) {
			var setting = Setting.Instance;
			oandaAccountId.Text = setting.OandaAccountId;
			oandaBearerToken.Text = setting.OandaBearerToken;
			dataFilePath.Text = setting.DataFilePath;
			dbHost.Text = setting.DBConnection.Host;
			dbPort.Text = setting.DBConnection.Port.ToString();
			dbPassword.Text = setting.DBConnection.Password;
		}

		private void button1_Click(object sender, EventArgs e) {
			var setting = Setting.Instance;
			setting.DataFilePath = dataFilePath.Text;
			setting.OandaAccountId = oandaAccountId.Text;
			setting.OandaBearerToken = oandaBearerToken.Text;
			setting.DBConnection.Host = dbHost.Text;
			setting.DBConnection.Port = int.Parse(dbPort.Text);
			setting.DBConnection.Password = dbPassword.Text;
			setting.Save();
			this.Close();
		}

	}
}
