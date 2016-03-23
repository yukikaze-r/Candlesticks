using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Candlesticks {
	public partial class OrderBookLabForm : Form {
		private static float ORDERBOOK_GRANULARITY = 0.05f;

		public OrderBookLabForm() {
			InitializeComponent();
		}

		private void RunTask(object sender, Action<Report> action) {
			Report.RunTask(sender, action, dataGridView1, label1);
		}

		private IEnumerable<OrderBookDao.Entity> GetOrderBooks() {
			foreach(var orderBook in new OrderBookDao(DBUtils.GetConnection()).GetAllOrderByDateTimeDescendant().Reverse().ToList()) {
				if(!new CandlesticksGetter() {
					Granularity = "M10",
					Start = orderBook.NormalizedDateTime.AddMinutes(-10),
					Count = 1
				}.Execute().First().IsNull) {
					yield return orderBook;
				}
			}
		}

		private void 近傍ポジション変化_Click(object sender, EventArgs e) {
			RunTask(sender, report => {
				report.Version = 3;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("date_time","pre_pl", "cur_pl", "pre_ps", "cur_ps", "low","high","close");
				using (DBUtils.OpenThreadConnection()) {
					OrderBookDao.Entity preOrderBook = null;
					foreach (var orderBook in GetOrderBooks()) {
						if(preOrderBook != null) {
							var candle = Candlestick.Aggregate(new CandlesticksGetter() {
								Granularity = "M10",
								Start = orderBook.NormalizedDateTime.AddMinutes(-20),
								Count = 2
							}.Execute());
							if (orderBook.NormalizedDateTime == new DateTime(2016, 2, 22, 10, 40, 0, DateTimeKind.Local)) {
								int a = 1;
							}
							float prePl = 0, prePs = 0;
							float curPl = 0, curPs = 0;
							foreach(var pp in GetRangePricePoint(orderBook, candle.Low, candle.High)) {
								curPl += pp.Pl;
								curPs += pp.Ps;
							}
							foreach (var pp in GetRangePricePoint(preOrderBook, candle.Low, candle.High)) {
								prePl += pp.Pl;
								prePs += pp.Ps;
							}
						report.WriteLine(orderBook.DateTime, prePl, curPl, prePs, curPs, candle.Low, candle.High, candle.Close);
						}
						preOrderBook = orderBook;
					}
				}
			});
		}

		private static OrderBookPricePointDao.Entity GetRatePricePoint(OrderBookDao.Entity orderBook, float rate) {
			float min = float.MaxValue;
			OrderBookPricePointDao.Entity closedPricePoint = null;
			foreach (var pricePoint in orderBook.GetPricePointsOrderByPrice()) {
				float d = Math.Abs(rate - pricePoint.Price);
				if (d < min) {
					min = d;
					closedPricePoint = pricePoint;
				}
			}
			return closedPricePoint;
		}

		private static IEnumerable<OrderBookPricePointDao.Entity> GetRangePricePoint(OrderBookDao.Entity orderBook, float min, float max) {
			float start = GetRoundPrice(min);
			float end = GetRoundPrice(max);
			foreach (var pricePoint in orderBook.GetPricePointsOrderByPrice()) {
				if(start - 0.001f <= pricePoint.Price && pricePoint.Price <= end + 0.001f) {
					yield return pricePoint;
				}
			}
		}

		private static float GetRoundPrice(float price) {
			int n = (int)((price + ORDERBOOK_GRANULARITY / 2) / ORDERBOOK_GRANULARITY);
			return ORDERBOOK_GRANULARITY * n;
		}
	}
}
