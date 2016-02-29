using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Candlesticks {
	public partial class OrderBookForm : Form {
		private NpgsqlConnection connection = null;
		private TcpClient tcpClient = null;
		private List<OrderBookDao.Entity> orderBooks;
		private HttpClient priceStreamClient = null;
		private Series streamPriceSeries = null;
		private float latestPrice;

		public OrderBookForm() {
			InitializeComponent();
		}

		
		private void chart1_Click_1(object sender, EventArgs e) {
			
		}

		private void OrderBookForm_Load(object sender, EventArgs ev) {
			LoadOrderBookList();

			orderBookList.SelectedIndex = 0;

			splitContainer1.Panel2.HorizontalScroll.Value = splitContainer1.Panel1.HorizontalScroll.Value
				= (splitContainer1.Panel1.HorizontalScroll.Maximum + splitContainer1.Panel1.HorizontalScroll.Minimum - splitContainer1.Panel1.ClientSize.Width) / 2;

			new Thread(new ThreadStart(ReceiveEvent)).Start();

			priceStreamClient = new OandaAPI().GetPrices(ReceivePrice);
		}

		private void ReceivePrice(float bid, float ask) {
			Invoke(new Action(() => {
				if (streamPriceSeries != null) {
					latestPrice = (bid + ask) / 2;
					streamPriceSeries.Points[0].XValue = latestPrice;
					streamPriceSeries.Points[1].XValue = latestPrice;
				}
			}));
		}

		private void ReceiveEvent() {
			Console.WriteLine("receive start");
			tcpClient = new TcpClient("localhost", 4444);
			var reader = new BinaryReader(tcpClient.GetStream());

			try {
				while (true) {
					int length = reader.ReadInt32();
					byte[] body = reader.ReadBytes(length);
					using(var memStream = new MemoryStream(body)) {
						DataContractSerializer ser = new DataContractSerializer(typeof(ServerEvent), new Type[] { typeof(OrderBookUpdated) });
						var receivedObject = ((ServerEvent) ser.ReadObject(memStream)).Body;
						Console.WriteLine("received: " + receivedObject);
						if (receivedObject is OrderBookUpdated) {
							var orderBookUpdated = (OrderBookUpdated)receivedObject;
							Invoke(new Action(() => {
								int selectedIndex = orderBookList.SelectedIndex;
								LoadOrderBookList(orderBookUpdated.DateTime);
								if (selectedIndex == 0) {
									orderBookList.SelectedIndex = 0;
								}
							}));
						}
					}

				}

			} catch(Exception e) {
				Console.WriteLine(e);
			}
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

		private void LoadOrderBookList(DateTime dateTime) {
			if(orderBooks.Count(e=>e.DateTime==dateTime) == 0) {
				var entity = new OrderBookDao(connection).GetByDateTime(dateTime);
				orderBookList.Items.Insert(0,entity.DateTime);
				orderBooks.Insert(0, entity);
			}
		}
		
		private void LoadChart(Chart chart, DateTime dateTime, PricePoints pricePoints, PricePoints previousPricePoints, bool hasStreamPriceSeries) {

			chart.Series.Clear();
			chart.ChartAreas.Clear();
			chart.Titles.Clear();

			chart.Size = new Size(10000, 400);


			ChartArea chartArea = new ChartArea();
			chartArea.AxisX.Interval = 0.05d;
			chartArea.AxisX.Minimum = 100f;
			chartArea.AxisX.Maximum= 125f;
			chartArea.AxisX.LabelStyle.Format = "f2";
			chartArea.AxisX.MajorGrid.Enabled = true;
			chartArea.AxisX.MajorGrid.Interval = 0.1d;
			chartArea.AxisX.MinorGrid.Enabled = true;
			chartArea.AxisX.MinorGrid.LineColor = Color.LightGray;
			chartArea.AxisX.MinorGrid.Interval = 0.05d;
			chartArea.AxisY.Maximum = 5.0f;
			chartArea.AxisY.Minimum = 0f;
			chartArea.AxisY.Interval = 1.0f;
			chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

			chartArea.AxisY2.Enabled = AxisEnabled.True;
			chartArea.AxisY2.Maximum = 2.5f;
			chartArea.AxisY2.Minimum = -2.5f;
			chartArea.AxisY2.Interval = 1.0f;

			chart.ChartAreas.Add(chartArea);



			Series seriesDeltaPos = new Series();
			chart.Series.Add(seriesDeltaPos);
			seriesDeltaPos.YAxisType = AxisType.Secondary;
			seriesDeltaPos.ChartType = SeriesChartType.StackedColumn;
			seriesDeltaPos.LegendText = "buy - sell position";
			seriesDeltaPos.Color = Color.FromArgb(100, 0, 255, 0);


			Series seriesPreviousDeltaPos = new Series();
			chart.Series.Add(seriesPreviousDeltaPos);
			seriesPreviousDeltaPos.YAxisType = AxisType.Secondary;
			seriesPreviousDeltaPos.ChartType = SeriesChartType.StackedColumn;
			seriesPreviousDeltaPos.Color = Color.FromArgb(100, 0, 255, 0);

			float totalPl = 0;
			float totalPs = 0;

			foreach (var price in pricePoints.price_points.Keys.OrderBy(k => k)) {
				var dprice = (double)price;
				var p = pricePoints.price_points[price];
				var curD = p.pl - p.ps;
				totalPl += p.pl;
				totalPs += p.ps;
				if(previousPricePoints != null) {
					if(previousPricePoints.price_points.ContainsKey(price)) {
						var pre = previousPricePoints.price_points[price];
						var preD = pre.pl - pre.ps;

						if (preD < 0 && curD > 0) {
							seriesDeltaPos.Points.Add(new DataPoint(dprice, curD) { Color = Color.FromArgb(100, 255, 0, 0) } );
							seriesPreviousDeltaPos.Points.Add(new DataPoint(dprice, preD) { Color = Color.FromArgb(100, 255, 0, 0) });
						} else if(preD > 0 && curD < 0) {
							seriesDeltaPos.Points.Add(new DataPoint(dprice, curD) { Color = Color.FromArgb(100, 0, 0, 255) });
							seriesPreviousDeltaPos.Points.Add(new DataPoint(dprice, preD) { Color = Color.FromArgb(100, 0, 0, 255) });
						} else if(preD > 0 && curD > 0) {
							seriesDeltaPos.Points.Add(new DataPoint(dprice, Math.Min(curD, preD)) { Color = Color.FromArgb(100, 0, 255, 0) });
							seriesPreviousDeltaPos.Points.Add(new DataPoint(dprice, Math.Abs(curD - preD)) { Color = curD > preD ? Color.FromArgb(100, 255, 0, 0) : Color.FromArgb(25,0,0,255) });
						} else {
							seriesDeltaPos.Points.Add(new DataPoint(dprice, Math.Max(curD, preD)) { Color = Color.FromArgb(100, 0, 255, 0) });
							seriesPreviousDeltaPos.Points.Add(new DataPoint(dprice, -Math.Abs(curD - preD)) { Color = curD > preD ? Color.FromArgb(25, 255, 0, 0) : Color.FromArgb(100, 0, 0, 255) });
						}
					} else {
						seriesDeltaPos.Points.Add(new DataPoint(dprice, curD));
						seriesPreviousDeltaPos.Points.Add(new DataPoint(dprice,0));
					}
				} else {
					seriesDeltaPos.Points.Add(new DataPoint(dprice, curD));
				}
			}


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

			Series seriesRate = new Series();
			chart.Series.Add(seriesRate);
			seriesRate.ChartType = SeriesChartType.Line;
			//			seriesRate.SetCustomProperty("PointWidth", "0.01");
			seriesRate.Points.Add(new DataPoint(pricePoints.rate, 5.0f));
			seriesRate.Points.Add(new DataPoint(pricePoints.rate, -1.0f));
			seriesRate.Color = Color.Blue;

			if (hasStreamPriceSeries) {
				streamPriceSeries = new Series();
				chart.Series.Add(streamPriceSeries);
				streamPriceSeries.ChartType = SeriesChartType.Line;
				//				streamPriceSeries.SetCustomProperty("PointWidth", "0.01");
				streamPriceSeries.Points.Add(new DataPoint(latestPrice, 5.0f));
				streamPriceSeries.Points.Add(new DataPoint(latestPrice, -1.0f));
				streamPriceSeries.Color = Color.Red;
			}
			float pld = 0;
			float psd = 0;
			if (previousPricePoints != null) {
				float prevTotalPl = 0;
				float prevTotalPs = 0;
				foreach (var pp in previousPricePoints.price_points) {
					prevTotalPl += pp.Value.pl;
					prevTotalPs += pp.Value.ps;
				}
				pld = totalPl - prevTotalPl;
				psd = totalPs - prevTotalPs;
			}
			chart.Titles.Add(String.Format("{0} short position:{1:0.###}({2:+0.###;-0.###;0.###}) long position:{3:0.###}({4:+0.###;-0.###;0.###}) ", dateTime, totalPs, psd, totalPl, pld));


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

		private Dictionary<OrderBookDao.Entity, PricePoints> pricePointsCache = new Dictionary<OrderBookDao.Entity, PricePoints>();

		private PricePoints GetPricePoints(OrderBookDao.Entity orderBook) {
			if(pricePointsCache.ContainsKey(orderBook)) {
				return pricePointsCache[orderBook];
			}

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
			pricePointsCache[orderBook] = pricePoints;
			return pricePoints;
		}

		private void LoadChart(Chart chart, int index) {
			OrderBookDao.Entity orderBook = orderBooks[index];
			PricePoints previousPricePoints = (index + 1 < orderBooks.Count()) ? GetPricePoints(orderBooks[index + 1]) : null;
			LoadChart(chart, orderBook.DateTime, GetPricePoints(orderBook), previousPricePoints, index == 0);
		}

		private void orderBookList_SelectedIndexChanged(object sender, EventArgs e) {
			LoadChart(chart1,orderBookList.SelectedIndex);
			if(orderBookList.SelectedIndex+1 < orderBooks.Count()) {
				LoadChart(chart2, orderBookList.SelectedIndex + 1);
			}
		}

		private void OrderBookForm_FormClosing(object sender, FormClosingEventArgs e) {
			if(connection != null) {
				connection.Close();
				connection = null;
			}
			if(tcpClient != null) {
				tcpClient.Close();
				tcpClient = null;
			}
			if(priceStreamClient != null) {
				priceStreamClient.Dispose();
				priceStreamClient = null;
			}
		}

	}
}
