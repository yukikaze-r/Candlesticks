using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Candlesticks
{
	class DBUtils {
		private static ThreadLocal<NpgsqlConnection> threadConnection = new ThreadLocal<NpgsqlConnection>();

		public static NpgsqlConnection OpenConnection() {
			var dbSetting = Setting.Instance.DBConnection;
			var conn = new NpgsqlConnection(
				"Host=" + dbSetting.Host +
				";Port=" + dbSetting.Port +
				";Username=" + dbSetting.UserName +
				";Password=" + dbSetting.Password +
				";Database=" + dbSetting.Database );
			conn.Open();
			return conn;
		}

		public static NpgsqlConnection GetConnection() {
			return threadConnection.Value;
		}

		public static ThreadConnection OpenThreadConnection() {
			if(threadConnection.Value != null) {
				throw new Exception("Already exists thread connection (TODO nested support)");
			}
			threadConnection.Value = OpenConnection();
			return new ThreadConnection();
		}

		public class ThreadConnection : IDisposable {

			#region IDisposable Support
			private bool disposedValue = false; // 重複する呼び出しを検出するには

			protected virtual void Dispose(bool disposing) {
				if (!disposedValue) {
					if (disposing) {
						DBUtils.threadConnection.Value.Close();
						DBUtils.threadConnection.Value = null;
					}
					disposedValue = true;
				}
			}

			public void Dispose() {
				Dispose(true);
			}
			#endregion

		}
	}
}
