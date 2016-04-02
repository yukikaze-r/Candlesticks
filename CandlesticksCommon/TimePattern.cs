using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Candlesticks
{
    class TimeOfDayPattern : TradeCondition
    {
		public TimeSpan CheckStartTime;
		public TimeSpan CheckEndTime;
		public bool IsCheckUp;
		public string Instrument = null;

		public TimeOfDayPattern() {
		}

		public bool IsMatch(out Candlesticks.Signal signal, TradeContext tradeContext) {
			signal = null;
			DateTime date = tradeContext.Date;

			if(date.Add(CheckStartTime) > date.Add(CheckEndTime)) {
				throw new Exception("checkStartDateTime > checkEndDateTime");
			}
			if (date.Add(CheckStartTime) >= DateTime.Now) {
				return false;
			}
			signal = new Signal() {
				Pattern = this,
			};
			float startPrice = tradeContext.GetPrice(date.Add(CheckStartTime));
			((Signal)signal).CheckStartPrice = startPrice;
			if (date.Add(CheckEndTime) >= DateTime.Now) {
				return false;
			}

			float endPrice = tradeContext.GetPrice(date.Add(CheckEndTime));
			((Signal)signal).CheckEndPrice = endPrice;

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
		
		

		public class Signal : Candlesticks.Signal {
			public TimeOfDayPattern Pattern;
			public float CheckStartPrice = float.NaN;
			public float CheckEndPrice = float.NaN;

			public string GetCheckResultDescription() {
				string priceFormat = PriceFormatter.GetPriceFormat(Pattern.Instrument);

				return CheckStartPrice.ToString(priceFormat) + "[" + Pattern.CheckStartTime + "]→"
					+ CheckEndPrice.ToString(priceFormat) + "[" + Pattern.CheckEndTime + "](" +
					(CheckStartPrice < CheckEndPrice ? "+" : "") + (CheckEndPrice - CheckStartPrice).ToString(priceFormat) + ")";
			}

			public bool IsValid {
				get {
					return !float.IsNaN(CheckStartPrice) && !float.IsNaN(CheckEndPrice);
				}
			}
			

			public bool IsCheckFinished {
				get {
					return DateTime.Today.Add(Pattern.CheckEndTime) <= DateTime.Now;
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
