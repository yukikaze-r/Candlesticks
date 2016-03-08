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
			EventReceiver.Instance.OrderBookUpdatedEvent += OnOrderBookUpdated;
		}

		private void OnOrderBookUpdated(OrderBookUpdated orderBookUpdated) {
			Invoke(new Action(()=> {
				dateTime = DateTime.Now;
				LoadChart();
			}));
		}

		private DateTime NormalizeDateTime(DateTime t) {
			return new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute / 20 * 20, 0, t.Kind);
		}

		private void LoadChart() {

			using (DBUtils.OpenThreadConnection()) {
				DateTime start = NormalizeDateTime(dateTime.AddDays(-1));

				chart1.Series.Clear();
				chart1.ChartAreas.Clear();
				chart1.Titles.Clear();

				var candles = Candlestick.Aggregate(new CandlesticksGetter() {
					Granularity = "M10",
					Start = start,
					End = start.AddDays(1)
				}.Execute().ToList(), 2).ToList();

				var positions = new OrderBookPricePointDao(
						DBUtils.GetConnection()).GetPositionSummaryGroupByOrderBookDateTime(start, dateTime)
						.Select(p => new Tuple<DateTime,float,float>(NormalizeDateTime(p.Item1),p.Item2,p.Item3)).ToList();

				DateTime startChart = candles[0].DateTime;
				DateTime endChart = candles[candles.Count() - 1].DateTime.AddMinutes(20);
				if(candles.Where(c=>!c.IsNull).Count() >= 1) {
					CreatePositionSmummaryChartArea(positions, startChart, endChart);
					CreateCandlesticksChartArea(candles, positions, startChart, endChart);
				}




				nextDayButton.Enabled = dateTime.AddDays(1) <= DateTime.Now;
				dateLabel.Text = dateTime.ToString("M/dd");


			}
		}

		private List<DateTime> SearchOpenStartPositions(IEnumerable<Tuple<DateTime,float,float>> positions) {
			bool isClosed = true;
			List<DateTime> result = new List<DateTime>();
			foreach(var p in positions) {
				if(p.Item2 + p.Item3 >= 99.9f) {
					isClosed = true;
				} else if(isClosed) {
					result.Add(p.Item1);
					isClosed = false;
				}
			}
			return result;
		}

		private void CreateCandlesticksChartArea(List<Candlestick> candles, List<Tuple<DateTime, float, float>> summary,  DateTime start, DateTime end) {
			ChartArea chartArea = new ChartArea("candlesticks");
			chart1.ChartAreas.Add(chartArea);

			float low = candles.Where(c=>!c.IsNull).Select(c => c.Low).Min();

			float high = candles.Where(c => !c.IsNull).Select(c => c.High).Max();

			double n = 0.1d;
			while (true) {
				double d = Math.Round((high - low) / n);
				if (d <= 2) {
					break;
				}
				n *= 2;
			}
			chartArea.AxisX.LabelStyle.Enabled = false;
			chartArea.AxisX.MajorGrid.Enabled = true;
			chartArea.AxisX.MajorGrid.IntervalType = DateTimeIntervalType.Hours;
			chartArea.AxisX.MajorGrid.Interval = 1;
			chartArea.AxisX.MinorGrid.Enabled = true;
			chartArea.AxisX.MinorGrid.IntervalType = DateTimeIntervalType.Minutes;
			chartArea.AxisX.MinorGrid.Interval = 20;
			chartArea.AxisX.MinorGrid.LineColor = Color.LightGray;
			chartArea.AxisX.Minimum = start.ToOADate();
			chartArea.AxisX.Maximum = end.ToOADate();

			chartArea.AxisY.Maximum = Math.Ceiling(high / n) * n;
			chartArea.AxisY.Minimum = Math.Floor(low / n) * n;


			Series openStartSeries = new Series();
			chart1.Series.Add(openStartSeries);
			openStartSeries.ChartArea = "candlesticks";
			openStartSeries.ChartType = SeriesChartType.RangeColumn;
			openStartSeries.SetCustomProperty("PixelPointWidth", "2");
			foreach (var dateTime in SearchOpenStartPositions(summary)) {
				openStartSeries.Points.Add(new DataPoint(dateTime.ToOADate(), new double[] { chartArea.AxisY.Minimum, chartArea.AxisY.Maximum }));
			}

			Series candleSeries = new Series();
			chart1.Series.Add(candleSeries);
			candleSeries.ChartArea = "candlesticks";
			candleSeries.XValueType = ChartValueType.DateTime;
			candleSeries.ChartType = SeriesChartType.Candlestick;
			candleSeries.SetCustomProperty("PriceUpColor", "red");
			candleSeries.SetCustomProperty("PriceDownColor", "blue");
			foreach (var candle in candles) {
				if(candle.IsNull) {
					continue;
				}
				DateTime t = candle.DateTime.AddMinutes(10);
//				Console.WriteLine(String.Format("{0} {1} {2} {3} {4}",t, candle.High, candle.Low, candle.Open, candle.Close));
				candleSeries.Points.Add(new DataPoint(t.ToOADate(), new double[] {
					candle.High, candle.Low, candle.Open, candle.Close }) { Color = candle.IsUp() ? Color.Red : Color.Blue });
			}

			chartArea.AlignWithChartArea = "position_summary";
		}

		private void CreatePositionSmummaryChartArea(List<Tuple<DateTime, float, float>> summary, DateTime start, DateTime end) {
			ChartArea chartArea = new ChartArea("position_summary");
			chart1.ChartAreas.Add(chartArea);


			chartArea.AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Hours;
			chartArea.AxisX.LabelStyle.Interval = 1;
			chartArea.AxisX.MajorGrid.Enabled = true;
			chartArea.AxisX.MajorGrid.IntervalType = DateTimeIntervalType.Hours;
			chartArea.AxisX.MajorGrid.Interval = 1;
			chartArea.AxisX.MinorGrid.Enabled = true;
			chartArea.AxisX.MinorGrid.IntervalType = DateTimeIntervalType.Minutes;
			chartArea.AxisX.MinorGrid.Interval = 20;
			chartArea.AxisX.MinorGrid.LineColor = Color.LightGray;
			chartArea.AxisX.Minimum = start.ToOADate();
			chartArea.AxisX.Maximum = end.ToOADate();

			chartArea.AxisY.Maximum = 44;
			chartArea.AxisY.Minimum = 30;
			chartArea.AxisY.MajorGrid.Enabled = true;
			chartArea.AxisY.MajorGrid.Interval = 5;
			chartArea.AxisY.MinorGrid.Enabled = true;
			chartArea.AxisY.MinorGrid.Interval = 1;
			chartArea.AxisY.MinorGrid.LineColor = Color.LightGray;
			//				chartArea.AxisY.Su

			chartArea.AxisY2.Enabled = AxisEnabled.True;
			chartArea.AxisY2.Maximum = 70;
			chartArea.AxisY2.Minimum = 56;
			chartArea.AxisY2.MajorGrid.Enabled = false;
			chartArea.AxisY2.MinorGrid.Enabled = false;
			chartArea.AxisY2.IsReversed = true;

			Series openStartSeries = new Series();
			chart1.Series.Add(openStartSeries);
			openStartSeries.ChartType = SeriesChartType.RangeColumn;
			openStartSeries.SetCustomProperty("PixelPointWidth", "2");
			foreach (var dateTime in SearchOpenStartPositions(summary)) {
				openStartSeries.Points.Add(new DataPoint(dateTime.ToOADate(), new double[] { chartArea.AxisY.Minimum, chartArea.AxisY.Maximum }));
			}

			Series shortPositionsSeries = new Series();
			chart1.Series.Add(shortPositionsSeries);
			shortPositionsSeries.ChartArea = "position_summary";
			shortPositionsSeries.ChartType = SeriesChartType.Line;
			shortPositionsSeries.YAxisType = AxisType.Primary;
			shortPositionsSeries.XValueType = ChartValueType.DateTime;
			shortPositionsSeries.Color = Color.Blue;
			shortPositionsSeries.LegendText = "short";

			Series longPositionsSeries = new Series();
			chart1.Series.Add(longPositionsSeries);
			longPositionsSeries.ChartArea = "position_summary";
			longPositionsSeries.ChartType = SeriesChartType.Line;
			longPositionsSeries.YAxisType = AxisType.Secondary;
			longPositionsSeries.XValueType = ChartValueType.DateTime;
			longPositionsSeries.Color = Color.Red;
			longPositionsSeries.LegendText = "long";

			foreach (var s in summary) {
				//				chartArea.AxisX.LabelStyle.Format = "HH";
				DateTime t = s.Item1;
				shortPositionsSeries.Points.Add(new DataPoint(t.ToOADate(), s.Item2));
				longPositionsSeries.Points.Add(new DataPoint(t.ToOADate(), s.Item3) {
					AxisLabel = s.Item1.ToString(t.Hour == 0 ? "M/d" : "%H")
				});
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

		private void PositionsForm_FormClosing(object sender, FormClosingEventArgs e) {
			EventReceiver.Instance.OrderBookUpdatedEvent -= OnOrderBookUpdated;
		}
	}
}
