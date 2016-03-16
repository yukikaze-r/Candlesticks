using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Candlesticks {

	public partial class Form1 : Form {
		
		public string REPORT_PATH {
			get {
				return Setting.Instance.DataFilePath;
			}
		}

		public string DATA_PATH {
			get {
				return Setting.Instance.DataFilePath;
			}
		}

		private Report report;

		public Form1() {
			InitializeComponent();
		}

		private List<TradePosition> positions = new List<TradePosition>();

		private void Form1_Load(object sender, EventArgs e) {
			new Thread(new ThreadStart(() => { EventReceiver.Instance.Execute(); })).Start();
		}


		private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
			Console.WriteLine("Form1_FormClosed");
			EventReceiver.Instance.Dispose();
			Application.Exit();
		}

		private void button1_Click(object sender, EventArgs e) {
			int range = 8;
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\h1.csv"));
			for(int t = range; t < list.Count(); t++) {
//				DoSettlement(t, list[t].Close);
				float low = list.GetRange(t - range, range).Select(s => s.Low).Min();
				if(list[t].Close < low) {
					DoTrade(t, TradeType.Ask, list[t].Close);
					// sell list[t].Close
				}
				float high = list.GetRange(t - range, range).Select(s => s.High).Max();
				if (list[t].Close > high) {
					DoTrade(t, TradeType.Bid, list[t].Close);
				}
			}
		}

		private void DoSettlement(int t, float price) {
			bool f = false;
			float money = 0;
			foreach (var position in positions) {
				f = true;
				if (position.TradeType == TradeType.Ask) {
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
				if (position.TradeType == TradeType.Ask) {
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
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\h1.csv"));
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
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\h1.csv"));
			Dictionary<string, List<Candlestick>> dayOftheWeekTimeDict = new Dictionary<string, List<Candlestick>>();
			foreach(var stick in list) {
				string key = stick.DateTime.DayOfWeek + " " + stick.DateTime.Hour;
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
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\h1.csv"));
			List<Dictionary<string, Candlestick>> dayOfWeekTable = new List<Dictionary<string, Candlestick>>();
			Dictionary<string, Candlestick> dict = new Dictionary<string, Candlestick>();
			DayOfWeek oldDayOfWeek = DayOfWeek.Friday;
			foreach (var stick in list) {
				if(stick.DateTime.DayOfWeek < oldDayOfWeek) {
					dict = new Dictionary<string, Candlestick>();
					dayOfWeekTable.Add(dict);
				}
				string key = stick.DateTime.DayOfWeek + " " + stick.DateTime.Hour;
				dict.Add(key, stick);
				oldDayOfWeek = stick.DateTime.DayOfWeek;
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
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\h1.csv"));
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
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\h1.csv"));
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
						Time = t, TradeType = TradeType.Ask, Price = CurrentPrice,
						HighSettlementPrice = CurrentPrice + 0.2f,
						LowSettlementPrice = CurrentPrice - 0.2f
					});
				}
			}

		}

		private void button7_Click(object sender, EventArgs e) {

			int range = 10;
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\h1.csv"));
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
			List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv"));
			foreach(var c in list) {
				if(start <= c.DateTime && c.DateTime < end) {
					yield return c;
				}
			}
		}

		private IEnumerable<Tuple<int[], int[]>> GetBestTradeTime(IEnumerable<Candlestick> list, int limit, bool isSummerTime) {
			return new BestTradeTime(list) { IsSummerTime = isSummerTime }.Calculate(limit);
		}

		private IEnumerable<Tuple<int[], int[]>> GetBestTradeWeekTime(IEnumerable<Candlestick> list, int limit) {
			DayOfWeek oldDayOfWeek = new DayOfWeek();
			Candlestick[] hmList = null;
			List<Candlestick[]> dateList = new List<Candlestick[]>();
			foreach (var c in list) {

				if (hmList==null || oldDayOfWeek > c.DateTime.DayOfWeek) {
					hmList = new Candlestick[48*7];
					dateList.Add(hmList);
				}
				oldDayOfWeek = c.DateTime.DayOfWeek;
				hmList[(int)c.DateTime.DayOfWeek * 48 + c.DateTime.Hour * 2 + (c.DateTime.Minute == 30 ? 1 : 0)] = c;
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
				if (!oldDate.Equals(c.DateTime.Date)) {
					hmList = new Candlestick[48];
					dateList.Add(hmList);
					oldDate = c.DateTime.Date;
				}
				hmList[c.DateTime.Hour * 2 + (c.DateTime.Minute == 30 ? 1 : 0)] = c;
			}

			foreach (var hml in dateList) {
				if(tradeTimes.Count(tt=>hml[tt].Open==0) >= 1) {
					continue;
				}

				float sd = hml[tradeTimes[1]].Close - hml[tradeTimes[0]].Open;
				float ed = hml[tradeTimes[3]].Close - hml[tradeTimes[2]].Open;
				if (sd > 0) {
					yield return new Tuple<DateTime, float,bool>(hml[tradeTimes[0]].DateTime, -ed,false);
				} else if (sd < 0) {
					yield return new Tuple<DateTime, float,bool>(hml[tradeTimes[0]].DateTime, ed,true);
				}
			}
		}

		private IEnumerable<Tuple<DateTime, float>> CheckWeekTrade(IEnumerable<Candlestick> list, int[] tradeTimes) {
			DayOfWeek oldDayOfWeek = new DayOfWeek();
			Candlestick[] hmList = null;
			List<Candlestick[]> dateList = new List<Candlestick[]>();
			foreach (var c in list) {
				if (hmList == null || oldDayOfWeek > c.DateTime.DayOfWeek) {
					hmList = new Candlestick[48 * 7];
					dateList.Add(hmList);
				}
				oldDayOfWeek = c.DateTime.DayOfWeek;
				hmList[(int)c.DateTime.DayOfWeek * 48 + c.DateTime.Hour * 2 + (c.DateTime.Minute == 30 ? 1 : 0)] = c;
			}

			foreach (var hml in dateList) {
				if (tradeTimes.Count(tt => hml[tt].Open == 0) >= 1) {
					continue;
				}

				float sd = hml[tradeTimes[1]].Close - hml[tradeTimes[0]].Open;
				float ed = hml[tradeTimes[3]].Close - hml[tradeTimes[2]].Open;
				if (sd > 0) {
					yield return new Tuple<DateTime, float>(hml[tradeTimes[0]].DateTime, -ed);
				} else if (sd < 0) {
					yield return new Tuple<DateTime, float>(hml[tradeTimes[0]].DateTime, ed);
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
			new DateTimeBestForm().Show();
		}
		
		private void 日時ベスト特定日時検証(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.Comment = "0000-0430_0500-0700";
				report.SetHeader("date", "balance", "isbuy", "isplus");
				List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv"));
				DateTime oldDate = new DateTime();
				Candlestick[] hmList = null;
				List<Candlestick[]> dateList = new List<Candlestick[]>();
				foreach (var c in list) {
					if (!oldDate.Equals(c.DateTime.Date)) {
						hmList = new Candlestick[48];
						dateList.Add(hmList);
						oldDate = c.DateTime.Date;
					}
					hmList[c.DateTime.Hour * 2 + (c.DateTime.Minute == 30 ? 1 : 0)] = c;
				}

				foreach (var t in CheckTrade(new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv"), new int[] { 0, 9, 10, 14 })) {
					report.WriteLine(t.Item1.Year + "/" + t.Item1.Month + "/" + t.Item1.Day,t.Item2,(t.Item3 ? "1" : "0"),(t.Item2 > 0 ? "1" : "0"));
				}

			});

		}


		private void 日時ベスト検証リミット別(object sender, EventArgs e) {
			RunTask(sender, (Report report) => {
				report.Version = 1;
				report.Comment = "";
				report.SetHeader("limit","usable","total","ratio","avg");
				List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv"));
				for (int limit = 1; limit <= 40; limit += 2) {
					DateTime startTime = list[0].DateTime;
					int total = 0;
					int usable = 0;
					float summ = 0;
					while (true) {
						DateTime endTime = startTime.AddMonths(24);
						DateTime checkEndTime = endTime.AddMonths(1);
						List<Candlestick> targetList = new List<Candlestick>(list.Where(c => startTime <= c.DateTime && c.DateTime < endTime));
						List<Candlestick> checkList = new List<Candlestick>(list.Where(c => endTime <= c.DateTime && c.DateTime < checkEndTime));
						if (checkList.Count == 0) {
							break;
						}
						foreach (var t in GetBestTradeTime(targetList, limit, false)) {
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
				List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv"));
				for (int month = 1; month <= 48; month++) {
					DateTime startTime = list[0].DateTime;
					int total = 0;
					int usable = 0;
					float summ = 0;
					float max = float.MinValue;
					float min = float.MaxValue;
					while (true) {
						DateTime endTime = startTime.AddMonths(month);
						DateTime checkEndTime = endTime.AddMonths(1);
						List<Candlestick> targetList = new List<Candlestick>(list.Where(c => startTime <= c.DateTime && c.DateTime < endTime));
						List<Candlestick> checkList = new List<Candlestick>(list.Where(c => endTime <= c.DateTime && c.DateTime < checkEndTime));
						if (checkList.Count == 0) {
							break;
						}
						foreach (var t in GetBestTradeTime(targetList, 5, false)) {
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
				List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv"));
				DateTime startTime = list[0].DateTime;
				int total = 0;
				int usable = 0;
				float summ = 0;
				float max = float.MinValue;
				float min = float.MaxValue;
				while (true) {
					DateTime endTime = startTime.AddMonths(30);
					DateTime checkEndTime = endTime.AddMonths(1);
					List<Candlestick> targetList = new List<Candlestick>(list.Where(c => startTime <= c.DateTime && c.DateTime < endTime));
					List<Candlestick> checkList = new List<Candlestick>(list.Where(c => endTime <= c.DateTime && c.DateTime < checkEndTime));
					if (checkList.Count == 0) {
						break;
					}
					foreach (var t in GetBestTradeTime(targetList, 5, false)) {
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
				foreach (var t in GetBestTradeWeekTime((new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv")), 80)) {
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

				List<Candlestick> list = new List<Candlestick>(new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv"));
				DateTime startTime = list[0].DateTime;
				int total = 0;
				int usable = 0;
				float summ = 0;
				float max = float.MinValue;
				float min = float.MaxValue;
				while (true) {
					DateTime endTime = startTime.AddMonths(32);
					DateTime checkEndTime = endTime.AddMonths(1);
					List<Candlestick> targetList = new List<Candlestick>(list.Where(c => startTime <= c.DateTime && c.DateTime < endTime));
					List<Candlestick> checkList = new List<Candlestick>(list.Where(c => endTime <= c.DateTime && c.DateTime < checkEndTime));
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

				DateTime current = DateTime.Now;
				
				using(DBUtils.OpenThreadConnection()) {
					foreach(var c in new CandlesticksGetter() {
						Instrument = "USD_JPY",
						Granularity = "M1",
						Start = new DateTime(2011, 4,1, 0,00,0,DateTimeKind.Local),
						End = new DateTime(2016, 3, 2, 16, 30, 0, DateTimeKind.Local),
					}.Execute()) {
//						report.WriteLine(c.Time, c.Open, c.High, c.Low, c.Close, c.Volume);
					}
				}
				/*
				var end = (new CandlesticksGetter() {
					Granularity = "H1"
				}.GetAlignTime(current.AddHours(-11)));

				foreach (var c in new OandaAPI().GetCandles(current.AddHours(-12), end, "USD_JPY","H1")) {
					report.WriteLine(c.DateTime, c.openMid, c.highMid, c.lowMid, c.closeMid, c.volume);
				}*/
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
				if (!oldDate.Equals(c.DateTime.Date)) {
					hmList = new Candlestick[48];
					dateList.Add(hmList);
					oldDate = c.DateTime.Date;
				}
				hmList[c.DateTime.Hour * 2 + (c.DateTime.Minute == 30 ? 1 : 0)] = c;
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

									var c1 = Candlestick.Aggregate(hml.Skip(s1).Take(e1 - s1 + 1));
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
				foreach (var t in GetBestTradeTimePinBar((new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv")), 100)) {
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
				BestTradeTime bestTradeTime = new BestTradeTime(new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv"));
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
				BestTradeTime bestTradeTime = new BestTradeTime(new CandlesticksReader().Read(DATA_PATH + @"\m30-5y.csv"));
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
//			d.ShowDialog(this);
		}

		private void 設定ToolStripMenuItem_Click(object sender, EventArgs e) {
			var dialog = new SettingDialog();
			dialog.ShowDialog(this);
		}


		private void 原油因果_Click(object sender, EventArgs ev) {

			RunTask(sender, (Report report) => {
				report.Version = 3;
				report.IsForceOverride = true;
				report.Comment = "時間帯";
				report.SetHeader("開始","終了","原油t","ドル円t","上昇量", "↑↑", "↑↓", "↓↑", "↓↓");

				using (DBUtils.OpenThreadConnection()) {
					DateTime start = DateTime.Now.AddYears(-1);
					DateTime end = DateTime.Now.AddHours(-1);

					var wtico = new CandlesticksGetter() {
						Instrument = "WTICO_USD",
						Granularity = "M10",
						Start = start,
						End = end
					}.Execute().ToList();

					var usdjpy = new CandlesticksGetter() {
						Instrument = "USD_JPY",
						Granularity = "M10",
						Start = start,
						End = end
					}.Execute().ToList();


					Console.WriteLine("wtico:" + wtico.Count() + " usdjpy:" + usdjpy.Count());

					for(TimeSpan s = new TimeSpan(0,0,0); s < new TimeSpan(24,0,0); s += new TimeSpan(1,0,0)) {
						for (TimeSpan e = s + new TimeSpan(1, 0, 0); e < new TimeSpan(24, 0, 0); e += new TimeSpan(1, 0, 0)) {
							StiUsdJpy(s,e, wtico, usdjpy, report, 6, 4, 0);
						}
					}
/*					for (float d = 0; d < 1f; d += 0.01f) {
						StiUsdJpy(wtico, usdjpy, report, 6, 4, d);
					}*/

				}
			});
		}

		private void StiUsdJpy(TimeSpan start, TimeSpan end, List<Candlestick> wtico, List<Candlestick> usdjpy, Report report, int n, int m, float thrashold) {

			int upup = 0;
			int updown = 0;
			int downup = 0;
			int downdown = 0;
			for (int i = 0; i < wtico.Count() - (n + m); i++) {
				if (wtico[i].DateTime != usdjpy[i].DateTime) {
					Console.WriteLine("different time");
				}
				TimeSpan t = wtico[i].DateTime.TimeOfDay;
				if(!(start <= t && t < end)) {
					continue;
				}

				var w = TakeRange(wtico, i, i + n);
				if (w.Count(c => c.IsNull) >= 1) {
					continue;
				}
				if (Candlestick.Aggregate(w).IsUp(thrashold)) {
					var u = TakeRange(usdjpy, i + n, i + n + m);
					if (u.Count(c => c.IsNull) >= 1) {
						continue;
					}
					if (Candlestick.Aggregate(u).IsUp()) {
						upup++;
					} else {
						updown++;
					}
				} else {
					var u = TakeRange(usdjpy, i + n, i + n + m);
					if (u.Count(c => c.IsNull) >= 1) {
						continue;
					}
					if (Candlestick.Aggregate(u).IsUp()) {
						downup++;
					} else {
						downdown++;
					}
				}
			}

			report.WriteLine(start,end,n,m,thrashold, upup, updown, downup, downdown);
		}

		private IEnumerable<T> TakeRange<T>(List<T> list, int s, int e) {
			for(int i= s; i< e; i++) {
				yield return list[i];
			}
		}

		private void くるくる_Click(object sender, EventArgs e) {
			Simulator simulator = new Simulator(null);

			bool first = true;

			foreach(var current in simulator) {
				if(first) {
					current.Ask(10);
					current.Bid(5);
					first = false;
				} else {

				}
			}

		}

		private class CheckRange {
			public TimeSpan start;
			public TimeSpan end;
			public bool isInRange = false;
			public Candlestick sum;
			public Action<CheckRange> endTimeFunc = delegate { };
			public float maxUp = float.MinValue;
			public float maxDown = float.MaxValue;
			public int upCount;
			public int downCount;
			public float totalInUp;
			public float totalInDown;

			public void Check(Candlestick candle) {
				if (candle.DateTime.TimeOfDay == start && candle.IsNull == false) {
					isInRange = true;
					sum = candle;
				} else if (isInRange) {
					if (candle.IsNull) {
						isInRange = false;
					} else if (candle.DateTime.TimeOfDay == end) {
						isInRange = false;
						endTimeFunc(this);
					} else {
						sum = sum.Add(candle);
					}
				}
			}
		}

		private void 区間の特徴_Click(object sender, EventArgs e) {

			RunTask(sender, (Report report) => {
				using(DBUtils.OpenThreadConnection()) {
					report.Version = 6;
//					report.IsForceOverride = true;
					report.Comment = "夏時間";
					report.SetHeader("区間1", "区間2", "区間3", "区間4", "最高値", "最安値", "終値");

					DateTime start = DateTime.Now.AddYears(-5);
					DateTime end = DateTime.Now.AddHours(-1);

/*					CheckRange signal = new CheckRange() {
						start = new TimeSpan(0, 0, 0),
						end = new TimeSpan(5,30,0)
					};*/

					CheckRange[] sub = new CheckRange[] {
						new CheckRange() {
							start = new TimeSpan(5, 30, 0),
							end = new TimeSpan(6, 00, 0)
						},
						new CheckRange() {
							start = new TimeSpan(6, 00, 0),
							end = new TimeSpan(6, 30, 0)
						},
						new CheckRange() {
							start = new TimeSpan(6, 30, 0),
							end = new TimeSpan(7, 00, 0)
						},
						new CheckRange() {
							start = new TimeSpan(7, 00, 0),
							end = new TimeSpan(7, 30, 0)
						}

					};

					CheckRange total = new CheckRange() {
						start = new TimeSpan(5, 30, 0),
						end = new TimeSpan(7, 30, 0),
						endTimeFunc = new Action<CheckRange>((range) => {
							foreach(var s in sub) {
								report.Write(s.sum.Close - s.sum.Open);
							}
							report.WriteLine(
								range.sum.High - range.sum.Open, range.sum.Low - range.sum.Open, range.sum.Close - range.sum.Open);
						})
					};

					TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
					foreach (var candle in new CandlesticksGetter() {
						Instrument = "USD_JPY",
						Granularity = "M10",
						Start = start,
						End = end
					}.Execute()) {
						if(est.IsDaylightSavingTime(candle.DateTime)==false) {
							continue;
						}
//						signal.Check(candle);
						foreach(var s in sub) {
							s.Check(candle);
						}
						total.Check(candle);

					}
				}
			});

		}

		private void ベスト区間_Click(object sender, EventArgs e) {

			RunTask(sender, (Report report) => {
				using (DBUtils.OpenThreadConnection()) {
					report.Version = 1;
					report.IsForceOverride = true;
					report.Comment = "直近1年";
					report.SetHeader("時間範囲", "夏時間?", "上昇率", "平均変動額", "平均上昇額", "平均下降額","最大上昇", "最大下降");

					DateTime start = DateTime.Now.AddYears(-1);
					DateTime end = DateTime.Now.AddHours(-1);


					foreach (bool isSummerTime in new bool[] { true, false }) {
						List<CheckRange> ranges = new List<CheckRange>();
						for (int i = 0; i < 48; i++) {
							ranges.Add(new CheckRange() {
								start = new TimeSpan(i / 2, (i % 2) * 30, 0),
								end = new TimeSpan((i + 1) / 2, ((i + 1) % 2) * 30, 0),
								endTimeFunc = new Action<CheckRange>((range) => {
									float delta = range.sum.Close - range.sum.Open;
									if (delta > 0) {
										range.upCount++;
										range.maxUp = Math.Max(range.maxUp, delta);
										range.totalInUp += delta;
									} else {
										range.downCount++;
										range.maxDown = Math.Min(range.maxDown, delta);
										range.totalInDown += delta;
									}

								})
							});
						}

						TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
						foreach (var candle in new CandlesticksGetter() {
							Instrument = "USD_JPY",
							Granularity = "M10",
							Start = start,
							End = end
						}.Execute()) {
							if (est.IsDaylightSavingTime(candle.DateTime) != isSummerTime) {
								continue;
							}
							foreach (var s in ranges) {
								s.Check(candle);
							}
						}
						foreach (var s in ranges) {
							report.WriteLine(s.start + "-" + s.end, isSummerTime, (float)s.upCount / (s.upCount + s.downCount), (s.totalInUp + s.totalInDown) / (s.upCount + s.downCount), s.totalInUp / s.upCount, s.totalInDown / s.downCount, s.maxUp, s.maxDown);
						}
					}
				}
			});

		}

		private void ストリーミングテスト_Click(object sender, EventArgs e) {
//			new OandaAPI().GetPrices((bid,ask)=>{ Console.WriteLine("bid:"+bid+" ask:"+ask); }, "USD_JPY");
		}

		private void オーダーブックToolStripMenuItem_Click(object sender, EventArgs e) {
			new OrderBookForm().Show();
		}

		private void シグナルToolStripMenuItem_Click(object sender, EventArgs e) {
			new SignalForm().Show();
		}

		private void チャートToolStripMenuItem_Click(object sender, EventArgs e) {
			new ChartForm().Show();

		}

		private void ポジション傾向ToolStripMenuItem_Click(object sender, EventArgs e) {
			new PositionsForm().Show();
		}

		private void 価格急変_Click(object sender, EventArgs e) {

			RunTask(sender, (Report report) => {
				/*
				report.Version = 1;
				report.IsForceOverride = true;
				report.SetHeader("pips","count");

				using (DBUtils.OpenThreadConnection()) {
					DateTime start = DateTime.Now.AddYears(-5);
					DateTime end = DateTime.Now.AddHours(-1);
					
					var usdjpy = new CandlesticksGetter() {
						Instrument = "USD_JPY",
						Granularity = "M10",
						Start = start,
						End = end
					}.Execute().ToList();

					Dictionary<int, int> sum = new Dictionary<int,int>();
					foreach(var c in usdjpy) {
						var d = Math.Abs(c.Close - c.Open);
						var key = (int)Math.Round(d*10);
						if(sum.ContainsKey(key) == false) {
							sum[key] = 1;
						} else {
							sum[key]++;
						}
					}
					foreach (var key in sum.Keys.OrderBy(k=>k)) {
						report.WriteLine(key, sum[key]);
					}


				}*/
				/*
				report.Version = 2;
				report.IsForceOverride = true;
				report.Comment = "次足の振幅";
				report.SetHeader("振幅");

				using (DBUtils.OpenThreadConnection()) {
					DateTime start = DateTime.Now.AddYears(-5);
					DateTime end = DateTime.Now.AddHours(-1);
					
					var usdjpy = new CandlesticksGetter() {
						Instrument = "USD_JPY",
						Granularity = "M10",
						Start = start,
						End = end
					}.Execute().ToList();

					Dictionary<int, int> sum = new Dictionary<int,int>();
					for(int t=0; t<usdjpy.Count(); t++) {
						var c = usdjpy[t];
						var d = Math.Max(c.Open - c.Low, c.High - c.Open);
						if ( d >= 0.2) {
							var c1 = usdjpy[t + 1];
							var d1 = Math.Max(c1.Open - c1.Low,c1.High - c1.Open);
							report.WriteLine(d1);
						}
					}
				}*/

				/*
				report.Version = 3;
				report.IsForceOverride = true;
				report.Comment = "平均振幅";
				report.SetHeader("平均振幅");

				using (DBUtils.OpenThreadConnection()) {
					DateTime start = DateTime.Now.AddYears(-5);
					DateTime end = DateTime.Now.AddHours(-1);

					var usdjpy = new CandlesticksGetter() {
						Instrument = "USD_JPY",
						Granularity = "M10",
						Start = start,
						End = end
					}.Execute().ToList();

					float sum = 0;
					for (int t = 0; t < usdjpy.Count(); t++) {
						var c = usdjpy[t];
						var d = Math.Max(c.Open - c.Low, c.High - c.Open);
						sum += d;
					}
					report.WriteLine(sum / usdjpy.Count());
				}*/


				report.Version = 6;
				report.IsForceOverride = true;
				report.Comment = "逆方向振幅";
				report.SetHeader("逆方向最大振幅","逆方向振幅","正方向最大振幅");


				using (DBUtils.OpenThreadConnection()) {
					DateTime start = DateTime.Now.AddYears(-5);
					DateTime end = DateTime.Now.AddHours(-1);

					var usdjpy = new CandlesticksGetter() {
						Instrument = "USD_JPY",
						Granularity = "M10",
						Start = start,
						End = end
					}.Execute().ToList();

					Dictionary<int, int> sum = new Dictionary<int, int>();
					for (int t = 0; t < usdjpy.Count(); t++) {
						var c = usdjpy[t];
						var d = Math.Abs(c.Close-c.Open);
						if (d >= 0.2) {
							var c1 = usdjpy[t + 1];
							report.WriteLine(c.IsUp() ? c1.Open - c1.Low : c1.High - c1.Open, (c.IsUp()==c1.IsUp() ? -1 : 1) * Math.Abs(c1.Open-c1.Close),c.IsUp() ? c1.High - c1.Open :c1.Open - c1.Low);
						}
					}
				}
			});
		}

		private void マウステスト_Click(object sender, EventArgs e) {
			new MouseTestForm().Show();
		}

		private void 因果_Click(object sender, EventArgs e) {
			new CauseAndEffectForm().Show();
		}
	}
}
