using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Candlesticks
{
	delegate void PriceListener(DateTime dateTime, float bid, float ask);

	class PriceObserver
    {
		private static Dictionary<string, PriceObserver> instances = new Dictionary<string, PriceObserver>();

		public static PriceObserver Get(string instrument) {
			lock(typeof(PriceObserver)) {
				if(!instances.ContainsKey(instrument)) {
					instances[instrument] = new PriceObserver(instrument);
				}
				return instances[instrument];
			}
		}

		private PriceListener listeners = null;
		private OandaAPI oandaApi;
		private HttpClient client;
		private string instrument;

		public PriceObserver(string instrument) {
			this.instrument = instrument;
		}

		public void Observe(PriceListener listener) {
			lock(this) {
				if (listeners == null) {
					oandaApi = new OandaAPI();
					listeners = delegate { };
					client = oandaApi.GetPrices(ReceivePrice, instrument);
				}
				listeners += listener;
			}
		}

		private void ReceivePrice(DateTime dateTime, float bid, float ask) {
			lock(this) {
				listeners.Invoke(dateTime, bid, ask);
			}
		}

		public void UnOnserve(PriceListener listener) {
			lock (this) {
				listeners -= listener;
				if (listeners.GetInvocationList().Length == 1) {
					client.Dispose();
					oandaApi.Dispose();
					listeners = null;
				}
			}
		}
    }
}
