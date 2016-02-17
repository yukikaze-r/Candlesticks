using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {

	struct TradePosition {
		public int Time;
		public float Price;
		public TradeType TradeType;
		public float HighSettlementPrice;
		public float LowSettlementPrice;
	}

	enum TradeType {
		Sell, Buy
	}

	static class TradeTypeExt {
		public static TradeType Reverse(this TradeType t) {
			if(t == TradeType.Sell) {
				return TradeType.Buy;
			}
			else {
				return TradeType.Sell;
			}
		}
	}
}
