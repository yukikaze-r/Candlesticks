﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {
	struct Candlestick {
		public DateTime DateTime;
		public float Open;
		public float Close;
		public float High;
		public float Low;
		public int Volume;

		public bool IsNull {
			get {
				return Open == 0f;
			}
		}

		public float PriceRange {
			get {
				return High - Low;
			}
		}


		public float BarRange {
			get {
				return Math.Abs(Open - Close);
			}
		}

		public bool IsUp(float d=0) {
			return Close > Open + d;
		}

		public bool IsPinbar(bool isUp, float minPositivePinRatio, float maxNegativePinRatio) {
			float bar = Math.Abs(this.Open - this.Close);
			float upPin = this.High - this.BarTop;
			float downPin = this.BarBottom - this.Low;
			if(isUp) {
				return upPin >= bar * minPositivePinRatio && downPin <= bar * maxNegativePinRatio;
			} else {
				return downPin >= bar * minPositivePinRatio && upPin <= bar * maxNegativePinRatio;
			}
		}

		public float BarTop {
			get {
				return Math.Max(this.Open, this.Close);
			}
		}

		public float BarBottom {
			get {
				return Math.Min(this.Open, this.Close);
			}
		}

		static public Candlestick Aggregate(IEnumerable<Candlestick> l) {
			Candlestick result = new Candlestick();
			result.High = float.MinValue;
			result.Low = float.MaxValue;
			bool isOpen = true;
			foreach(var c in l) {
				if(isOpen) {
					result.DateTime = c.DateTime;
					result.Open = c.Open;
					isOpen = false;
				}
				if (c.IsNull) {
					continue;
				}
				result.Close = c.Close;
				result.High = Math.Max(result.High, c.High);
				result.Low = Math.Min(result.Low, c.Low);
			}
			if(isOpen) {
				throw new Exception();
			}
			return result;
		}

		static public IEnumerable<Candlestick> Aggregate(IEnumerable<Candlestick> l, int n) {
			for (int s = 0; s < l.Count(); s += n) {
				yield return Aggregate(l.Skip(s).Take(n));
			}
		}

		public Candlestick Add(Candlestick candle) {
			return Candlestick.Aggregate(new Candlestick[] { this, candle });
		}
	}
}
