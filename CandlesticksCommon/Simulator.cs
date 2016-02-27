using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Candlesticks
{
    class Simulator : IEnumerable<Simulator.Current>
    {
		IEnumerable<Candlestick> candles;

		public float Money { get; set; }
		public List<Position> Positions { get; set; }

		public Simulator(IEnumerable<Candlestick> candles) {
			this.candles = candles;
			this.Positions = new List<Position>();
		}
		

		public IEnumerator<Current> GetEnumerator() {
			return new CurrentEnumrator(this,candles.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new CurrentEnumrator(this,candles.GetEnumerator());
		}

		public class CurrentEnumrator : IEnumerator<Current> {
			private Simulator simulator;
			private IEnumerator<Candlestick> enumerator;

			public CurrentEnumrator(Simulator simulator, IEnumerator<Candlestick> enumerator) {
				this.enumerator = enumerator;
				this.simulator = simulator;
			}

			public Current Current {
				get {
					return new Current(this,enumerator.Current);
				}
			}

			object IEnumerator.Current {
				get {
					return new Current(this, enumerator.Current);
				}
			}

			public void Dispose() {
				enumerator.Dispose();
			}

			public bool MoveNext() {
				return enumerator.MoveNext();
			}

			public void Reset() {
				enumerator.Reset();
			}

			public void Settle(Position position, int volume) {
				this.simulator.Money += position.Settle(enumerator.Current, volume);
				if(position.Volume == 0) {
					this.simulator.Positions.Remove(position);
				}
			}

			public Position Ask(Current current,int n) {
				Position result = new Position(current, TradeType.Ask, n);
				this.simulator.Positions.Add(result);
				return result;

			}

			public Position Bid(Current current,int n) {
				Position result = new Position(current, TradeType.Bid, n);
				this.simulator.Positions.Add(result);
				return result;

			}

		}

		public class Current {
			private CurrentEnumrator enumerator;
			private Candlestick candlestick;
			
			public Current(CurrentEnumrator enumerator, Candlestick candlestick) {
				this.enumerator = enumerator;
				this.candlestick = candlestick;
			}

			public Candlestick Candlestick {
				get {
					return candlestick;
				}
			}

			public Position Ask(int n) {
				return enumerator.Ask(this,n);
			}

			public Position Bid(int n) {
				return enumerator.Bid(this,n);
			}

			public void Settle(Position position, int volume) {
				enumerator.Settle(position,volume);
			}
		}

		public class Position {
			private Current current;
			private TradeType tradeType;
			private int volume;

			public Position(Current current, TradeType tradeType, int volume) {
				this.current = current;
				this.tradeType = tradeType;
				this.volume = volume;
			}

			public void Settle(int volume = 0) {
				current.Settle(this,volume == 0 ? this.volume : volume);
			}

			public float Settle(Candlestick candlestick, int volume) {
				if(volume > this.volume) {
					throw new Exception();
				}

				this.volume -= volume;
				float d = (candlestick.Close - current.Candlestick.Close) * volume;
				if(tradeType == TradeType.Ask) {
					return d;
				} else {
					return -d;
				}
			}

			public int Volume {
				get {
					return volume;
				}
			}
		}
    }
}
