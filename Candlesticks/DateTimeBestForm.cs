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
	public partial class DateTimeBestForm : Form {
		private Report report;

		public DateTimeBestForm() {
			InitializeComponent();
		}

		private void RunTask(object sender, Action<Report> action) {
			Task.Run(() => {
				using (this.report = new Report() {
					Name = ((Control)sender).Text,
					BasePath = Setting.Instance.DataFilePath,
					DataGridView = dataGridView1,
					StatusControl = taskStatus,
				}) {
					try {
						action(this.report);
					} catch (ReportExistedException) {
					}
				}
			});
		}

		private IEnumerable<Candlestick> GetM30Candles(TimeSpan span) {
			return Candlestick.Aggregate(GetM10Candles(span), 3);
		}


		private IEnumerable<Candlestick> GetM10Candles(TimeSpan span) {
			using (DBUtils.OpenThreadConnection()) {
				return new CandlesticksGetter() {
					Start = DateTime.Today.AddTicks(-span.Ticks),
					End = DateTime.Today,
					Granularity = "M10"
				}.Execute();
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
	}
}
