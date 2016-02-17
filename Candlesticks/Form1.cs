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
	public partial class Form1 : Form {

		private string REPORT_PATH = @"C:\Users\玲\OneDrive\ドキュメント\finance";
		private Report report;

		public Form1() {
			InitializeComponent();
		}

		private List<TradePosition> positions = new List<TradePosition>();

		private void button1_Click(object sender, EventArgs e) {
			int range = 8;
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\h1.csv"));
			for(int t = range; t < list.Count(); t++) {
//				DoSettlement(t, list[t].Close);
				float low = list.GetRange(t - range, range).Select(s => s.Low).Min();
				if(list[t].Close < low) {
					DoTrade(t, TradeType.Buy, list[t].Close);
					// sell list[t].Close
				}
				float high = list.GetRange(t - range, range).Select(s => s.High).Max();
				if (list[t].Close > high) {
					DoTrade(t, TradeType.Sell, list[t].Close);
				}
			}
		}

		private void DoSettlement(int t, float price) {
			bool f = false;
			float money = 0;
			foreach (var position in positions) {
				f = true;
				if (position.TradeType == TradeType.Buy) {
					money += price - position.Price;
				} else {
					money += position.Price - price;
				}
			}
			if (f) {
				positions.Clear();
				Console.Out.WriteLine(t + "," + money);
			}
		}

		private void DoTrade(int t, TradeType tradeType, float price) {
			bool f = false;
			float money = 0;
			int score = 0;
			foreach (var position in positions.Where(p => p.TradeType == tradeType.Reverse())) {
				if (position.TradeType == TradeType.Buy) {
					money += price - position.Price;
					score += price - position.Price > 0 ? 1 : -1;
				} else {
					money += position.Price - price;
					score += position.Price - price - position.Price > 0 ? 1 : -1;
				}
				f = true;
			}
			if(f) {
				positions.Clear();
			} else {
				if (positions.Count() == 0) {
					positions.Add(new TradePosition() {
						Price = price,
						TradeType = tradeType,
					});
				}
			}
			if(f) {
				Console.Out.WriteLine(t+","+money+","+ score);
			}
		}

		private void button2_Click(object sender, EventArgs e) {
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\h1.csv"));
			Dictionary<int, int> upD = new Dictionary<int, int>();
			Dictionary<int, int> downD = new Dictionary<int, int>();
			int c = 0;
			bool isUp = false;
			foreach (var stick in list) {
				if(stick.Close > stick.Open) {
					if(isUp) {
						c++;
					} else {
						if (!downD.ContainsKey(c+1)) {
							downD[c + 1] = 0;
						}
						downD[c + 1]++;
						c = 0;
						isUp = true;
					}
				} else {
					if (!isUp) {
						c++;
					} else {
						if (!upD.ContainsKey(c + 1)) {
							upD[c + 1] = 0;
						}
						upD[c + 1]++;
						c = 0;
						isUp = false;
					}
				}
			}

			foreach(var k in upD.Keys.OrderBy(k=>k)) {
				Console.Out.WriteLine(k+","+upD[k]);
			}
			foreach (var k in downD.Keys.OrderBy(k => k)) {
				Console.Out.WriteLine(k + "," + downD[k]);
			}

		}

		private void button3_Click(object sender, EventArgs e) {
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\h1.csv"));
			Dictionary<string, List<Candlestick>> dayOftheWeekTimeDict = new Dictionary<string, List<Candlestick>>();
			foreach(var stick in list) {
				string key = stick.Time.DayOfWeek + " " + stick.Time.Hour;
				if(dayOftheWeekTimeDict.ContainsKey(key) == false) {
					dayOftheWeekTimeDict[key] = new List<Candlestick>();
				}
				dayOftheWeekTimeDict[key].Add(stick);
			}
			foreach (var dayOfWeek in Enum.GetNames(typeof(DayOfWeek))) {
				for(int h=0; h<24; h++) {
					List<Candlestick> l = null;
					dayOftheWeekTimeDict.TryGetValue(dayOfWeek + " " + h, out l);
					if(l != null) {
						Console.Out.WriteLine(dayOfWeek+","+h+","+l.Where(s => s.IsUp()).Count() + "," + l.Count());
					}
				}
			}
		}

		private void button4_Click(object sender, EventArgs e) {
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\h1.csv"));
			List<Dictionary<string, Candlestick>> dayOfWeekTable = new List<Dictionary<string, Candlestick>>();
			Dictionary<string, Candlestick> dict = new Dictionary<string, Candlestick>();
			DayOfWeek oldDayOfWeek = DayOfWeek.Friday;
			foreach (var stick in list) {
				if(stick.Time.DayOfWeek < oldDayOfWeek) {
					dict = new Dictionary<string, Candlestick>();
					dayOfWeekTable.Add(dict);
				}
				string key = stick.Time.DayOfWeek + " " + stick.Time.Hour;
				dict.Add(key, stick);
				oldDayOfWeek = stick.Time.DayOfWeek;
			}

			foreach(var d in dayOfWeekTable) {
				try {
					float d1 = d[DayOfWeek.Tuesday + " 17"].Close - d[DayOfWeek.Monday + " 9"].Open;
					float d2 = d[DayOfWeek.Friday + " 17"].Close - d[DayOfWeek.Thursday + " 9"].Open;
					Console.Out.WriteLine(d1 + "," + d2);
				} catch(Exception) {
				}
			}

		}

		private void button5_Click(object sender, EventArgs e) {

			int range = 10;
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\h1.csv"));
			for (int t = range; t < list.Count(); t++) {
				float low = list.GetRange(t - range, range).Select(s => s.Low).Min();
				if (list[t].Close < low) {
					int r = Math.Min(range, list.Count() - t);
					float h = list.GetRange(t, range).Select(s => s.High).Max();
					float l = list.GetRange(t, range).Select(s => s.Low).Min();
					Console.Out.WriteLine(list[t].Close+","+h + ","+l);
				}
/*				float high = list.GetRange(t - range, range).Select(s => s.High).Max();
				if (list[t].Close > high) {
					DoTrade(t, TradeType.Buy, list[t].Close);
				}*/
			}
		}

		private void button6_Click(object sender, EventArgs e) {
			int range = 10;
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\h1.csv"));
			List<TradePosition> positions = new List<TradePosition>();
			for (int t = range; t < list.Count(); t++) {
				float CurrentPrice = list[t].Close;
				List<TradePosition> done = new List<TradePosition>();
				foreach(TradePosition p in positions) {
					if(list[t].Low <= p.LowSettlementPrice) {
						if (list[t].High >= p.HighSettlementPrice) {
							Console.WriteLine((t - p.Time) + ",0");
							done.Add(p);
						} else {
							Console.WriteLine((t - p.Time) + "," + (p.LowSettlementPrice - p.Price));
							done.Add(p);
						}
					}
					else if(list[t].High >= p.HighSettlementPrice) {
						Console.WriteLine((t - p.Time) + "," +( p.HighSettlementPrice - p.Price));
						done.Add(p);
					}
				}
				positions.RemoveAll(p => done.Contains(p));


				float low = list.GetRange(t - range, range).Select(s => s.Low).Min();
				if (CurrentPrice < low && positions.Count() == 0) {
					positions.Add(new TradePosition() {
						Time = t, TradeType = TradeType.Buy, Price = CurrentPrice,
						HighSettlementPrice = CurrentPrice + 0.2f,
						LowSettlementPrice = CurrentPrice - 0.2f
					});
				}
			}

		}

		private void button7_Click(object sender, EventArgs e) {

			int range = 10;
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\h1.csv"));
			for (int t = range; t < list.Count(); t++) {
				float low = list.GetRange(t - range, range).Select(s => s.Low).Min();
				if (list[t].Close < low && t < list.Count() - range * 2) {
					for(int i=1; i<=range*2; i++) {
						Console.Out.Write(list[t + i].Close - list[t].Close+",");
					}
					Console.Out.WriteLine();
				}
			}
		}

		private string FormatHourMinute(int t) {
			return (t / 2 < 10 ? "0" : "") +  t / 2 + ":" + (t % 2==0?"00":"30");
		}

		private string FormatWeekHourMinute(int t) {
			int dw = t / 48;
			return new char[] { '日', '月', '火', '水', '木', '金', '土' }[dw] + FormatHourMinute(t % 48);
		}


		private IEnumerable<Candlestick> GetByDateRangeM30(DateTime start, DateTime end) {
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv"));
			foreach(var c in list) {
				if(start <= c.Time && c.Time < end) {
					yield return c;
				}
			}
		}

		private IEnumerable<Tuple<int[], int[]>> GetBestTradeTime(IEnumerable<Candlestick> list, int limit) {
			return new BestTradeTime(list).Calculate(limit);
		}

		private IEnumerable<Tuple<int[], int[]>> GetBestTradeWeekTime(IEnumerable<Candlestick> list, int limit) {
			DayOfWeek oldDayOfWeek = new DayOfWeek();
			Candlestick[] hmList = null;
			List<Candlestick[]> dateList = new List<Candlestick[]>();
			foreach (var c in list) {
				if (hmList==null || oldDayOfWeek > c.Time.DayOfWeek) {
					hmList = new Candlestick[48*7];
					dateList.Add(hmList);
				}
				oldDayOfWeek = c.Time.DayOfWeek;
				hmList[(int)c.Time.DayOfWeek * 48 + c.Time.Hour * 2 + (c.Time.Minute == 30 ? 1 : 0)] = c;
			}

			List<Tuple<int[], int[]>> summary = new List<Tuple<int[], int[]>>();
			for (int s1 = 0; s1 < 7 * 48 - 6; s1++) {
				for (int e1 = s1 + 1; e1 < s1 + 6; e1++) {
					for (int s2 = e1 + 1; s2 < 7 * 48 - 6; s2++) {
						for (int e2 = s2 + 1; e2 < s2 + 6; e2++) {
							int[] dayResult = new int[4];
							bool f = false;
							foreach (var hml in dateList) {
								try {
									float d1 = hml[e1].Close - hml[s1].Open;
									float d2 = hml[e2].Close - hml[s2].Open;
									if (hml[e1].Close==0 ||  hml[s1].Open == 0 || hml[s2].Open == 0 || hml[e2].Close == 0 ) {
										continue;
									}
									dayResult[(d1 >= 0 ? 0 : 2) + (d2 >= 0 ? 0 : 1)]++;
									f = true;
								} catch (Exception) {
								}
							}
							if(f) {
								summary.Add(new Tuple<int[], int[]>(new int[] { s1, e1, s2, e2 }, dayResult));
							}
						}
					}
				}
			}

			int i = 0;
			foreach (var t in summary.OrderByDescending(t => (float)(t.Item2[1] + t.Item2[2]) /*/ t.Item2.Sum()*/)) {
				if (i >= limit) {
					break;
				}
				yield return t;
				i++;
			}

		}
		private IEnumerable<Tuple<DateTime,float,bool>> CheckTrade(IEnumerable<Candlestick> list, int[] tradeTimes) {
			DateTime oldDate = new DateTime();
			Candlestick[] hmList = null;
			List<Candlestick[]> dateList = new List<Candlestick[]>();
			foreach (var c in list) {
				if (!oldDate.Equals(c.Time.Date)) {
					hmList = new Candlestick[48];
					dateList.Add(hmList);
					oldDate = c.Time.Date;
				}
				hmList[c.Time.Hour * 2 + (c.Time.Minute == 30 ? 1 : 0)] = c;
			}

			foreach (var hml in dateList) {
				if(tradeTimes.Count(tt=>hml[tt].Open==0) >= 1) {
					continue;
				}

				float sd = hml[tradeTimes[1]].Close - hml[tradeTimes[0]].Open;
				float ed = hml[tradeTimes[3]].Close - hml[tradeTimes[2]].Open;
				if (sd > 0) {
					yield return new Tuple<DateTime, float,bool>(hml[tradeTimes[0]].Time, -ed,false);
				} else if (sd < 0) {
					yield return new Tuple<DateTime, float,bool>(hml[tradeTimes[0]].Time, ed,true);
				}
			}
		}

		private IEnumerable<Tuple<DateTime, float>> CheckWeekTrade(IEnumerable<Candlestick> list, int[] tradeTimes) {
			DayOfWeek oldDayOfWeek = new DayOfWeek();
			Candlestick[] hmList = null;
			List<Candlestick[]> dateList = new List<Candlestick[]>();
			foreach (var c in list) {
				if (hmList == null || oldDayOfWeek > c.Time.DayOfWeek) {
					hmList = new Candlestick[48 * 7];
					dateList.Add(hmList);
				}
				oldDayOfWeek = c.Time.DayOfWeek;
				hmList[(int)c.Time.DayOfWeek * 48 + c.Time.Hour * 2 + (c.Time.Minute == 30 ? 1 : 0)] = c;
			}

			foreach (var hml in dateList) {
				if (tradeTimes.Count(tt => hml[tt].Open == 0) >= 1) {
					continue;
				}

				float sd = hml[tradeTimes[1]].Close - hml[tradeTimes[0]].Open;
				float ed = hml[tradeTimes[3]].Close - hml[tradeTimes[2]].Open;
				if (sd > 0) {
					yield return new Tuple<DateTime, float>(hml[tradeTimes[0]].Time, -ed);
				} else if (sd < 0) {
					yield return new Tuple<DateTime, float>(hml[tradeTimes[0]].Time, ed);
				}
			}
		}



		private void RunTask(object sender, Action<Report> action) {
			Task.Run(() => {
				using (this.report = new Report() {
					Name = ((Control)sender).Text,
					BasePath = REPORT_PATH,
					DataGridView = dataGridView1,
					StatusControl = taskStatus,
				}) {
					try {
						action(this.report);
					} catch(ReportExistedException) {
					}
				}
			});
		}

		private void excelButton_Click(object sender, EventArgs e) {
			if (this.report != null) {
				System.Diagnostics.Process.Start("excel.exe", this.report.CsvFilePath);
			}
		}



		private void 日時ベスト(object sender, EventArgs ev) {
			RunTask(sender, (Report report) => {
				report.Version = 4;
				report.IsForceOverride = false;
				report.Comment = "最短1h→30m";
				report.SetHeader("start","end", "start", "end", "↑↑","↑↓","↓↑","↓↓");
				foreach (var t in GetBestTradeTime((new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv")), 100)) {
					foreach (var time in t.Item1) {
						report.Write(FormatHourMinute(time));
					}
					foreach (var c in t.Item2) {
						report.Write(c);
					}
					report.WriteLine();
				}
			});
		}
		
		private void 日時ベスト特定日時検証(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.Comment = "0000-0430_0500-0700";
				report.SetHeader("date", "balance", "isbuy", "isplus");
				List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv"));
				DateTime oldDate = new DateTime();
				Candlestick[] hmList = null;
				List<Candlestick[]> dateList = new List<Candlestick[]>();
				foreach (var c in list) {
					if (!oldDate.Equals(c.Time.Date)) {
						hmList = new Candlestick[48];
						dateList.Add(hmList);
						oldDate = c.Time.Date;
					}
					hmList[c.Time.Hour * 2 + (c.Time.Minute == 30 ? 1 : 0)] = c;
				}

				foreach (var t in CheckTrade(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv"), new int[] { 0, 9, 10, 14 })) {
					report.WriteLine(t.Item1.Year + "/" + t.Item1.Month + "/" + t.Item1.Day,t.Item2,(t.Item3 ? "1" : "0"),(t.Item2 > 0 ? "1" : "0"));
				}

			});

		}


		private void 日時ベスト検証リミット別(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.Comment = "";
				report.SetHeader("limit","usable","total","ratio","avg");
				List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv"));
				for (int limit = 1; limit <= 40; limit += 2) {
					DateTime startTime = list[0].Time;
					int total = 0;
					int usable = 0;
					float summ = 0;
					while (true) {
						DateTime endTime = startTime.AddMonths(24);
						DateTime checkEndTime = endTime.AddMonths(1);
						List<Candlestick> targetList = new List<Candlestick>(list.Where(c => startTime <= c.Time && c.Time < endTime));
						List<Candlestick> checkList = new List<Candlestick>(list.Where(c => endTime <= c.Time && c.Time < checkEndTime));
						if (checkList.Count == 0) {
							break;
						}
						foreach (var t in GetBestTradeTime(targetList, limit)) {
							float m = CheckTrade(checkList, t.Item1).Sum(r => r.Item2);
							if (m > 1 * 30 * 0.003f) {
								usable++;
							}
							summ += m;
							total++;
						}
						startTime = startTime.AddMonths(1);
					}
					report.WriteLine(limit,usable,total,(float)usable / total,summ / total);
				}
			});
		}
		private void 日時ベスト検証期間別(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.Comment = "";
				report.IsForceOverride = true;
				report.SetHeader("month", "usable", "total", "ratio", "avg", "max", "min");
				List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv"));
				for (int month = 1; month <= 48; month++) {
					DateTime startTime = list[0].Time;
					int total = 0;
					int usable = 0;
					float summ = 0;
					float max = float.MinValue;
					float min = float.MaxValue;
					while (true) {
						DateTime endTime = startTime.AddMonths(month);
						DateTime checkEndTime = endTime.AddMonths(1);
						List<Candlestick> targetList = new List<Candlestick>(list.Where(c => startTime <= c.Time && c.Time < endTime));
						List<Candlestick> checkList = new List<Candlestick>(list.Where(c => endTime <= c.Time && c.Time < checkEndTime));
						if (checkList.Count == 0) {
							break;
						}
						foreach (var t in GetBestTradeTime(targetList, 5)) {
							float m = CheckTrade(checkList, t.Item1).Sum(r => r.Item2) - 1 * 20 * 0.003f;
							if (m > 0) {
								usable++;
							}
							summ += m;
							max = Math.Max(max, m);
							min = Math.Min(min, m);
							total++;
						}
						startTime = startTime.AddMonths(1);
					}
					report.WriteLine(month, usable, total, (float)usable / total, summ / total, max, min);
				}
			});
		}


		private void 日時ベスト検証リスク(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("date", "balance");
				List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv"));
				DateTime startTime = list[0].Time;
				int total = 0;
				int usable = 0;
				float summ = 0;
				float max = float.MinValue;
				float min = float.MaxValue;
				while (true) {
					DateTime endTime = startTime.AddMonths(30);
					DateTime checkEndTime = endTime.AddMonths(1);
					List<Candlestick> targetList = new List<Candlestick>(list.Where(c => startTime <= c.Time && c.Time < endTime));
					List<Candlestick> checkList = new List<Candlestick>(list.Where(c => endTime <= c.Time && c.Time < checkEndTime));
					if (checkList.Count == 0) {
						break;
					}
					foreach (var t in GetBestTradeTime(targetList, 5)) {
						float m = CheckTrade(checkList, t.Item1).Sum(r => r.Item2) - 1 * 20 * 0.003f;
						report.WriteLine(endTime.Year + "-" + endTime.Month, m);
						if (m > 0) {
							usable++;
						}
						summ += m;
						max = Math.Max(max, m);
						min = Math.Min(min, m);
						total++;
					}
					startTime = startTime.AddMonths(1);
				}
			});
		}

		private void 曜日ベスト(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.Comment = "";
				report.IsForceOverride = true;
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓", "ratio");
				foreach (var t in GetBestTradeWeekTime((new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv")), 80)) {
					foreach (var time in t.Item1) {
						report.Write(FormatWeekHourMinute(time));
					}
					foreach (var c in t.Item2) {
						report.Write(c);
					}
					report.Write((float)(t.Item2[1] + t.Item2[2]) / t.Item2.Sum());
					report.WriteLine();
				}
			});
		}


		private void 曜日ベスト検証(object sender, EventArgs ea) {
			RunTask(sender,(Report report) => {
				report.Version = 3;
				report.IsForceOverride = false;
				report.SetHeader("month", "usable", "total", "ratio", "avg", "max", "min");

				List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv"));
				DateTime startTime = list[0].Time;
				int total = 0;
				int usable = 0;
				float summ = 0;
				float max = float.MinValue;
				float min = float.MaxValue;
				while (true) {
					DateTime endTime = startTime.AddMonths(32);
					DateTime checkEndTime = endTime.AddMonths(1);
					List<Candlestick> targetList = new List<Candlestick>(list.Where(c => startTime <= c.Time && c.Time < endTime));
					List<Candlestick> checkList = new List<Candlestick>(list.Where(c => endTime <= c.Time && c.Time < checkEndTime));
					if (checkList.Count == 0) {
						break;
					}
					int usableInMonth = 0;
					int totalInMonth = 0;
					float summInMonth = 0;
					float maxInMonth = float.MinValue;
					float minInMonth = float.MaxValue;
					foreach (var t in GetBestTradeWeekTime(targetList, 20)) {
						float m = CheckWeekTrade(checkList, t.Item1).Sum(r => r.Item2) - 1 * 4 * 0.003f;
						if (m > 0) {
							usableInMonth++;
						}
						summInMonth += m;
						maxInMonth = Math.Max(maxInMonth, m);
						minInMonth = Math.Min(minInMonth, m);
						totalInMonth++;
					}
					report.WriteLine(endTime.Year + "/" + endTime.Month, usableInMonth, totalInMonth, (float)usableInMonth / totalInMonth, summInMonth / totalInMonth, maxInMonth, minInMonth);
					total += totalInMonth;
					usable += usableInMonth;
					summ += summInMonth;
					max = Math.Max(max, maxInMonth);
					min = Math.Min(min, minInMonth);
					startTime = startTime.AddMonths(1);
				}
				report.WriteLine("total", usable, total, (float)usable / total, summ / total, max, min);
			});
		}

		private void 秒足取得(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.SetHeader("time","open","high","low","close","volume");

				DateTime current = DateTime.UtcNow;

				foreach (var c in new OandaAPI().GetCandles(current.AddHours(-1), current)) {
					report.WriteLine(c.time, c.openMid, c.highMid, c.lowMid, c.closeMid, c.volume);
				}
			});
		}


		private void オーダーブック取得(object sender, EventArgs ev) {

			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;

				DateTime current = DateTime.UtcNow;
				var data = new OandaAPI().GetOrderbookData();
				var pricePoints = data[data.Keys.Max()];

				report.Comment = data.Keys.Max().ToString().Replace('/','-').Replace(':','-').Replace(' ','_');
				report.SetHeader("price", "order_long", "order_short", "pos_long", "pos_short");

				foreach (var e in pricePoints.price_points.Keys.OrderBy(k=>k)) {
					var p = pricePoints.price_points[e];
					report.WriteLine(e, p.ol, p.os, p.pl, p.ps);
				}
			});
		}

		struct GetBestTradeTimePinBarResult {
			public bool IsUp1;
			public bool IsUp2;
			public int pinbarType; 
		}

		private int GetBestTradeTimePinBarResultIndex(bool IsUp1, bool IsUp2, int pinbarType) {
			return ((IsUp1 ? 1 : 0) * 2 + (IsUp2 ? 1 : 0)) * 3 + pinbarType;
		}

		private IEnumerable<Tuple<int[], int[]>> GetBestTradeTimePinBar(IEnumerable<Candlestick> list, int limit) {
			DateTime oldDate = new DateTime();
			Candlestick[] hmList = null;
			List<Candlestick[]> dateList = new List<Candlestick[]>();
			foreach (var c in list) {
				if (!oldDate.Equals(c.Time.Date)) {
					hmList = new Candlestick[48];
					dateList.Add(hmList);
					oldDate = c.Time.Date;
				}
				hmList[c.Time.Hour * 2 + (c.Time.Minute == 30 ? 1 : 0)] = c;
			}

			List<Tuple<int[], int[]>> summary = new List<Tuple<int[], int[]>>();
			for (int s1 = 0; s1 < 48 - 12; s1++) {
				for (int e1 = s1 + 1; e1 < s1 + 12; e1++) {
					for (int s2 = e1 + 1; s2 < 48 - 12; s2++) {
						for (int e2 = s2 + 1; e2 < s2 + 12; e2++) {
							int[] dayResult = new int[12];
							summary.Add(new Tuple<int[], int[]>(new int[] { s1, e1, s2, e2 }, dayResult));
							foreach (var hml in dateList) {
								try {
									if (hml[e1].Close == 0 || hml[s1].Open == 0 || hml[s2].Open == 0 || hml[e2].Close == 0) {

										continue;
									}
									float d1 = hml[e1].Close - hml[s1].Open;
									float d2 = hml[e2].Close - hml[s2].Open;

									var c1 = Candlestick.Create(hml.Skip(s1).Take(e1 - s1 + 1));
									int barType;
									if(c1.IsPinbar(true, 0.3f, 0.1f)) {
										barType = 1;
									} else if(c1.IsPinbar(false, 0.3f, 0.1f)) {
										barType = 2;
									} else {
										barType = 0;
									}
									dayResult[GetBestTradeTimePinBarResultIndex(d1 >= 0, d2 >= 0,barType)]++;
								} catch (Exception) {
								}
							}
						}
					}
				}
			}

			int best = 0;
			foreach (var t in summary.OrderByDescending(t => 
				new int[] { 0, 1, 2 }.Max( barType=> t.Item2[GetBestTradeTimePinBarResultIndex(true, false, barType)]) +
				new int[] { 0, 1, 2 }.Max(barType => t.Item2[GetBestTradeTimePinBarResultIndex(false, true, barType)])) ) {
				if (best >= limit) {
					break;
				}
				yield return t;
				best++;
			}

		}
		private void 日時ベストピンバー(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("start", "end", "start", "end", 
					"↑↑", "↑上↑", "↑下↑",
					"↑↓", "↑上↓", "↑下↓",
					"↓↑", "↓上↑", "↓下↑",
					"↓↓", "↓上↓", "↓下↓");
				foreach (var t in GetBestTradeTimePinBar((new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv")), 100)) {
					foreach (var time in t.Item1) {
						report.Write(FormatHourMinute(time));
					}
					foreach (var c in t.Item2) {
						report.Write(c);
					}
					report.WriteLine();
				}
			});
		}

		private void 日時ベスト時間ずらし(object sender, EventArgs e) {

			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓");
				BestTradeTime bestTradeTime = new BestTradeTime(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv"));
				bestTradeTime.ShiftHour = 12;

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

		private void 日時上下ベスト(object sender, EventArgs e) {

			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.IsForceOverride = true;
				report.Comment = "";
				report.SetHeader("start", "end", "start", "end", "↑↑", "↑↓", "↓↑", "↓↓");
				BestTradeTime bestTradeTime = new BestTradeTime(new CandlesticksReader().Read(@"C:\Users\玲\Desktop\m30-5y.csv"));
				bestTradeTime.Comparator = f => f[1] - f[0];
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

		private void オーダーブックビュー(object sender, EventArgs e) {
			var d = new OrderBookForm();
			d.Show(this);
//			d.ShowDialog(this);
		}
	}
}
