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
	public partial class ChartLabForm : Form {
		public ChartLabForm() {
			InitializeComponent();
		}

		private void RunTask(object sender, Action<Report> action) {
			Report.RunTask(sender, action, dataGridView1, label1);
		}

		private void 急変戻り戻り_Click(object sender, EventArgs e) {
			RunTask(sender, (report) => {
				using(DBUtils.OpenThreadConnection()) {
					report.Version = 1;
					report.IsForceOverride = true;
					report.Comment = "";
					report.SetHeader("t", "upc1", "upc2", "uptotal", "downc1", "downc2", "downtotal");

					var candles = new CandlesticksGetter() {
						Granularity = "M1",
						Start = DateTime.Today.AddYears(-5),
						End = DateTime.Today
					}.Execute().ToList();

					List<int> indexList = GetOverMoveCandles(candles).ToList();

					foreach (var t in Fibs().Take(20)) {

						int upc1 = 0;
						int upc2 = 0;
						int uptotal = 0;
						int downc1 = 0;
						int downc2 = 0;
						int downtotal = 0;
						foreach (var i in indexList) {
							if (i + t + 15 >= candles.Count()) {
								break;
							}
							var candleOrg = candles[i];
							var candleArter15 = candles[i + t];
							var candleArter30 = candles[i + t + 15];
							if(candleOrg.IsNull || candleArter15.IsNull || candleArter30.IsNull) {
								continue;
							}
							if (candleOrg.IsUp()) {
								if (candleOrg.Close > candleArter15.Close) {
									upc1++;
									if (candleArter15.Close < candleArter30.Close) {
										upc2++;
									}
								}
								uptotal++;
							} else {
								if (candleOrg.Close < candleArter15.Close) {
									downc1++;
									if (candleArter15.Close > candleArter30.Close) {
										downc2++;
									}
								}
								downtotal++;
							}
						}
						report.WriteLine(t, upc1, upc2, uptotal, downc1, downc2, downtotal);

					}
				}
			});


		}

		private IEnumerable<int> GetOverMoveCandles(List<Candlestick> candles) {
			int index = 0;
			while (true) {
				index++;
				index = Find(index, candles, c => c.PriceRange > 0.20);
				if (index == -1) {
					break;
				}
				if (candles.Skip(index - 5).Take(5).Where(c => c.BarRange > 0.10).Count() >= 1) {
					continue;
				}
				yield return index;
			}
		}

		private IEnumerable<int> Fibs() {
			int x = 0;
			int y = 1;

			while(true) {
				int next = x + y;
				yield return next;
				x = y;
				y = next;
			}
		}

		private int Find(int start, List<Candlestick> candles, Func<Candlestick,bool> eval) {
			for(int i= start; i<candles.Count(); i++) {
				if(eval(candles[i])) {
					return i;
				}
			}
			return -1;
		}

		private void 急変日時_Click(object sender, EventArgs e) {
			RunTask(sender, (report) => {
				using (DBUtils.OpenThreadConnection()) {
					report.Version = 1;
					report.IsForceOverride = true;
					report.Comment = "";
					report.SetHeader("datetime","isUp","price", "2584date","2584price");

					var candles = new CandlesticksGetter() {
						Granularity = "M1",
						Start = DateTime.Today.AddYears(-5),
						End = DateTime.Today
					}.Execute().ToList();
					foreach(int i in GetOverMoveCandles(candles)) {
						if(i+2584>=candles.Count()) {
							break;
						}
						report.WriteLine(
							candles[i].DateTime.ToString("yyyy年MM月dd日(dddd) h:mm"), 
							candles[i].IsUp(),
							candles[i].Close,
							candles[i+2584].DateTime.ToString("yyyy年MM月dd日(dddd) h:mm"),
							candles[i+2584].Close);
					}
				}
			});
		}
	}
}
