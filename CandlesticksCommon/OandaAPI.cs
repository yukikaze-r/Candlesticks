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
using System.Threading;
using System.Text.RegularExpressions;

namespace Candlesticks {
	class OandaAPI {
		private static string BearerToken {
			get {
				return Setting.Instance.OandaBearerToken;
			}
		}
		private static string AccountId {
			get {
				return Setting.Instance.OandaAccountId;
			}
		}

		private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private HttpClient client;

		public OandaAPI() {
			client = new HttpClient();
			client.BaseAddress = new Uri("https://api-fxpractice.oanda.com/");
		}

		// http://www.oanda.com/lang/ja/forex-trading/analysis/currency-units-calculator
		// JP225_USD  SPX500_USD BCO_USD SGD_HKD  NAS100_USD USB10Y_USD US30_USD WTICO_USD
		public IEnumerable<OandaCandle> GetCandles(DateTime start, DateTime end, string instrument="USD_JPY", string granularity="M30") {
			Console.WriteLine("start:"+start+" end:"+end);
			if(start == end) {
				Console.WriteLine("start == end");
				return new List<OandaCandle>();
			}

			string startParam = WebUtility.UrlEncode(XmlConvert.ToString(start, XmlDateTimeSerializationMode.Utc));
			string endParam = WebUtility.UrlEncode(XmlConvert.ToString(end, XmlDateTimeSerializationMode.Utc));

//			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "v1/candles?instrument=" + instrument + "&start=" + startParam + "&end=" + endParam + "&granularity=" + granularity + "&dailyAlignment=0&alignmentTimezone=Asia%2FTokyo");
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "v1/candles?instrument="+ instrument + "&start="+ startParam+"&end="+endParam+"&candleFormat=midpoint&granularity="+ granularity+"&dailyAlignment=0&alignmentTimezone=Asia%2FTokyo");
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", OandaAPI.BearerToken);

			Console.WriteLine(request.ToString());

			Task<HttpResponseMessage> webTask = client.SendAsync(request);
			webTask.Wait();

			Task<String> readTask = webTask.Result.Content.ReadAsStringAsync();
			readTask.Wait();

			if(webTask.Result.StatusCode == HttpStatusCode.NoContent) {
				Console.WriteLine("205 NoContent");
				return new List<OandaCandle>();
			}

			if (webTask.Result.StatusCode != HttpStatusCode.OK) {
				Console.WriteLine("HttpStatus:" + webTask.Result.StatusCode+" "+ readTask.Result);
				throw new Exception(readTask.Result);
			}
//			Console.WriteLine(readTask.Result);

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

		// 停止するにはHttpClientをDispose
		public HttpClient GetPrices(Action<float,float> receiver, string instrument = "USD_JPY") {
			HttpClient client = new HttpClient();
			new Thread(new ThreadStart(async () => {
				client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
				client.BaseAddress = new Uri("https://stream-fxpractice.oanda.com/");
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "v1/prices?accountId=" + AccountId + "&instruments=" + instrument);
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", OandaAPI.BearerToken);

				HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
				StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync());
				var regex = new Regex(@"""bid"":(\d+(\.\d+)?),""ask"":(\d+(\.\d+)?)");
				try {
					do {
						string s = reader.ReadLine();
						Console.WriteLine(">" + s);
						var match = regex.Match(s);
						if(match.Success) {
							receiver(float.Parse(match.Groups[1].Value), float.Parse(match.Groups[3].Value));
						}
					} while (true);
				} catch (IOException ex) {
					Console.WriteLine(ex);
				}
			})).Start();
			return client;
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
