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
	public partial class CauseAndEffectForm : Form {
		public CauseAndEffectForm() {
			InitializeComponent();
		}

		private void RunTask(object sender, Action<Report> action) {
			Report.RunTask(sender, action, dataGridView1, taskStatus);
		}

		private void button1_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				using(DBUtils.OpenThreadConnection()) {
					report.Version = 1;
					report.IsForceOverride = true;
					report.Comment = "";

					string[] causes = new string[] {
						"WTICO_USD","US30_USD","JP225_USD","USB10Y_USD","USD_JPY","EUR_USD"
					};
						string[] effects = new string[] {
						"WTICO_USD","US30_USD","JP225_USD","USB10Y_USD","USD_JPY","EUR_USD"
					};
					Tuple<string, TimeSpan>[] granularityTuples = new Tuple<string, TimeSpan>[] {
						new Tuple<string,TimeSpan>("D",new TimeSpan(365*5,0,0,0)),
						new Tuple<string,TimeSpan>("H1",new TimeSpan(365*5,0,0,0)),
						new Tuple<string,TimeSpan>("M10",new TimeSpan(365,0,0,0)),
						new Tuple<string,TimeSpan>("M1",new TimeSpan(365,0,0,0)),
					};

					report.SetHeader("effect", "cause", "granularity", "↑↑->↑", "↑↓->↑", "↓↑->↑", "↓↓->↑", "↑↑->↓", "↑↓->↓", "↓↑->↓", "↓↓->↓");
					foreach (var granularityTuple in granularityTuples) {
						foreach (string effect in effects) {
							foreach (string cause in causes) {
								if(effect == cause) {
									continue;
								}
								ReportCauseAndEffect(granularityTuple, effect, cause, report);
							}
						}
					}
				}
			});
		}

		private static void ReportCauseAndEffect(Tuple<string, TimeSpan> granularityTuple, string effect, string cause, Report report) {
			DateTime start = DateTime.Today.AddTicks(-granularityTuple.Item2.Ticks);
			DateTime end = DateTime.Today;
			var effectCandles = new CandlesticksGetter() {
				Instrument = effect,
				Granularity = granularityTuple.Item1,
				Start = start,
				End = end
			}.Execute().ToList();
			var causeCandles = new CandlesticksGetter() {
				Instrument = cause,
				Granularity = granularityTuple.Item1,
				Start = start,
				End = end
			}.Execute().ToList();
			int i = 0, j = 0;

			Dictionary<int, int> summary = new Dictionary<int, int>();
			int prevUpDown = -1;
			while (i < effectCandles.Count() && j < causeCandles.Count()) {
				var effectCandle = effectCandles[i];
				var causeCandle = causeCandles[j];
				if (effectCandle.IsNull || effectCandle.DateTime < causeCandle.DateTime || effectCandle.Close == effectCandle.Open) {
					i++;
					continue;
				}
				if (causeCandle.IsNull || effectCandle.DateTime > causeCandle.DateTime || causeCandle.Close == causeCandle.Open) {
					j++;
					continue;
				}
				if (prevUpDown != -1) {
					int key = prevUpDown + (effectCandle.IsUp() ? 0 : 4);
					if (summary.ContainsKey(key) == false) {
						summary[key] = 0;
					}
					summary[key]++;
				}
				prevUpDown = (effectCandle.IsUp() ? 0 : 2) + (causeCandle.IsUp() ? 0 : 1);
				i++;
				j++;
			}
			report.Write(effect, cause, granularityTuple.Item1);
			for (int updown=0; updown<8; updown++) {
				report.Write(summary.ContainsKey(updown) ? summary[updown] : 0);
			}
			report.WriteLine();
		}

		private class CauseContext {
			public Candlestick[] Candles;
			public bool IsUp;
			public int Index = 0;
			public Candlestick CurrentCandle {
				get {
					return Candles[Index];
				}
			}
			public bool IsMatch {
				get {
					return Candles[Index].IsUp() == IsUp;
				}
			}
		}

		private static void ReportEffectAndCauseCombination(Report report, List<CauseContext> causeContexts, bool effectIsUp) {
			int prev = -1;
			int matchCount = 0;
			int total = 0;
			while (causeContexts.Count(c => c.Index >= c.Candles.Count()) == 0) {
				bool indexIncrement = false;
				DateTime maxDateTime = causeContexts.Max(c => c.CurrentCandle.DateTime);
				foreach (var causeContext in causeContexts) {
					if (causeContext.CurrentCandle.IsNull || maxDateTime > causeContext.CurrentCandle.DateTime) {
						causeContext.Index++;
						indexIncrement = true;
					}
				}
				if(indexIncrement) {
					continue;
				}
				if (prev == 1) {
					if (causeContexts[0].CurrentCandle.IsUp() == effectIsUp) {
						matchCount++;
					}
					total++;
				}
				if (causeContexts.Count(c => !c.IsMatch) == 0) {
					prev = 1;
				} else {
					prev = 0;
				}

				foreach (var causeContext in causeContexts) {
					causeContext.Index++;
				}
			}
			report.WriteLine(matchCount, total);
			report.WriteLine();
		}

		private void 因果EUR_USD_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				using (DBUtils.OpenThreadConnection()) {
					report.Version = 1;
					report.IsForceOverride = true;
					report.Comment = "";
					report.SetHeader("↑↑↑↑->↓", "total");

					DateTime start = DateTime.Today.AddTicks(-new TimeSpan(365 * 5, 0, 0, 0).Ticks);
					DateTime end = DateTime.Today;

					var causeContexts = new List<CauseContext>();
					foreach (var cause in new string[] { "EUR_USD", "WTICO_USD", "US30_USD", "JP225_USD" }) {
						var causeCandles = new CandlesticksGetter() {
							Instrument = cause,
							Granularity = "D",
							Start = start,
							End = end
						}.Execute().ToArray();
						causeContexts.Add(new CauseContext() {
							Candles = causeCandles,
							IsUp = true
						});
					}
					ReportEffectAndCauseCombination(report, causeContexts, false);
				}
			});
		}


		private void 因果USD_JPY_Click(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				using (DBUtils.OpenThreadConnection()) {
					report.Version = 1;
					report.IsForceOverride = true;
					report.Comment = "";
					report.SetHeader("↑↓↓↓↓->↓", "total");

					DateTime start = DateTime.Today.AddTicks(-new TimeSpan(365, 0, 0, 0).Ticks);
					DateTime end = DateTime.Today;

					var causeContexts = new List<CauseContext>();
					var instruments = new string[] { "USD_JPY", "WTICO_USD", "US30_USD", "JP225_USD", "EUR_USD" };
					var isUps = new bool[] { true,false,false,false,false };
					foreach(var t in instruments.Zip(isUps,(i,isUp)=>new Tuple<string,bool>(i,isUp))) {
						var causeCandles = new CandlesticksGetter() {
							Instrument = t.Item1,
							Granularity = "M1",
							Start = start,
							End = end
						}.Execute().ToArray();
						causeContexts.Add(new CauseContext() {
							Candles = causeCandles,
							IsUp = t.Item2
						});
					}
					ReportEffectAndCauseCombination(report, causeContexts, false);
				}
			});
		}
	}
}
