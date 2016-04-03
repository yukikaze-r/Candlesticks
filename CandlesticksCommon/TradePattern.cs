using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Candlesticks
{
    public class TradePattern {
		public TradeCondition TradeCondition;
		public TradeOrders TradeOrders;
	}

	public class TradeOrders : List<TimeTradeOrder> {
		public TradeOrders(params TimeTradeOrder [] orders) : base(orders) {
						
		}

		public string GetTradeDescription(TradeContext tradeContext) {
			return String.Join(" ", this.Select(order => order.GetTradeDescription(tradeContext)));
		}

		public void DoTrade(TradeContext tradeContext) {
			foreach (var order in this) {
				tradeContext.DoTrade(order.TradeType, order.Time);
			}
		}

		public bool IsInTradeTime {
			get {
				DateTime now = DateTime.Now;
				DateTime first = DateTime.Today.Add(this.Min(o => o.Time));
				DateTime last = DateTime.Today.Add(this.Max(o => o.Time));
				return first <= now && now <= last;
			}
		}

	}

	public class TradeContext {
		public string Instrument;
		public Func<DateTime, float> GetPrice;
		public DateTime Date;
		public List<Tuple<TradeType, float>> positions = new List<Tuple<TradeType, float>>();
		public float Profit = 0f;
		public bool IsValid = true;

		public TradeContext() {
			GetPrice = GetPriceImpl;
		}

		public void DoTrade(TradeType tradeType, TimeSpan time) {
			float currentPrice = GetPrice(Date.AddTicks(time.Ticks));
			if(float.IsNaN(currentPrice)) {
				IsValid = false;
				return;
			}
			if (tradeType != TradeType.Settle) {
				positions.Add(new Tuple<TradeType, float>(tradeType, currentPrice));
			} else {
				foreach(var position in positions) {
					if(position.Item1 == TradeType.Ask) {
						Profit += currentPrice - position.Item2;
					} else {
						Profit += position.Item2 - currentPrice;
					}
				}
				positions.Clear();
			}
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
	}

	public interface TradeCondition {
		bool IsMatch(out Signal signal, TradeContext tradeContext);
		string GetCheckDescription();
	}

	public class TradeConditionAnd : TradeCondition {
		public TradeCondition[] TradeConditions;

		public string GetCheckDescription() {
			return String.Join("かつ", TradeConditions.Select(s => "("+s.GetCheckDescription()+")"));
		}

		public bool IsMatch(out Signal signal, TradeContext tradeContext) {
			signal = new AndSignal();
			foreach(var c in TradeConditions) {
				Signal s = null;
				bool result = c.IsMatch(out s, tradeContext);
				if(s != null) {
					((AndSignal)signal).Add(s);
				}
				if(result == false) {
					return false;
				}
			}
			return true;
		}

		public class AndSignal : Signal {

			private List<Signal> signals = new List<Signal>();

			public void Add(Signal signal) {
				signals.Add(signal);
			}

			public bool IsCheckFinished {
				get {
					return signals.Where(s => s.IsCheckFinished == false).Count() == 0;
				}
			}

			public bool IsValid {
				get {
					return signals.Where(s => s.IsValid == false).Count() == 0;
				}
			}

			public string GetCheckResultDescription() {
				return String.Join(" ", signals.Select(s => s.GetCheckResultDescription()));
			}
		}

	}

	public class TimeTradeOrder {
		public TradeType TradeType;
		public TimeSpan Time;
		public string Instrument;

		public string GetTradeDescription(TradeContext tradeContext) {
			DateTime priceGettableTime = DateTime.Now.AddSeconds(-5);
			float tradeStartPrice = float.NaN;
			var builder = new StringBuilder();
			builder.Append(Time.ToString());
			builder.Append(" ");
			builder.Append(this.TradeType);
			builder.Append(" ");

			DateTime tradeStartDateTime = tradeContext.Date.AddTicks(this.Time.Ticks);
			if (tradeStartDateTime < priceGettableTime) {
				tradeStartPrice = tradeContext.GetPrice(tradeStartDateTime);
				builder.Append(PriceFormatter.Format(tradeStartPrice, Instrument));
			} else {
				builder.Append("???");
			}
			return builder.ToString();
		}
	}

	public interface Signal {
		string GetCheckResultDescription();
		bool IsCheckFinished {
			get;
		}
		bool IsValid {
			get;
		}
	}
}
