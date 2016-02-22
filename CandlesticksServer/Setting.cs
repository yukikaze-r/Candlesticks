
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {

	[DataContract]
	class Setting {
		public static Setting Instance;

		public static void LoadInstance(string filePath) {
			var settings = new DataContractJsonSerializerSettings();
			settings.UseSimpleDictionaryFormat = true;
			var serializer = new DataContractJsonSerializer(typeof(Setting), settings);
			using (var stream = new FileStream(filePath, FileMode.Open)) {
				Instance = (Setting)serializer.ReadObject(stream);
			}
		}

		[DataMember]
		public string LogFilePath = null;

		[DataMember]
		public int ListenPort = -1;

		[DataMember]
		public string OandaBearerToken = null;

		[DataMember]
		public DBConnectionSetting DBConnection = null;

		[DataContract]
		public class DBConnectionSetting {
			[DataMember]
			public string Host = "localhost";
			[DataMember]
			public int Port = 5432;
			[DataMember]
			public string UserName = "candlesticks";
			[DataMember]
			public string Password = null;
			[DataMember]
			public string Database = "candlesticks";
		}
	}
}
