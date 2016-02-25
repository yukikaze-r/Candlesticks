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
		Bid, Ask
	}

	static class TradeTypeExt {
		public static TradeType Reverse(this TradeType t) {
			if(t == TradeType.Bid) {
				return TradeType.Ask;
			}
			else {
				return TradeType.Bid;
			}
		}
	}
}
