using Npgsql;
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
	public partial class OrderBookForm : Form {
		private NpgsqlConnection connection = null;
		private DateTime latestTime;
		private List<OrderBookDao.Entity> orderBooks;

		public OrderBookForm() {
			InitializeComponent();
		}

		private void chart1_Click(object sender, EventArgs e) {

		}

		private void OrderBookForm_Load(object sender, EventArgs ev) {
			LoadOrderBookList();

			orderBookList.SelectedIndex = 0;

//			LoadAllChart();

			splitContainer1.Panel2.HorizontalScroll.Value = splitContainer1.Panel1.HorizontalScroll.Value
				= (splitContainer1.Panel1.HorizontalScroll.Maximum + splitContainer1.Panel1.HorizontalScroll.Minimum - splitContainer1.Panel1.ClientSize.Width) / 2;

			
		}

		private void LoadOrderBookList() {
			connection = DBUtils.OpenConnection();
			orderBookList.Items.Clear();
			orderBooks = new List<OrderBookDao.Entity>();
			foreach (var entity in new OrderBookDao(connection).GetAllOrderByDateTimeDescendant()) {
				orderBookList.Items.Add(entity.DateTime);
				orderBooks.Add(entity);
			}
		}

		private void LoadAllChart() {
			Console.WriteLine("LoadAllChart");

			DateTime current = DateTime.UtcNow;
			var data = new OandaAPI().GetOrderbookData(3600);

			DateTime[] times = data.Keys.OrderByDescending(t => t).Take(2).ToArray();
			LoadChart(chart1, times[0], data[times[0]]);
			LoadChart(chart2, times[1], data[times[1]]);

		}

		private void LoadChart(Chart chart, DateTime dateTime, PricePoints pricePoints) {

			chart.Series.Clear();
			chart.ChartAreas.Clear();
			chart.Titles.Clear();

			chart.Size = new Size(10000, 400);

			chart.Titles.Add(new Title(dateTime.ToString()));

			ChartArea chartArea = new ChartArea();
			chartArea.AxisX.Interval = 0.05d;
			chartArea.AxisX.Minimum = 100f;
			chartArea.AxisX.Maximum= 125f;
			chartArea.AxisY.Maximum = 5.0f;
			chartArea.AxisY.Minimum = -1.0f;
			chartArea.AxisY.Interval = 1.0f;


			chart.ChartAreas.Add(chartArea);


			Series seriesRate = new Series();
			seriesRate.ChartType = SeriesChartType.Column;
			var dataPoint = new DataPoint(pricePoints.rate, 5.0f);
			seriesRate.SetCustomProperty("PointWidth", "0.01");
			//			dataPoint["PointWidth"] = "1.0";
			seriesRate.Points.Add(dataPoint);
			seriesRate.Color = Color.Orange;
			chart.Series.Add(seriesRate);

			Series seriesDeltaPos = new Series();
			seriesDeltaPos.ChartType = SeriesChartType.Column;
			seriesDeltaPos.LegendText = "buy - sell position";
			seriesDeltaPos.Color = Color.LightGreen;
			foreach (var price in pricePoints.price_points.Keys.OrderBy(k => k)) {
				var dprice = decimal.ToDouble((decimal)price);
				var p = pricePoints.price_points[price];
				seriesDeltaPos.Points.Add(new DataPoint(dprice, p.pl - p.ps));
			}
			chart.Series.Add(seriesDeltaPos);

			Series[] lines = new Series[4];
			string[] titles = new string[] { "order_long", "order_short", "pos_long", "pos_short" };
			Color[] colors = new Color[] { Color.Pink, Color.LightBlue, Color.Red, Color.Blue};
			for (int i=0; i<4; i++) {
				Series seriesLine = new Series();
				chart.Series.Add(seriesLine);

				seriesLine.ChartType = SeriesChartType.Line;
				seriesLine.LegendText = titles[i];
				seriesLine.BorderWidth = 1;
				seriesLine.Color = colors[i];
				seriesLine.MarkerStyle = MarkerStyle.Circle;
				seriesLine.MarkerSize = 2;
				lines[i] = seriesLine;
			}
			foreach (var price in pricePoints.price_points.Keys.OrderBy(k => k)) {
				var dprice = decimal.ToDouble((decimal)price);
				var p = pricePoints.price_points[price];
				lines[0].Points.Add(new DataPoint(dprice, p.ol));
				lines[1].Points.Add(new DataPoint(dprice, p.os));
				lines[2].Points.Add(new DataPoint(dprice, p.pl));
				lines[3].Points.Add(new DataPoint(dprice, p.ps));
			}



		}

		private void splitContainer1_Panel1_Scroll(object sender, ScrollEventArgs e) {
			splitContainer1.Panel2.HorizontalScroll.Value = splitContainer1.Panel1.HorizontalScroll.Value;
		}

		private void splitContainer1_Panel2_Scroll(object sender, ScrollEventArgs e) {
			splitContainer1.Panel1.HorizontalScroll.Value = splitContainer1.Panel2.HorizontalScroll.Value;
		}


		protected override Point ScrollToControl(Control activeControl)
		{
		    return splitContainer1.Panel1.AutoScrollPosition;
		}

		private void timer1_Tick(object sender, EventArgs e) {
			DateTime now = DateTime.Now;
			if(now.Minute % 20 == 1 && now > latestTime.AddMinutes(1)) {
				latestTime = now;
				LoadAllChart();
			}
		}
		
		private void LoadChart(Chart chart, OrderBookDao.Entity orderBook) {
			PricePoints pricePoints = new PricePoints();
			pricePoints.rate = orderBook.Rate;
			pricePoints.price_points = new Dictionary<float, PricePoint>();
			foreach (var pp in orderBook.GetPricePointsOrderByPrice()) {
				pricePoints.price_points[pp.Price] = new PricePoint() {
					os = pp.Os,
					ol = pp.Ol,
					ps = pp.Ps,
					pl = pp.Pl
				};
			}
			LoadChart(chart, orderBook.DateTime, pricePoints);
		}

		private void orderBookList_SelectedIndexChanged(object sender, EventArgs e) {
			LoadChart(chart1,orderBooks[orderBookList.SelectedIndex]);
			if(orderBookList.SelectedIndex+1 < orderBooks.Count()) {
				LoadChart(chart2, orderBooks[orderBookList.SelectedIndex + 1]);
			}
		}

		private void OrderBookForm_FormClosing(object sender, FormClosingEventArgs e) {
			if(connection != null) {
				connection.Close();
				connection = null;
			}
		}
	}
}
