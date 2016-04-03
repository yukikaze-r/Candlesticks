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



		private List<TradePattern> patterns = new List<TradePattern>();


		private HashSet<TimeTradeOrder> havePositionSet = new HashSet<TimeTradeOrder>();

		public SignalForm() {
			InitializeComponent();
		}

		private void SignalForm_Load(object sender, EventArgs e) {
			using (DBUtils.OpenThreadConnection()) {
				// 64.19%
				patterns.Add(
					new TradePattern() {
						TradeCondition = new TimeOfDayPattern() {
							Instrument = "USD_JPY",
							CheckStartTime = new TimeSpan(7, 50, 0),
							CheckEndTime = new TimeSpan(8, 40, 0),
							IsCheckUp = true,
						},
						TradeOrders = new TradeOrders(
							new TimeTradeOrder() {
								Instrument = "USD_JPY",
								TradeType = TradeType.Bid,
								Time = new TimeSpan(8, 50, 0),
							},
							new TimeTradeOrder() {
								Instrument = "USD_JPY",
								TradeType = TradeType.Settle,
								Time = new TimeSpan(11, 30, 0),
							}
						)
					}
				);

				// 75.76%
				patterns.Add(
					new TradePattern() {
						TradeCondition = new TimeOfDayPattern() {
							Instrument = "USD_JPY",
							CheckStartTime = new TimeSpan(0, 30, 0),
							CheckEndTime = new TimeSpan(4, 50, 0),
							IsCheckUp = false,
						},
						TradeOrders = new TradeOrders(
							new TimeTradeOrder() {
								Instrument = "USD_JPY",
								TradeType = TradeType.Ask,
								Time = new TimeSpan(4, 50, 0),
							},
							new TimeTradeOrder() {
								Instrument = "USD_JPY",
								TradeType = TradeType.Settle,
								Time = new TimeSpan(7, 10, 0),
							}
						)
					}
				);

				//73.78%
				patterns.Add(
					new TradePattern() {
						TradeCondition = new TimeOfDayPattern() {
							Instrument = "USD_JPY",
							CheckStartTime = new TimeSpan(0, 20, 0),
							CheckEndTime = new TimeSpan(5, 40, 0),
							IsCheckUp = false,
						},
						TradeOrders = new TradeOrders(
							new TimeTradeOrder() {
								Instrument = "USD_JPY",
								TradeType = TradeType.Ask,
								Time = new TimeSpan(5, 49, 0),
							},
							new TimeTradeOrder() {
								Instrument = "USD_JPY",
								TradeType = TradeType.Settle,
								Time = new TimeSpan(7, 10, 0),
							}
						)
					}
				);


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

				// 62.87%
				patterns.Add(
					new TradePattern() {
						TradeCondition = new TimeOfDayPattern() {
							Instrument = "USD_JPY",
							CheckStartTime = new TimeSpan(1, 30, 0),
							CheckEndTime = new TimeSpan(4, 20, 0),
							IsCheckUp = false,
						},
						TradeOrders = new TradeOrders(
							new TimeTradeOrder() {
								Instrument = "USD_JPY",
								TradeType = TradeType.Bid,
								Time = new TimeSpan(9, 50, 0),
							},
							new TimeTradeOrder() {
								Instrument = "USD_JPY",
								TradeType = TradeType.Settle,
								Time = new TimeSpan(13, 10, 0),
							}
						)
					}
				);

				// 64.54% ... A

				patterns.Add(
					new TradePattern() {
						TradeCondition = new TimeOfDayPattern() {
							Instrument = "EUR_USD",
							CheckStartTime = new TimeSpan(8, 50, 0),
							CheckEndTime = new TimeSpan(11, 00, 0),
							IsCheckUp = false,
						},
						TradeOrders = new TradeOrders(
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
						)
					}
				);



				//62.9% ... B
				patterns.Add(
					new TradePattern() {
						TradeCondition = new TimeOfDayPattern() {
							Instrument = "EUR_USD",
							CheckStartTime = new TimeSpan(6, 50, 0),
							CheckEndTime = new TimeSpan(7, 50, 0),
							IsCheckUp = true,
						},
						TradeOrders = new TradeOrders(
							new TimeTradeOrder() {
								Instrument = "EUR_USD",
								TradeType = TradeType.Ask,
								Time = new TimeSpan(11, 00, 0),
							},
							new TimeTradeOrder() {
								Instrument = "EUR_USD",
								TradeType = TradeType.Settle,
								Time = new TimeSpan(11, 30, 0),
							}
						)
					}
				);
				// 11:00-11:40の上昇確率
				// !A && !B => 42.86%
				// A && !B => 57.76%
				// !A && B => 53.82%
				// A && B => 68.81%

				patterns.Add(
					new TradePattern() {
						TradeCondition = new TradeConditionAnd() {
							TradeConditions = new TradeCondition[] {
								new TimeOfDayPattern() {
									Instrument = "EUR_USD",
									CheckStartTime = new TimeSpan(6, 50, 0),
									CheckEndTime = new TimeSpan(7, 50, 0),
									IsCheckUp = true,
								},
								new TimeOfDayPattern() {
									Instrument = "EUR_USD",
									CheckStartTime = new TimeSpan(8, 50, 0),
									CheckEndTime = new TimeSpan(11, 00, 0),
									IsCheckUp = false,
								},
							},
						},

						TradeOrders = new TradeOrders(
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
						)
					}
				);



				//63.98%
				patterns.Add(
					new TradePattern() {
						TradeCondition = new TimeOfDayPattern() {
							Instrument = "EUR_USD",
							CheckStartTime = new TimeSpan(0, 50, 0),
							CheckEndTime = new TimeSpan(4, 00, 0),
							IsCheckUp = true,
						},
						TradeOrders = new TradeOrders(
							new TimeTradeOrder() {
								Instrument = "EUR_USD",
								TradeType = TradeType.Bid,
								Time = new TimeSpan(6, 20, 0),
							},
							new TimeTradeOrder() {
								Instrument = "EUR_USD",
								TradeType = TradeType.Settle,
								Time = new TimeSpan(7, 40, 0),
							}
						)
					}
				);

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
					cells["pattern"].Value = pattern.TradeCondition.GetCheckDescription();

					Signal signal;
					bool isMatch = false;

					TradeContext tradeContext = new TradeContext() {
						Instrument = pattern.TradeOrders[0].Instrument,
						Date = DateTime.Today
					};

					cells["trade"].Value = pattern.TradeOrders.GetTradeDescription(tradeContext);
					if (pattern.TradeCondition.IsMatch(out signal, tradeContext)) {
						pattern.TradeOrders.DoTrade(tradeContext);
						if (tradeContext.Profit > 0f) {
							cells["trade"].Style.BackColor = Color.LightPink;
						} else {
							cells["trade"].Style.BackColor = pattern.TradeOrders.IsInTradeTime ? Color.Red : Color.Yellow;
						}
						isMatch = true;
					} else {
						cells["trade"].Style.BackColor = Color.White;
						isMatch = false;
					}

					if (signal != null) {
						if (isMatch) {
							cells["match"].Value = "Matched!" + signal.GetCheckResultDescription();
							cells["match"].Style.BackColor = Color.Yellow;
							var isAutoTrade = cells["autoTrade"].Value;
							if (isAutoTrade != null && (bool)isAutoTrade) {
								DoTrade(pattern.TradeOrders);
							}
						} else {
							if (signal.IsCheckFinished) {
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

		private void DoTrade(TradeOrders orders) {
			DateTime now = DateTime.Now;
			DateTime nowMinutes = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, now.Kind);
			foreach (var order in orders) {
				var mouseClickPosition = GetMouseClickPosition(order.Instrument);

				if (order.TradeType == TradeType.Settle) {
					if (DateTime.Today.Add(order.Time) == nowMinutes && now.Second % 10 == randomSecond) {
						Cursor.Position = mouseClickPosition.Settle;
						mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
						mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
					}
				} else {
					if (DateTime.Today.Add(order.Time) == nowMinutes && !havePositionSet.Contains(order)) {
						Cursor.Position = order.TradeType == TradeType.Ask ? mouseClickPosition.Ask : mouseClickPosition.Bid;
						mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
						mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
						havePositionSet.Add(order);
					}
				}

			}

		}

		private Setting.MouseClikPositoin GetMouseClickPosition(string instrument) {
			switch (instrument) {
				case "USD_JPY":
					return Setting.Instance.MouseClickPositionUSD_JPY;
				case "EUR_USD":
					return Setting.Instance.MouseClickPositionEUR_USD;
				default:
					throw new Exception("Unknown Instrument:" + instrument);
			}
		}
	}
}
