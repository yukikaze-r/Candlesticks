using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Candlesticks {
	class CandlesticksGetter {
		public string Instrument = "USD_JPY";

		/*
			１分の初めにアライン
			“S5” - 5 秒
			“S10” - 10 秒
			“S15” - 15 秒
			“S30” - 30 秒
			“M1” - 1 分
			1時間の初めにアライン
			“M2” - 2 分
			“M3” - 3 分
			“M5” - 5 分
			“M10” - 10 分
			“M15” - 15 分
			“M30” - 30 分
			“H1” - 1 時間
			1日の初めにアライン(17:00, 米国東部標準時)
			“H2” - 2 時間
			“H3” - 3 時間
			“H4” - 4 時間
			“H6” - 6 時間
			“H8” - 8 時間
			“H12” - 12 時間
			“D” - 1 日
			1週間の初めにアライン (土曜日)
			“W” - 1 週
			1か月の初めにアライン (その月の最初の日)
			“M” - 1 か月
		*/
		public string Granularity = "M10";
		public DateTime Start;
		public DateTime End {
			set {
				end = value;
				if(end > DateTime.Now) {
					end = DateTime.Now;
				}
			}
			get {
				return end;
			}
		}
		private DateTime end;
		public int Count = -1;
		private OandaAPI oandaAPI = null;
		public OandaAPI OandaAPI {
			get {
				if(oandaAPI == null) {
					oandaAPI = new OandaAPI();
				}
				return oandaAPI;
			}
			set {
				oandaAPI = value;
			}
		}


		private void SaveOandaCandle(CandlestickDao dao, OandaCandle oandaCandle) {
			var entity = dao.CreateNewEntity();
			entity.Instrument = this.Instrument;
			entity.Granularity = this.Granularity;
			entity.DateTime = oandaCandle.DateTime;
			entity.Open = oandaCandle.openMid;
			entity.High = oandaCandle.highMid;
			entity.Low = oandaCandle.lowMid;
			entity.Close = oandaCandle.closeMid;
			entity.Volume = oandaCandle.volume;
			try {
				entity.Save();
			} catch(NpgsqlException e) {
				if(e.Code == "23505") {
					Console.WriteLine(e.Message);
				} else {
					throw e;
				}
			}
		}

		private void SaveNullCandle(CandlestickDao dao, DateTime t) {
			var entity = dao.CreateNewEntity();
			entity.Instrument = this.Instrument;
			entity.Granularity = this.Granularity;
			entity.DateTime = t;
			entity.Open = 0;
			entity.High = 0;
			entity.Low = 0;
			entity.Close = 0;
			entity.Volume = 0;

			try {
				entity.Save();
			} catch (NpgsqlException e) {
				if (e.Code == "23505") {
					Console.WriteLine(e.Message);
				} else {
					throw e;
				}
			}
		}

		public IEnumerable<Candlestick> Execute() {
			List<Candlestick> result = new List<Candlestick>();

			TimeSpan granularitySpan = GetGranularitySpan();
			if (Count != -1) {
				End = Start.AddTicks(granularitySpan.Ticks * Count);
			}

			var dao = new CandlestickDao();
			using(var transaction = DBUtils.GetConnection().BeginTransaction()) {
				DateTime t = GetAlignTime(Start);

				foreach (var entity in dao.GetBy(Instrument, Granularity, t, End).ToList()) {
					if(entity.DateTime != t) {
						foreach (var oandaCandle in GetCandles(t, entity.DateTime.AddSeconds(-1))) {
							t = SaveAndAdd(result, granularitySpan, dao, t, oandaCandle);
						}
						FillNullCandles(result, granularitySpan, dao, t, entity.DateTime);
						t = entity.DateTime;
					}
					result.Add(entity.Candlestick);
					t = t.Add(granularitySpan);
				}
				if (t < End) {
					foreach (var oandaCandle in GetCandles(t, End)) {
						t = SaveAndAdd(result, granularitySpan, dao, t, oandaCandle);
					}
					FillNullCandles(result, granularitySpan, dao, t, End);
				}
				transaction.Commit();
			}
			return result;
		}

		private void FillNullCandles(List<Candlestick> result, TimeSpan granularitySpan, CandlestickDao dao, DateTime t, DateTime endTime) {
			while (t < endTime) {
				SaveNullCandle(dao, t);
				result.Add(new Candlestick() { DateTime = t, Open = 0 });
				t = t.Add(granularitySpan);
			}
		}

		private DateTime SaveAndAdd(List<Candlestick> result, TimeSpan granularitySpan, CandlestickDao dao, DateTime t, OandaCandle oandaCandle) {
			while (t < oandaCandle.DateTime) {
				SaveNullCandle(dao, t);
				result.Add(new Candlestick() { DateTime = t, Open = 0 });
				t = t.Add(granularitySpan);
			}
			SaveOandaCandle(dao, oandaCandle);
			result.Add(oandaCandle.Candlestick);
			t = t.Add(granularitySpan);
			return t;
		}

		private IEnumerable<OandaCandle> GetCandles(DateTime start, DateTime end) {
			TimeSpan granularitySpan = GetGranularitySpan();
			long count;
			do {
				count = (end - start).Ticks / granularitySpan.Ticks;
				DateTime e = start.AddTicks(granularitySpan.Ticks * Math.Min(count, 5000));
				foreach (var c in this.OandaAPI.GetCandles(start, e, Instrument, Granularity)) {
					yield return c;
				}
				start = e;
			} while (count > 5000);
		}

		public DateTime GetAlignTime(DateTime time) {
			if (Granularity[0] == 'S') {
				int s = int.Parse(Granularity.Substring(1));
				int aligned = (int)Math.Floor((double)time.Second / s) * s;
				return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, aligned, 0, time.Kind);

//				return time.AddMilliseconds(-time.Millisecond).AddSeconds(-time.Second + aligned);
			} else if (Granularity[0] == 'M') {
				int s = int.Parse(Granularity.Substring(1));
				int aligned = (int)Math.Floor((double)time.Minute / s) * s;
				return new DateTime(time.Year, time.Month, time.Day, time.Hour, aligned, 0, 0, time.Kind);
//				return time.AddMilliseconds(-time.Millisecond).AddSeconds(-time.Second).AddMinutes(-time.Minute + aligned);
			} else if (Granularity[0] == 'H' || Granularity == "D") {
				int s = Granularity == "D" ? 24 : int.Parse(Granularity.Substring(1));
				int aligned = (int)Math.Floor((double)time.Hour / s) * s;
				Console.WriteLine("align:" + aligned + " s:" + s + " hour:" + time.Hour);
				return new DateTime(time.Year, time.Month, time.Day, aligned, 0, 0, 0, time.Kind);
//				return time.AddMilliseconds(-time.Millisecond).AddSeconds(-time.Second).AddMinutes(-time.Minute).AddHours(-time.Hour + aligned);
			}


			throw new Exception("not supported granularity:" + Granularity);
		}

		public TimeSpan GetGranularitySpan() {
			if (Granularity[0] == 'S') {
				return new TimeSpan(0, 0, int.Parse(Granularity.Substring(1)));
			}
			if (Granularity[0] == 'M') {
				return new TimeSpan(0, int.Parse(Granularity.Substring(1)), 0);
			}
			if (Granularity[0] == 'H') {
				return new TimeSpan(int.Parse(Granularity.Substring(1)), 0, 0);
			}
			if (Granularity == "D") {
				return new TimeSpan(24, 0, 0);
			}
			throw new Exception("not supported granularity:" + Granularity);



		}
	}
}
