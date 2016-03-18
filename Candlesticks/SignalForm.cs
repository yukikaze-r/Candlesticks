using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Candlesticks {
	public partial class SignalForm : Form {
		private static int randomSecond = new Random().Next() % 10;

		[DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
		static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		private const int MOUSEEVENTF_LEFTDOWN = 0x2;
		private const int MOUSEEVENTF_LEFTUP = 0x4;



		private List<TimeOfDayPattern> patterns = new List<TimeOfDayPattern>();
		private HashSet<TimeOfDayPattern> havePositionSet = new HashSet<TimeOfDayPattern>();
		private HashSet<TimeOfDayPattern> haveSettleSet = new HashSet<TimeOfDayPattern>();

		public SignalForm() {
			InitializeComponent();
		}

		private void SignalForm_Load(object sender, EventArgs e) {
			using (DBUtils.OpenThreadConnection()) {
				patterns.Add(new TimeOfDayPattern() {
					Instrument = "USD_JPY",
					CheckStartTime = new TimeOfDayPattern.Time(7, 50),
					CheckEndTime = new TimeOfDayPattern.Time(8, 40),
					IsCheckUp = true,
					TradeStartTime = new TimeOfDayPattern.Time(8, 50),
					TradeEndTime = new TimeOfDayPattern.Time(11, 30),
					TradeType = TradeType.Bid
				});

				patterns.Add(new TimeOfDayPattern() {
					Instrument = "USD_JPY",
					CheckStartTime = new TimeOfDayPattern.Time(0, 10),
					CheckEndTime = new TimeOfDayPattern.Time(4, 30),
					IsCheckUp = false,
					TradeStartTime = new TimeOfDayPattern.Time(4, 30),
					TradeEndTime = new TimeOfDayPattern.Time(7, 10),
					TradeType = TradeType.Ask
				});

				patterns.Add(new TimeOfDayPattern() {
					Instrument = "USD_JPY",
					CheckStartTime = new TimeOfDayPattern.Time(0, 20),
					CheckEndTime = new TimeOfDayPattern.Time(5, 40),
					IsCheckUp = false,
					TradeStartTime = new TimeOfDayPattern.Time(5, 49),
					TradeEndTime = new TimeOfDayPattern.Time(7, 10),
					TradeType = TradeType.Ask
				});
				/*
				patterns.Add(new TimeOfDayPattern() {
					Instrument = "USD_JPY",
					CheckStartTime = new TimeOfDayPattern.Time(5, 50),
					CheckEndTime = new TimeOfDayPattern.Time(6, 00),
					IsCheckUp = false,
					TradeStartTime = new TimeOfDayPattern.Time(6, 00),
					TradeEndTime = new TimeOfDayPattern.Time(6, 10),
					TradeType = TradeType.Ask
				});*/


				patterns.Add(new TimeOfDayPattern() {
					Instrument = "EUR_USD",
					CheckStartTime = new TimeOfDayPattern.Time(8, 50),
					CheckEndTime = new TimeOfDayPattern.Time(11, 00),
					IsCheckUp = false,
					TradeStartTime = new TimeOfDayPattern.Time(11, 00),
					TradeEndTime = new TimeOfDayPattern.Time(11, 40),
					TradeType = TradeType.Ask
				});

				patterns.Add(new TimeOfDayPattern() {
					Instrument = "EUR_USD",
					CheckStartTime = new TimeOfDayPattern.Time(0, 50),
					CheckEndTime = new TimeOfDayPattern.Time(4, 00),
					IsCheckUp = true,
					TradeStartTime = new TimeOfDayPattern.Time(6, 20),
					TradeEndTime = new TimeOfDayPattern.Time(7, 40),
					TradeType = TradeType.Bid
				});
				/*
				patterns.Add(new TimeOfDayPattern() {
					CheckStartTime = new TimeOfDayPattern.Time(1, 50),
					CheckEndTime = new TimeOfDayPattern.Time(3, 20),
					IsCheckUp = true,
					TradeStartTime = new TimeOfDayPattern.Time(3, 20),
					TradeEndTime = new TimeOfDayPattern.Time(3, 22),
					TradeType = TradeType.Ask
				});*/

				this.signalDataGrid.Columns.Add("pattern", "パターン");
				this.signalDataGrid.Columns.Add("match", "マッチ状況");
				this.signalDataGrid.Columns.Add("trade", "トレード");

				this.signalDataGrid.Columns.Add(new DataGridViewCheckBoxColumn() {
					Name = "autoTrade",
					HeaderText = "自動取引"
				});

				foreach (var pattern in patterns) {
					this.signalDataGrid.Rows.Add();
				}
			}
		}

		private void SignalForm_Shown(object sender, EventArgs e) {
			this.signalDataGrid.CurrentCell = null;
		}

		private void timer1_Tick(object sender, EventArgs e) {
//			this.signalDataGrid.CurrentCell = null;
			using (DBUtils.OpenThreadConnection()) {
				int index = 0;
				this.signalDataGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
				foreach (var pattern in patterns) {
					var cells = this.signalDataGrid.Rows[index].Cells;
					cells["pattern"].Value = pattern.GetCheckDescription();

					TimeOfDayPattern.Signal signal;
					bool isMatch = false;
					if (pattern.IsMatch(out signal)) {
						cells["trade"].Style.BackColor = signal.IsInTradeTime ? Color.Red : Color.Yellow;
						isMatch = true;
					} else {
						cells["trade"].Style.BackColor = Color.White;
						isMatch = false;
					}
					cells["trade"].Value = pattern.GetTradeDescription();
					if (signal != null) {
						if(isMatch) {
							cells["match"].Value = "Matched!" + signal.GetCheckResultDescription();
							cells["match"].Style.BackColor = Color.Yellow;
							var isAutoTrade = cells["autoTrade"].Value;
							if (isAutoTrade!=null && (bool)isAutoTrade) {
								DoTrade(pattern);
							}
						} else {
							if(signal.IsCheckFinished) {
								cells["match"].Style.BackColor = Color.LightGray;
							}
							cells["match"].Value = "Not Match " + signal.GetCheckResultDescription();
						}
					} else {
						cells["match"].Value = "Not Match";
					}
					index++;
				}
				this.signalDataGrid.AutoResizeColumn(0);
				this.signalDataGrid.AutoResizeColumn(1);
				this.signalDataGrid.AutoResizeColumn(2);
			}
		}

		private void DoTrade(TimeOfDayPattern pattern) {
			DateTime now = DateTime.Now;
			DateTime nowMinutes = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, now.Kind);
			var mouseClickPosition = GetMouseClickPosition(pattern.Instrument);
			if (pattern.TradeStartTime.Todays == nowMinutes && !havePositionSet.Contains(pattern)) {
				Cursor.Position = pattern.TradeType == TradeType.Ask ? mouseClickPosition.Ask : mouseClickPosition.Bid;
				mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
				mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
				havePositionSet.Add(pattern);
			}
			if(pattern.TradeEndTime.Todays == nowMinutes && now.Second % 10 == randomSecond) {
				Cursor.Position = mouseClickPosition.Settle;
				mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
				mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
			}
		}

		private Setting.MouseClikPositoin GetMouseClickPosition(string instrument) {
			switch(instrument) {
				case "USD_JPY":
					return Setting.Instance.MouseClickPositionUSD_JPY;
				case "EUR_USD":
					return Setting.Instance.MouseClickPositionEUR_USD;
				default:
					throw new Exception("Unknown Instrument:"+instrument);
			}
		}
	}
}
