using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Runtime.Serialization;
using System.Net;

namespace Candlesticks {
	class OandaAPI {
		private static string BearerToken {
			get {
				return Setting.Instance.OandaBearerToken;
			}
		}

		private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private HttpClient client;

		public OandaAPI() {
			client = new HttpClient();
			client.BaseAddress = new Uri("https://api-fxpractice.oanda.com/");
		}

		// http://www.oanda.com/lang/ja/forex-trading/analysis/currency-units-calculator
		// JP225_USD  SPX500_USD BCO_USD SGD_HKD  NAS100_USD USB10Y_USD US30_USD
		public IEnumerable<OandaCandle> GetCandles(DateTime start, DateTime end, string instrument="USD_JPY", string granularity="M30") {
			string startParam = WebUtility.UrlEncode(XmlConvert.ToString(start, XmlDateTimeSerializationMode.Utc));
			string endParam = WebUtility.UrlEncode(XmlConvert.ToString(end, XmlDateTimeSerializationMode.Utc));
			
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "v1/candles?instrument="+ instrument + "&start="+ startParam+"&end="+endParam+"&candleFormat=midpoint&granularity="+ granularity+"&dailyAlignment=0&alignmentTimezone=Asia%2FTokyo");
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", OandaAPI.BearerToken);

			Console.WriteLine(request.ToString());

			Task<HttpResponseMessage> webTask = client.SendAsync(request);
			webTask.Wait();

			Task<String> readTask = webTask.Result.Content.ReadAsStringAsync();
			readTask.Wait();

			if (webTask.Result.StatusCode != HttpStatusCode.OK) {
				Console.WriteLine("HttpStatus:" + webTask.Result.StatusCode+" "+ readTask.Result);
				throw new Exception(readTask.Result);
			}

			var settings = new DataContractJsonSerializerSettings();
			settings.UseSimpleDictionaryFormat = true;
			var serializer = new DataContractJsonSerializer(typeof(OandaCandles), settings);
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(readTask.Result))) {
				return ((OandaCandles)serializer.ReadObject(ms)).candles;
			}
		}

		public Dictionary<DateTime, PricePoints> GetOrderbookData(int period = 43200) {
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "labs/v1/orderbook_data?instrument=USD_JPY&period="+ period);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", OandaAPI.BearerToken);
			
			Task<HttpResponseMessage> webTask = client.SendAsync(request);
			webTask.Wait();

			Task<String> readTask = webTask.Result.Content.ReadAsStringAsync();
			readTask.Wait();

			if (webTask.Result.StatusCode != HttpStatusCode.OK) {
				throw new Exception(readTask.Result);
			}

			var settings = new DataContractJsonSerializerSettings();
			settings.UseSimpleDictionaryFormat = true;
			var serializer = new DataContractJsonSerializer(typeof(Dictionary<long,PricePoints>), settings);
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(readTask.Result))) {
				Dictionary<DateTime, PricePoints> result = new Dictionary<DateTime, PricePoints>();
				foreach(var e in (Dictionary < long,PricePoints > ) serializer.ReadObject(ms)) {
					result[UNIX_EPOCH.AddSeconds(e.Key).ToLocalTime()] = e.Value;
				}
				return result;
			}
		}

	}

	class Candles {
		private string json;

		public Candles(string json) {
			this.json = json;
		}

		public IEnumerable<OandaCandle> Parse() {
			var settings = new DataContractJsonSerializerSettings();
			settings.UseSimpleDictionaryFormat = true;
			var serializer = new DataContractJsonSerializer(typeof(OandaCandles), settings);
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json))) {
				return ((OandaCandles)serializer.ReadObject(ms)).candles;
			}
		}
	}

	[DataContract]
	class PricePoints {
		[DataMember]
		public Dictionary<float, PricePoint> price_points;
		[DataMember]
		public float rate;
	}

	[DataContract]
	class PricePoint {
		[DataMember]
		public float os;
		[DataMember]
		public float ps;
		[DataMember]
		public float pl;
		[DataMember]
		public float ol;
	}

	[DataContract]
	class OandaCandles {
		[DataMember]
		public List<OandaCandle> candles;
	}

	[DataContract]
	class OandaCandle {
		[DataMember]
		public string time;

		[DataMember]
		public float openMid;

		[DataMember]
		public float highMid;

		[DataMember]
		public float lowMid;

		[DataMember]
		public float closeMid;

		[DataMember]
		public int volume;

		public DateTime DateTime {
			get {
				return XmlConvert.ToDateTime(time, XmlDateTimeSerializationMode.Local);
			}
		}

		public Candlestick Candlestick {
			get {
				Candlestick s = new Candlestick();
				s.Time = this.DateTime;
				s.Open = this.openMid;
				s.Close = this.closeMid;
				s.High = this.highMid;
				s.Low = this.lowMid;
				s.Volume = this.volume;
				return s;
			}
		}


	}
}
