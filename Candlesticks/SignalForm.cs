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
	public partial class SignalForm : Form {
		private List<TimeOfDayPattern> patterns = new List<TimeOfDayPattern>();

		public SignalForm() {
			InitializeComponent();
		}

		private void SignalForm_Load(object sender, EventArgs e) {
			using (DBUtils.OpenThreadConnection()) {
				patterns.Add(new TimeOfDayPattern() {
					CheckStartTime = new TimeOfDayPattern.Time(7, 50),
					CheckEndTime = new TimeOfDayPattern.Time(8, 40),
					IsCheckUp = true,
					TradeStartTime = new TimeOfDayPattern.Time(8, 50),
					TradeEndTime = new TimeOfDayPattern.Time(11, 30),
					TradeType = TradeType.Bid
				});

				patterns.Add(new TimeOfDayPattern() {
					CheckStartTime = new TimeOfDayPattern.Time(0, 10),
					CheckEndTime = new TimeOfDayPattern.Time(4, 30),
					IsCheckUp = false,
					TradeStartTime = new TimeOfDayPattern.Time(4, 30),
					TradeEndTime = new TimeOfDayPattern.Time(7, 10),
					TradeType = TradeType.Ask
				});

				patterns.Add(new TimeOfDayPattern() {
					CheckStartTime = new TimeOfDayPattern.Time(0, 20),
					CheckEndTime = new TimeOfDayPattern.Time(5, 40),
					IsCheckUp = false,
					TradeStartTime = new TimeOfDayPattern.Time(5, 50),
					TradeEndTime = new TimeOfDayPattern.Time(7, 10),
					TradeType = TradeType.Ask
				});

				patterns.Add(new TimeOfDayPattern() {
					CheckStartTime = new TimeOfDayPattern.Time(5, 50),
					CheckEndTime = new TimeOfDayPattern.Time(6, 00),
					IsCheckUp = false,
					TradeStartTime = new TimeOfDayPattern.Time(6, 00),
					TradeEndTime = new TimeOfDayPattern.Time(6, 10),
					TradeType = TradeType.Ask
				});
				this.signalDataGrid.ReadOnly = true;
				this.signalDataGrid.Columns.Add("pattern", "パターン");
				this.signalDataGrid.Columns.Add("match", "マッチ状況");
				this.signalDataGrid.Columns.Add("trade", "トレードシグナル");

				foreach (var pattern in patterns) {
					this.signalDataGrid.Rows.Add();
				}
			}
		}

		private void SignalForm_Shown(object sender, EventArgs e) {
			this.signalDataGrid.CurrentCell = null;
		}

		private void timer1_Tick(object sender, EventArgs e) {
			this.signalDataGrid.CurrentCell = null;
			using (DBUtils.OpenThreadConnection()) {
				int index = 0;
				foreach (var pattern in patterns) {
					this.signalDataGrid.Rows[index].Cells[0].Value = pattern.GetCheckDescription();

					TimeOfDayPattern.Signal signal;
					bool isMatch = false;
					if (pattern.IsMatch(out signal)) {
						this.signalDataGrid.Rows[index].Cells[2].Value = signal.GetTradeDescription();
						this.signalDataGrid.Rows[index].Cells[2].Style.BackColor = signal.IsInTradeTime ? Color.Red : Color.Yellow;
						isMatch = true;
					} else {
						this.signalDataGrid.Rows[index].Cells[2].Value = "";
						this.signalDataGrid.Rows[index].Cells[2].Style.BackColor = Color.White;
						isMatch = false;
					}
					if (signal != null) {
						this.signalDataGrid.Rows[index].Cells[1].Value = signal.GetCheckResultDescription();
						if(isMatch) {
							this.signalDataGrid.Rows[index].Cells[1].Style.BackColor = isMatch ? Color.Yellow : Color.White;
						}
					}
					index++;
				}
				this.signalDataGrid.AutoResizeColumn(0);
				this.signalDataGrid.AutoResizeColumn(1);
				this.signalDataGrid.AutoResizeColumn(2);
			}
		}
	}
}
