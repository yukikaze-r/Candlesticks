using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Candlesticks
{
	class DBUtils {
		public static NpgsqlConnection OpenConnection() {
			var dbSetting = Setting.Instance.DBConnection;
			var conn = new NpgsqlConnection(
				"Host=" + dbSetting.Host +
				";Port=" + dbSetting.Port +
				";Username=" + dbSetting.UserName +
				";Password=" + dbSetting.Password +
				";Database=" + dbSetting.Database);
			conn.Open();
			return conn;
		}
	}
}
