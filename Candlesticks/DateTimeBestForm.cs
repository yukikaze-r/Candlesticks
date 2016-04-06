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


		private IEnumerable<Candlestick> GetM10Candles(TimeSpan span, string instrument = "USD_JPY") {
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


		private void ReportMultiCondition(Report report, TradeOrders tradeOrders, TradeCondition[] conditions, IEnumerable<Candlestick> candles) {
			report.SetHeader("condition","prifit rate","profit count","total match");

			var dateTimePrice = new Dictionary<DateTime, float>();
			var dateSet = new HashSet<DateTime>();
			foreach (var c in candles) {
				if (c.IsNull) {
					continue;
				}
				dateTimePrice[c.DateTime] = c.Open;
				dateSet.Add(c.DateTime.Date);
			}
			Func<DateTime, float> getPrice = dateTime => 
				dateTimePrice.ContainsKey(dateTime) ? dateTimePrice[dateTime] : float.NaN;

			int c1, c2, c3, c4, total;
			int r1, r2, r3, r4;
			r1 = r2 = r3 = r4 = c1 = c2 = c3 = c4 = total = 0;

			foreach (var date in dateSet) {
				var m = new List<bool>();
				bool isValid = true;
				foreach (var condition in conditions) {
					TradeContext c = new TradeContext() {
						Instrument = tradeOrders[0].Instrument,
						Date = date,
						GetPrice = getPrice,
					};
					Signal signal;
					m.Add(condition.IsMatch(out signal, c));
					if (signal == null || signal.IsValid == false) {
						isValid = false;
					}
				}
				if (!isValid) {
					continue;
				}
				TradeContext context = new TradeContext() {
					Instrument = tradeOrders[0].Instrument,
					Date = date,
					GetPrice = getPrice,
				};
				tradeOrders.DoTrade(context);
				if(context.IsValid==false) {
					continue;
				}
				bool isSuccess = context.Profit > 0f;

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
			report.WriteLine("!p1 && !p2", (float)r1 / c1, r1, c1);
			report.WriteLine("p1 && !p2", (float)r2 / c2, r2, c2);
			report.WriteLine("!p1 && p2", (float)r3 / c3, r3, c3);
			report.WriteLine("p1 && p2", (float)r4 / c4, r4, c4);
			report.WriteLine("total", (float)(r1 + r2 + r3 + r4) / (c1 + c2 + c3 + c4), r1 + r2 + r3 + r4, c1 + c2 + c3 + c4);
		}

		private static bool IsIntersect(int[] range1, int [] range2) {
			if (range1[0] == range2[0] && range1[1] == range2[1]) {
				return true;
			}
			if (range2[0] < range1[1] && range1[1] < range2[1]) {
				return true;
			}
			if (range1[0] < range2[1] && range2[1] < range1[1]) {
				return true;
			}
			if (range2[0] < range1[0] && range1[0] < range2[1]) {
				return true;
			}
			if (range1[0] < range2[0] && range2[0] < range1[1]) {
				return true;
			}
			return false;
		}

		private static void ReportGroupByTradeStartAndEndTime(Report report, BestTradeTime bestTradeTime) {
			var dict = new Dictionary<int[], List<Tuple<int[], int[]>>>();
			foreach (var t in bestTradeTime.Calculate()) {
				if(bestTradeTime.Comparator(t.Item2) < 5500) {
					continue;
				}
				int tradeStartTime = t.Item1[2];
				int tradeEndTime = t.Item1[3];
				int[] key = new int[] { tradeStartTime, tradeEndTime };
				if (!dict.ContainsKey(key)) {
					dict[key] = new List<Tuple<int[], int[]>>();
				}
				dict[key].Add(t);
			}
			var list = new List<Tuple<int[], int[]>>();
			foreach (var tuples in dict.Values) {
				int[] pre = null;
				foreach (var t in tuples.OrderByDescending(t => bestTradeTime.Comparator(t.Item2))) {
					if(pre != null) {
						if(IsIntersect(pre,t.Item1)) {
							continue;
						}
					}
					pre = t.Item1;
					list.Add(t);
				}
			}

			foreach (var t in list.OrderByDescending(t => bestTradeTime.Comparator(t.Item2)).Take(10000)) {
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
				var bestTradeTime = new BestTradeTime(GetM10Candles(new TimeSpan(5 * 365, 0, 0, 0), "EUR_USD")) {
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
				var bestTradeTime = new BestTradeTime(GetM10Candles(new TimeSpan(365 * 5, 0, 0, 0), "EUR_USD")) {
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

				var tradeOrders = new TradeOrders(
					new TimeTradeOrder() {
						Instrument = "EUR_USD",
						TradeType = TradeType.Ask,
						Time = new TimeSpan(11, 00, 0),
					},
					new TimeTradeOrder() {
						Instrument = "EUR_USD",
						TradeType = TradeType.Settle,
						Time = new TimeSpan(11, 40, 0),
					}
				);

				TradeCondition[] conditions = new TimeOfDayPattern[] {
					new TimeOfDayPattern() {
						Instrument = "EUR_USD",
						CheckStartTime = new TimeSpan(8, 50, 0),
						CheckEndTime = new TimeSpan(11, 00, 0),
						IsCheckUp = false,
					},
					new TimeOfDayPattern() {
						Instrument = "EUR_USD",
						CheckStartTime = new TimeSpan(6, 50, 0),
						CheckEndTime = new TimeSpan(7, 50, 0),
						IsCheckUp = true,
					},
				};

				ReportMultiCondition(report, tradeOrders, conditions, GetM10Candles(new TimeSpan(365 * 5, 0, 0, 0), "EUR_USD"));
			});

		}

		private void EURUSD1100_1140_rev_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 2;
				report.IsForceOverride = true;
				report.Comment = "";

				var tradeOrders = new TradeOrders(
					new TimeTradeOrder() {
						Instrument = "EUR_USD",
						TradeType = TradeType.Bid,
						Time = new TimeSpan(11, 00, 0),
					},
					new TimeTradeOrder() {
						Instrument = "EUR_USD",
						TradeType = TradeType.Settle,
						Time = new TimeSpan(11, 40, 0),
					}
				);

				TradeCondition[] conditions = new TimeOfDayPattern[] {
					new TimeOfDayPattern() {
						Instrument = "EUR_USD",
						CheckStartTime = new TimeSpan(8, 50, 0),
						CheckEndTime = new TimeSpan(11, 00, 0),
						IsCheckUp = true,
					},
					new TimeOfDayPattern() {
						Instrument = "EUR_USD",
						CheckStartTime = new TimeSpan(6, 50, 0),
						CheckEndTime = new TimeSpan(7, 50, 0),
						IsCheckUp = false,
					},
				};

				ReportMultiCondition(report, tradeOrders, conditions, GetM10Candles(new TimeSpan(365 * 5, 0, 0, 0), "EUR_USD"));
			});
		}

		private void USDJPY0550_0710_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.Comment = "";

				var tradeOrders = new TradeOrders(
					new TimeTradeOrder() {
						Instrument = "USD_JPY",
						TradeType = TradeType.Ask,
						Time = new TimeSpan(5, 50, 0),
					},
					new TimeTradeOrder() {
						Instrument = "USD_JPY",
						TradeType = TradeType.Settle,
						Time = new TimeSpan(7, 10, 0),
					}
				);

				TradeCondition[] conditions = new TimeOfDayPattern[] {
					new TimeOfDayPattern() {
						Instrument = "USD_JPY",
						CheckStartTime = new TimeSpan(0, 30, 0),
						CheckEndTime = new TimeSpan(4, 50, 0),
						IsCheckUp = false,
					},
					new TimeOfDayPattern() {
						Instrument = "USD_JPY",
						CheckStartTime = new TimeSpan(0, 20, 0),
						CheckEndTime = new TimeSpan(5, 40, 0),
						IsCheckUp = false,
					},
				};

				ReportMultiCondition(report, tradeOrders, conditions, GetM10Candles(new TimeSpan(365 * 5, 0, 0, 0), "USD_JPY"));
			});
		}

		private static float GetDoubleCheckProbability(int [] s) {
			float result = 0;
			for (int i = 0; i < 8; i += 2) {
				float p0 = (float)s[i] / (s[i] + s[i+1]);
				float p1 = 1 - p0;
				result = Math.Max(result, p0);
				result = Math.Max(result, p1);
			}
			return result;
		}

		private void 日時ベスト通年複合5年10分足_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 3;
				report.IsForceOverride = true;
				report.Comment = "USDJPY_0450_0710_夏時間";
				report.SetHeader("start", "end", "start", "end", "↑↑↑", "↑↑↓", "↑↓↑", "↑↓↓", "↓↑↑", "↓↑↓", "↓↓↑", "↓↓↓","rate");
				var bestTradeTime = new BestTradeTime(GetM10Candles(new TimeSpan(365 * 5, 0, 0, 0))) {
					Granularity = new TimeSpan(0, 10, 0),
					IsSummerTime = true
				};
				List<Tuple<int[], int[], float>> list = new List<Tuple<int[], int[], float>>();
				foreach(var t in bestTradeTime.CalculateDoubleCheckRange(new TimeSpan(4, 50, 0), new TimeSpan(7, 10, 0))) {
					var p = GetDoubleCheckProbability(t.Item2);
					if(p < 0.55f) {
						continue;
					}
					list.Add(new Tuple<int[],int[],float>(t.Item1, t.Item2, p));
				}
				foreach(var t in list.OrderByDescending(t => t.Item3).Take(1000)) {
					foreach(var checkTime in t.Item1) {
						report.Write(bestTradeTime.HMString(checkTime));
					}
					foreach (var count in t.Item2) {
						report.Write(count);
					}
					report.WriteLine(t.Item3);
				}
			});
		}
	}
}
