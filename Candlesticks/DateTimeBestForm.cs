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
	public partial class 日時ベスト正順通年5年10分足EUR_USD : Form {
		public 日時ベスト正順通年5年10分足EUR_USD() {
			InitializeComponent();
		}

		private void RunTask(object sender, Action<Report> action) {
			Report.RunTask(sender, action, dataGridView1, taskStatus);
		}

		private IEnumerable<Candlestick> GetM30Candles(TimeSpan span) {
			return Candlestick.Aggregate(GetM10Candles(span), 3);
		}


		private IEnumerable<Candlestick> GetM10Candles(TimeSpan span, string instrument="USD_JPY") {
			using (DBUtils.OpenThreadConnection()) {
				return new CandlesticksGetter() {
					Start = DateTime.Today.AddTicks(-span.Ticks),
					End = DateTime.Today,
					Granularity = "M10",
					Instrument = instrument
				}.Execute();
			}
		}

		private static void ReportGroupByTradeStartTime(Report report, BestTradeTime bestTradeTime) {
			var dict = new Dictionary<int, List<Tuple<int[], int[]>>>();
			foreach (var t in bestTradeTime.Calculate()) {
				int tradeStartTime = t.Item1[2];
				if (!dict.ContainsKey(tradeStartTime)) {
					dict[tradeStartTime] = new List<Tuple<int[], int[]>>();
				}
				dict[tradeStartTime].Add(t);
			}
			var list = new List<Tuple<int[], int[]>>();
			foreach (var tuples in dict.Values) {
				list.Add(tuples.OrderByDescending(t => bestTradeTime.Comparator(t.Item2)).First());
			}
			foreach (var t in list.OrderByDescending(t => bestTradeTime.Comparator(t.Item2))) {
				foreach (var time in t.Item1) {
					report.Write(bestTradeTime.HMString(time));
				}
				foreach (var c in t.Item2) {
					report.Write(c);
				}
				report.WriteLine();
			}
		}


		private void 日時ベスト冬時間5年_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 6;
				report.IsForceOverride = true;
				report.Comment = "冬時間";
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓");
				var bestTradeTime = new BestTradeTime(GetM30Candles(new TimeSpan(365 * 5, 0, 0, 0))) {
					IsSummerTime = false };
				foreach (var t in bestTradeTime.Calculate(100)) {

					foreach (var time in t.Item1) {
						report.Write(bestTradeTime.HMString(time));
					}
					foreach (var c in t.Item2) {
						report.Write(c);
					}
					report.WriteLine();
				}
			});
		}

		private void 日時ベスト冬時間３ヶ月_Click(object sender, EventArgs e) {

			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = false;
				report.Comment = "冬時間";
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓");
				var bestTradeTime = new BestTradeTime(GetM30Candles(new TimeSpan(3 * 30, 0, 0, 0))) {
					IsSummerTime = false
				};
				foreach (var t in bestTradeTime.Calculate(100)) {

					foreach (var time in t.Item1) {
						report.Write(bestTradeTime.HMString(time));
					}
					foreach (var c in t.Item2) {
						report.Write(c);
					}
					report.WriteLine();
				}
			});
		}

		private void 日時ベスト夏時間5年_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 6;
				report.IsForceOverride = true;
				report.Comment = "冬時間";
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓");
				var bestTradeTime = new BestTradeTime(GetM30Candles(new TimeSpan(365 * 5, 0, 0, 0))) {
					IsSummerTime = true
				};
				foreach (var t in bestTradeTime.Calculate(100)) {

					foreach (var time in t.Item1) {
						report.Write(bestTradeTime.HMString(time));
					}
					foreach (var c in t.Item2) {
						report.Write(c);
					}
					report.WriteLine();
				}
			});

		}

		private void 日時ベスト通年５年１０分足_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓");
				var bestTradeTime = new BestTradeTime(GetM10Candles(new TimeSpan(365 * 5, 0, 0, 0))) {
					Granularity = new TimeSpan(0, 10, 0)
				};
				ReportGroupByTradeStartTime(report, bestTradeTime);
			});
		}

		private void 日時ベスト通年5年10分足12時_Click(object sender, EventArgs e) {

			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓");
				var bestTradeTime = new BestTradeTime(GetM10Candles(new TimeSpan(5 * 365, 0, 0, 0))) {
					Granularity = new TimeSpan(0, 10, 0),
					ShiftHour = 12,
				};
				ReportGroupByTradeStartTime(report, bestTradeTime);
			});
		}


		private void 日時ベスト通年5年10分足12時EUR_USD_Click(object sender, EventArgs e) {

			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓");
				var bestTradeTime = new BestTradeTime(GetM10Candles(new TimeSpan(5 * 365, 0, 0, 0),"EUR_USD")) {
					Granularity = new TimeSpan(0, 10, 0),
					ShiftHour = 12,
				};
				ReportGroupByTradeStartTime(report, bestTradeTime);
			});
		}

		private void 日時ベスト正順通年5年10分足_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓");
				var bestTradeTime = new BestTradeTime(GetM10Candles(new TimeSpan(365 * 5, 0, 0, 0))) {
					Granularity = new TimeSpan(0, 10, 0),
					Comparator = f => (int)(Math.Max((double)f[0] / (f[0] + f[1]), (double)f[3] / (f[2] + f[3])) * 10000)
				};
				ReportGroupByTradeStartTime(report, bestTradeTime);
			});
		}

		private void 日時ベスト正順通年5年10分足EURUSD_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓");
				var bestTradeTime = new BestTradeTime(GetM10Candles(new TimeSpan(365 * 5, 0, 0, 0),"EUR_USD")) {
					Granularity = new TimeSpan(0, 10, 0),
					Comparator = f => (int)(Math.Max((double)f[0] / (f[0] + f[1]), (double)f[3] / (f[2] + f[3])) * 10000)
				};
				ReportGroupByTradeStartTime(report, bestTradeTime);
			});

		}

		private void EURUSD1100_1140_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 2;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("!p1 && !p2","match","p1 && !p2", "match", "!p1 && p2", "match", "p1 && p2", "match", "total");

				var dateTimePrice = new Dictionary<DateTime, float>();
				var dateSet = new HashSet<DateTime>();
				foreach (var c in GetM10Candles(new TimeSpan(365 * 5, 0, 0, 0), "EUR_USD")) {
					if(c.IsNull) {
						continue;
					}
					dateTimePrice[c.DateTime] = c.Open;
					dateSet.Add(c.DateTime.Date);
				}

				Func<DateTime, float> getPrice = dateTime => dateTimePrice.ContainsKey(dateTime) ? dateTimePrice[dateTime] : float.NaN;

				var patterns = new List<TimeOfDayPattern>();
				patterns.Add(new TimeOfDayPattern() {
					Instrument = "EUR_USD",
					CheckStartTime = new TimeOfDayPattern.Time(8, 50),
					CheckEndTime = new TimeOfDayPattern.Time(11, 00),
					IsCheckUp = false,
					TradeStartTime = new TimeOfDayPattern.Time(11, 00),
					TradeEndTime = new TimeOfDayPattern.Time(11, 40),
					TradeType = TradeType.Ask,
					GetPrice = getPrice,
				});

				patterns.Add(new TimeOfDayPattern() {
					Instrument = "EUR_USD",
					CheckStartTime = new TimeOfDayPattern.Time(6, 50),
					CheckEndTime = new TimeOfDayPattern.Time(7, 50),
					IsCheckUp = true,
					TradeStartTime = new TimeOfDayPattern.Time(11, 00),
					TradeEndTime = new TimeOfDayPattern.Time(11, 30),
					TradeType = TradeType.Ask,
					GetPrice = getPrice,
				});

				int c1, c2, c3, c4, total;
				int r1, r2, r3, r4;
				r1 = r2 = r3 = r4 = c1 = c2 = c3 = c4 = total = 0;

				foreach (var date in dateSet) {
					var m = new List<bool>();
					bool isValid = true;
					foreach (var p in patterns) {
						TimeOfDayPattern.Signal signal;
						m.Add(p.IsMatch(out signal, date));
						if(signal == null || float.IsNaN(signal.CheckStartPrice) || float.IsNaN(signal.CheckEndPrice)) {
							isValid = false;
						}
					}
					if(!isValid) {
						continue;
					}
					bool isSuccess;
					patterns[0].GetTradeDescription(out isSuccess, date);

					total++;
					if(!m[0] && !m[1]) {
						c1++;
						r1 += isSuccess ? 1 : 0;
					}
					if (m[0] && !m[1]) {
						c2++;
						r2 += isSuccess ? 1 : 0;
					}
					if (!m[0] && m[1]) {
						c3++;
						r3 += isSuccess ? 1 : 0;
					}
					if (m[0] && m[1]) {
						c4++;
						r4 += isSuccess ? 1 : 0;
					}
				}
				report.WriteLine(r1, c1, r2, c2, r3, c3, r4, c4, total);
			});

		}

		private void EURUSD1100_1140_rev_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 2;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("!p1 && !p2", "match", "p1 && !p2", "match", "!p1 && p2", "match", "p1 && p2", "match", "total");

				var dateTimePrice = new Dictionary<DateTime, float>();
				var dateSet = new HashSet<DateTime>();
				foreach (var c in GetM10Candles(new TimeSpan(365 * 5, 0, 0, 0), "EUR_USD")) {
					if (c.IsNull) {
						continue;
					}
					dateTimePrice[c.DateTime] = c.Open;
					dateSet.Add(c.DateTime.Date);
				}

				Func<DateTime, float> getPrice = dateTime => dateTimePrice.ContainsKey(dateTime) ? dateTimePrice[dateTime] : float.NaN;

				var patterns = new List<TimeOfDayPattern>();
				patterns.Add(new TimeOfDayPattern() {
					Instrument = "EUR_USD",
					CheckStartTime = new TimeOfDayPattern.Time(8, 50),
					CheckEndTime = new TimeOfDayPattern.Time(11, 00),
					IsCheckUp = true,
					TradeStartTime = new TimeOfDayPattern.Time(11, 00),
					TradeEndTime = new TimeOfDayPattern.Time(11, 40),
					TradeType = TradeType.Bid,
					GetPrice = getPrice,
				});

				patterns.Add(new TimeOfDayPattern() {
					Instrument = "EUR_USD",
					CheckStartTime = new TimeOfDayPattern.Time(6, 50),
					CheckEndTime = new TimeOfDayPattern.Time(7, 50),
					IsCheckUp = false,
					TradeStartTime = new TimeOfDayPattern.Time(11, 00),
					TradeEndTime = new TimeOfDayPattern.Time(11, 30),
					TradeType = TradeType.Bid,
					GetPrice = getPrice,
				});

				int c1, c2, c3, c4, total;
				int r1, r2, r3, r4;
				r1 = r2 = r3 = r4 = c1 = c2 = c3 = c4 = total = 0;

				foreach (var date in dateSet) {
					var m = new List<bool>();
					bool isValid = true;
					foreach (var p in patterns) {
						TimeOfDayPattern.Signal signal;
						m.Add(p.IsMatch(out signal, date));
						if (signal == null || float.IsNaN(signal.CheckStartPrice) || float.IsNaN(signal.CheckEndPrice)) {
							isValid = false;
						}
					}
					if (!isValid) {
						continue;
					}
					bool isSuccess;
					patterns[0].GetTradeDescription(out isSuccess, date);

					total++;
					if (!m[0] && !m[1]) {
						c1++;
						r1 += isSuccess ? 1 : 0;
					}
					if (m[0] && !m[1]) {
						c2++;
						r2 += isSuccess ? 1 : 0;
					}
					if (!m[0] && m[1]) {
						c3++;
						r3 += isSuccess ? 1 : 0;
					}
					if (m[0] && m[1]) {
						c4++;
						r4 += isSuccess ? 1 : 0;
					}
				}
				report.WriteLine(r1, c1, r2, c2, r3, c3, r4, c4, total);
			});
		}
	}
}
