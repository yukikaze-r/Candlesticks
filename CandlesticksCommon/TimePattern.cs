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
			return CheckStartTime + "～" + CheckEndTime + 
				"の間に価格が" + (IsCheckUp ? "上がっていたら" : "下がっていたら");
		}

		public class Signal {
			public TimeOfDayPattern Pattern;
			public float CheckStartPrice = float.NaN;
			public float CheckEndPrice = float.NaN;

			public string GetCheckResultDescription() {
				return CheckStartPrice.ToString("F3") + "[" + Pattern.CheckStartTime + "]→"
					+ CheckEndPrice.ToString("F3") + "[" + Pattern.CheckEndTime + "](" +
					(CheckStartPrice < CheckEndPrice ? "+" : "") + (CheckEndPrice - CheckStartPrice).ToString("F3") + ")";
			}

			public bool IsInTradeTime {
				get {
					DateTime now = DateTime.Now;
					return Pattern.TradeStartTime.Todays <= now && now <= Pattern.TradeEndTime.Todays;
				}
			}

			public string GetTradeDescription() {
				var builder = new StringBuilder();
				builder.Append(Pattern.TradeType);
				builder.Append("-");
				DateTime tradeStartDateTime = DateTime.Today.AddTicks(Pattern.TradeStartTime.Ticks);
				if(tradeStartDateTime > DateTime.Now) {
					builder.Append(TimeOfDayPattern.GetPrice(tradeStartDateTime).ToString("F3"));
				} else {
					builder.Append("???");
				}
				builder.Append("[" + Pattern.TradeStartTime + "]");
				builder.Append("→");

				DateTime tradeEndDateTime = DateTime.Today.AddTicks(Pattern.TradeEndTime.Ticks);
				if (tradeEndDateTime > DateTime.Now) {
					builder.Append(TimeOfDayPattern.GetPrice(tradeEndDateTime).ToString("F3"));
				} else {
					builder.Append("???");
				}
				builder.Append("[" + Pattern.TradeEndTime + "]");

				return builder.ToString();
			}
		}

		private static float GetPrice(DateTime dateTime) {
			return new CandlesticksGetter() {
				Start = dateTime.AddMinutes(-1),
				Granularity = "M1",
				Count = 1
			}.Execute().First().Close;
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
