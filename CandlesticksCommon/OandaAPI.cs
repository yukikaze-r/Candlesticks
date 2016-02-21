﻿using System;
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

		public IEnumerable<OandaCandle> GetCandles(DateTime start, DateTime end) {
			string startParam = WebUtility.UrlEncode(XmlConvert.ToString(start, XmlDateTimeSerializationMode.Utc));
			string endParam = WebUtility.UrlEncode(XmlConvert.ToString(end, XmlDateTimeSerializationMode.Utc));
			
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "v1/candles?instrument=USD_JPY&start="+ startParam+"&end="+endParam+"&candleFormat=midpoint&granularity=M1&dailyAlignment=0&alignmentTimezone=America%2FNew_York");
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
		public float volume;
	}
}
