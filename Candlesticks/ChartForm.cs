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
	public partial class ChartForm : Form {
		private static string[] INSTRUMENTS = { "USD_JPY", "EUR_JPY", "EUR_USD", "US30_USD", "JP225_USD","EU50_EUR" };

		private string instrument = "US30_USD";
		private string granularity = "M1";
		private OandaAPI oandaApi = null;
		private List<OandaCandle> candles;


		public ChartForm() {
			InitializeComponent();
		}

		private void ChartForm_Load(object sender, EventArgs e) {



			oandaApi = new OandaAPI();
			candles = oandaApi.GetCandles(100,instrument,granularity).ToList();

			chart1.Series.Clear();
			chart1.ChartAreas.Clear();
			chart1.Titles.Clear();

			float low = candles.Select(c => c.lowMid).Min();
			float high = candles.Select(c => c.highMid).Max();

			ChartArea chartArea = new ChartArea();
			chart1.ChartAreas.Add(chartArea);
			chartArea.AxisX.LabelStyle.Format = "T";

			double n = 0.1d;
			while(true) {
				double d = Math.Round((high - low) / n);
				if (d <= 2) {
					break;
				}
				n *= 2;
			}

			chartArea.AxisY.Maximum = Math.Ceiling(high / n) * n;
			chartArea.AxisY.Minimum = Math.Floor(low / n) * n;

			Series candleSeries = new Series();
			chart1.Series.Add(candleSeries);
			candleSeries.XValueType = ChartValueType.DateTime;
			candleSeries.ChartType = SeriesChartType.Candlestick;
			foreach(var candle in candles) {
				candleSeries.Points.Add(new DataPoint(candle.DateTime.ToOADate(),new double[] { candle.highMid, candle.lowMid, candle.openMid,  candle.closeMid }));
			}
		}


		private void ChartForm_FormClosed(object sender, FormClosedEventArgs e) {
			if (oandaApi != null) {
				oandaApi.Dispose();
			}
		}

		private void instrument1ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

		}

		private void instrument2ComboBox_SelectedIndexChanged(object sender, EventArgs e) {

		}

		private void candleRadioButton_CheckedChanged(object sender, EventArgs e) {

		}

		private void lineRadioButton_CheckedChanged(object sender, EventArgs e) {

		}
	}
}
