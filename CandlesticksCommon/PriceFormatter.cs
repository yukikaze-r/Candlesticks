using System;
using System.Collections.Generic;
using System.Text;

namespace Candlesticks
{
    class PriceFormatter
    {
		public static string Format(float price, string instrument) {
			return price.ToString(GetPriceFormat(instrument));
		}


		public static string GetPriceFormat(string instrument) {
			string priceFormat = null;
			switch (instrument) {
				case "USD_JPY":
				case "EUR_JPY":
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
	}
}
