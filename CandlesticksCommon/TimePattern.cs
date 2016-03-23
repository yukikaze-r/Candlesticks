using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Candlesticks
{
    class TimeOfDayPattern
    {
		public Time CheckStartTime;
		public Time CheckEndTime;
		public bool IsCheckUp;

		public Time TradeStartTime;
		public Time TradeEndTime;
		public TradeType TradeType;

		public int TotalVerification;
		public int MatchVerification;

		public string Instrument;

		public bool IsMatch(out Signal signal) {
			signal = null;

			if(CheckStartTime.Todays > CheckEndTime.Todays) {
				throw new Exception("checkStartDateTime > checkEndDateTime");
			}
			if (CheckStartTime.Todays >= DateTime.Now) {
				return false;
			}
			signal = new Signal() {
				Pattern = this,
			};
			float startPrice = GetPrice(CheckStartTime.Todays);
			signal.CheckStartPrice = startPrice;
			if (CheckEndTime.Todays >= DateTime.Now) {
				return false;
			}

			float endPrice = GetPrice(CheckEndTime.Todays);
			signal.CheckEndPrice = endPrice;

			if (this.IsCheckUp) {
				return endPrice > startPrice;
			} else {
				return endPrice < startPrice;
			}
		}

		public string GetCheckDescription() {
			return "["+this.Instrument + "] " + CheckStartTime + "～" + CheckEndTime + 
				"の間に価格が" + (IsCheckUp ? "上がっていたら" : "下がっていたら");
		}

		public string GetTradeDescription() {
			DateTime priceGettableTime = DateTime.Now.AddSeconds(-5);

			var builder = new StringBuilder();
			builder.Append(this.TradeType);
			builder.Append("-");
			DateTime tradeStartDateTime = DateTime.Today.AddTicks(this.TradeStartTime.Ticks);
			if (tradeStartDateTime < priceGettableTime) {
				builder.Append(GetPrice(tradeStartDateTime).ToString("F3"));
			} else {
				builder.Append("???");
			}
			builder.Append("[" + this.TradeStartTime + "]");
			builder.Append("→");

			DateTime tradeEndDateTime = DateTime.Today.AddTicks(this.TradeEndTime.Ticks);
			if (tradeEndDateTime < priceGettableTime) {
				builder.Append(GetPrice(tradeEndDateTime).ToString("F3"));
			} else {
				builder.Append("???");
			}
			builder.Append("[" + this.TradeEndTime + "]");

			return builder.ToString();
		}

		public class Signal {
			public TimeOfDayPattern Pattern;
			public float CheckStartPrice = float.NaN;
			public float CheckEndPrice = float.NaN;

			public string GetCheckResultDescription() {
				string priceFormat = GetPriceFormat();

				return CheckStartPrice.ToString(priceFormat) + "[" + Pattern.CheckStartTime + "]→"
					+ CheckEndPrice.ToString(priceFormat) + "[" + Pattern.CheckEndTime + "](" +
					(CheckStartPrice < CheckEndPrice ? "+" : "") + (CheckEndPrice - CheckStartPrice).ToString(priceFormat) + ")";
			}

			private string GetPriceFormat() {
				string priceFormat = null;
				switch (Pattern.Instrument) {
					case "USD_JPY":
						priceFormat = "F3";
						break;
					case "EUR_USD":
						priceFormat = "F5";
						break;
					default:
						throw new Exception();
				}

				return priceFormat;
			}

			public bool IsInTradeTime {
				get {
					DateTime now = DateTime.Now;
					return Pattern.TradeStartTime.Todays <= now && now <= Pattern.TradeEndTime.Todays;
				}
			}

			public bool IsCheckFinished {
				get {
					return Pattern.CheckEndTime.Todays <= DateTime.Now;
				}
			}

		}

		private float GetPrice(DateTime dateTime) {
			return new CandlesticksGetter() {
				Start = dateTime.AddMinutes(-10),
				Granularity = "M1",
				Count = 10,
				Instrument = this.Instrument
			}.Execute().Reverse().Where(c => !c.IsNull)
				.Select(c => c.Close).DefaultIfEmpty(float.NaN).First();
		}

		public struct Time {
			int Hour;
			int Minutes;

			public Time(TimeSpan timeSpan) {
				this.Hour = timeSpan.Hours;
				this.Minutes = timeSpan.Minutes;
			}

			public Time(int hour, int minute) {
				this.Hour = hour;
				this.Minutes = minute;
			}

			public long Ticks {
				get {
					return ((Hour * 60L) + Minutes) * 60L * 10000000L;
				}
			}

			public DateTime Todays {
				get {
					return DateTime.Today.AddTicks(this.Ticks);
				}
			}

			public override string ToString() {
				return Hour + ":" + Minutes.ToString("D2");
			}

			public TimeSpan TimeSpan {
				get {
					return new TimeSpan(Hour, Minutes, 0);
				}
			}
		}

	}
}
