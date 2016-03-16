﻿using System;
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
					"WTICO_USD","US30_USD","JP225_USD","USB10Y_USD"
				};
					string[] effects = new string[] {
					"USD_JPY","EUR_USD"
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
				if (effectCandle.IsNull || effectCandle.DateTime < causeCandle.DateTime) {
					i++;
					continue;
				}
				if (causeCandle.IsNull || effectCandle.DateTime > causeCandle.DateTime) {
					j++;
					continue;
				}
				if (prevUpDown != -1) {
					int key = prevUpDown + (effectCandle.IsUp() ? 4 : 0);
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
	}
}
