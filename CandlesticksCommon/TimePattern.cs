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

		public string Instrument = null;

		public Func<DateTime, float> GetPrice;

		public TimeOfDayPattern() {
			this.GetPrice = GetPriceImpl;
		}

		public bool IsMatch(out Signal signal, DateTime date) {
			signal = null;

			if(CheckStartTime.ToDateTime(date) > CheckEndTime.ToDateTime(date)) {
				throw new Exception("checkStartDateTime > checkEndDateTime");
			}
			if (CheckStartTime.ToDateTime(date) >= DateTime.Now) {
				return false;
			}
			signal = new Signal() {
				Pattern = this,
			};
			float startPrice = GetPrice(CheckStartTime.ToDateTime(date));
			signal.CheckStartPrice = startPrice;
			if (CheckEndTime.ToDateTime(date) >= DateTime.Now) {
				return false;
			}

			float endPrice = GetPrice(CheckEndTime.ToDateTime(date));
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

		public string GetTradeDescription(out bool isSuccessTrade, DateTime date) {
			DateTime priceGettableTime = DateTime.Now.AddSeconds(-5);
			float tradeStartPrice = float.NaN;
			float tradeEndPrice = float.NaN;
			var builder = new StringBuilder();
			builder.Append(this.TradeType);
			builder.Append("-");
			DateTime tradeStartDateTime = date.AddTicks(this.TradeStartTime.Ticks);
			if (tradeStartDateTime < priceGettableTime) {
				tradeStartPrice = GetPrice(tradeStartDateTime);
				builder.Append(tradeStartPrice.ToString(GetPriceFormat()));
			} else {
				builder.Append("???");
			}
			builder.Append("[" + this.TradeStartTime + "]");
			builder.Append("→");

			DateTime tradeEndDateTime = date.AddTicks(this.TradeEndTime.Ticks);
			if (tradeEndDateTime < priceGettableTime) {
				tradeEndPrice = GetPrice(tradeEndDateTime);
				builder.Append(GetPrice(tradeEndDateTime).ToString(GetPriceFormat()));
			} else {
				builder.Append("???");
			}
			builder.Append("[" + this.TradeEndTime + "]");

			if(tradeStartPrice != float.NaN && tradeEndPrice != float.NaN) {
				if(TradeType == TradeType.Ask) {
					isSuccessTrade = tradeStartPrice < tradeEndPrice;
				} else {
					isSuccessTrade = tradeStartPrice > tradeEndPrice;
				}
			} else {
				isSuccessTrade = false;
			}

			return builder.ToString();
		}

		public string GetPriceFormat() {
			string priceFormat = null;
			switch (Instrument) {
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

		private float GetPriceImpl(DateTime dateTime) {
			return new CandlesticksGetter() {
				Start = dateTime.AddMinutes(-10),
				Granularity = "M1",
				Count = 10,
				Instrument = this.Instrument
			}.Execute().Reverse().Where(c => !c.IsNull)
				.Select(c => c.Close).DefaultIfEmpty(float.NaN).First();
		}

		public class Signal {
			public TimeOfDayPattern Pattern;
			public float CheckStartPrice = float.NaN;
			public float CheckEndPrice = float.NaN;

			public string GetCheckResultDescription() {
				string priceFormat = Pattern.GetPriceFormat();

				return CheckStartPrice.ToString(priceFormat) + "[" + Pattern.CheckStartTime + "]→"
					+ CheckEndPrice.ToString(priceFormat) + "[" + Pattern.CheckEndTime + "](" +
					(CheckStartPrice < CheckEndPrice ? "+" : "") + (CheckEndPrice - CheckStartPrice).ToString(priceFormat) + ")";
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

			public DateTime ToDateTime(DateTime date) {
				return date.AddTicks(this.Ticks);
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
