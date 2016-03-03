using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Candlesticks {
	public partial class PositionsForm : Form {
		private DateTime dateTime;

		public PositionsForm() {
			InitializeComponent();
		}

		private void PositionsForm_Load(object sender, EventArgs e) {
			dateTime = DateTime.Now;
			LoadChart();
		}

		private void LoadChart() {

			using (DBUtils.OpenThreadConnection()) {
				DateTime start = dateTime.AddDays(-1);
				DateTime end = dateTime;
				var summary = new OrderBookPricePointDao(DBUtils.GetConnection()).GetPositionSummaryGroupByOrderBookDateTime(start, end).ToList();

				chart1.Series.Clear();
				chart1.ChartAreas.Clear();
				chart1.Titles.Clear();

				ChartArea chartArea = new ChartArea(); ;
				chart1.ChartAreas.Add(chartArea);

				chartArea.AxisX.LabelStyle.Format = "HH";
				chartArea.AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Hours;
				chartArea.AxisX.LabelStyle.Interval = 1;
				chartArea.AxisX.MajorGrid.Enabled = true;
				chartArea.AxisX.MajorGrid.IntervalType = DateTimeIntervalType.Hours;
				chartArea.AxisX.MajorGrid.Interval = 1;
				chartArea.AxisX.MinorGrid.Enabled = true;
				chartArea.AxisX.MinorGrid.IntervalType = DateTimeIntervalType.Minutes;
				chartArea.AxisX.MinorGrid.Interval = 20;
				chartArea.AxisX.MinorGrid.LineColor = Color.LightGray;

				chartArea.AxisY.Maximum = 42;
				chartArea.AxisY.Minimum = 32;
				chartArea.AxisY.MajorGrid.Enabled = true;
				chartArea.AxisY.MajorGrid.Interval = 5;
				chartArea.AxisY.MinorGrid.Enabled = true;
				chartArea.AxisY.MinorGrid.Interval = 1;
				chartArea.AxisY.MinorGrid.LineColor = Color.LightGray;
				//				chartArea.AxisY.Su

				chartArea.AxisY2.Enabled = AxisEnabled.True;
				chartArea.AxisY2.Maximum = 68;
				chartArea.AxisY2.Minimum = 58;
				chartArea.AxisY2.MajorGrid.Enabled = false;
				chartArea.AxisY2.MinorGrid.Enabled = false;
				chartArea.AxisY2.IsReversed = true;

				Series shortPositionsSeries = new Series();
				chart1.Series.Add(shortPositionsSeries);
				shortPositionsSeries.ChartType = SeriesChartType.Line;
				shortPositionsSeries.YAxisType = AxisType.Primary;
				shortPositionsSeries.XValueType = ChartValueType.DateTime;
				shortPositionsSeries.Color = Color.Blue;
				shortPositionsSeries.LegendText = "short";

				Series longPositionsSeries = new Series();
				chart1.Series.Add(longPositionsSeries);
				longPositionsSeries.ChartType = SeriesChartType.Line;
				longPositionsSeries.YAxisType = AxisType.Secondary;
				longPositionsSeries.XValueType = ChartValueType.DateTime;
				longPositionsSeries.Color = Color.Red;
				longPositionsSeries.LegendText = "long";

				foreach (var s in summary) {
					shortPositionsSeries.Points.Add(new DataPoint(s.Item1.ToOADate(), s.Item2));
					longPositionsSeries.Points.Add(new DataPoint(s.Item1.ToOADate(), s.Item3));
				}
				nextDayButton.Enabled = dateTime.AddDays(1) <= DateTime.Now;
				dateLabel.Text = dateTime.ToString("M/dd");
			}
		}

		private void previousDayButton_Click(object sender, EventArgs e) {
			dateTime = dateTime.AddDays(-1);
			LoadChart();
		}

		private void nextDayButton_Click(object sender, EventArgs e) {
			dateTime = dateTime.AddDays(1);
			LoadChart();
		}
	}
}
