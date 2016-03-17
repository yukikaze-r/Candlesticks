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
		private Series streamPriceSeries = null;
		private float latestPrice;
		private OandaAPI oandaApi;
		private Series latestVolumeSeries = null;
		private List<Candlestick> latestS5Candles;
		private object volumeCandlesLock = new object();
		private Thread retriveVolumesThread = null;

		public OrderBookForm() {
			InitializeComponent();
		}

		
		private void chart1_Click_1(object sender, EventArgs e) {
			
		}

		private void OrderBookForm_Load(object sender, EventArgs ev) {
			oandaApi = new OandaAPI();

			LoadOrderBookList();

			orderBookList.SelectedIndex = 0;

			splitContainer1.Panel2.HorizontalScroll.Value = splitContainer1.Panel1.HorizontalScroll.Value
				= (splitContainer1.Panel1.HorizontalScroll.Maximum + splitContainer1.Panel1.HorizontalScroll.Minimum - splitContainer1.Panel1.ClientSize.Width) / 2;

			EventReceiver.Instance.OrderBookUpdatedEvent += OnOrderBookUpdated;

			retriveVolumesThread = new Thread(new ThreadStart(RetriveVolumes));
			retriveVolumesThread.Start();

			PriceObserver.Get("USD_JPY").Observe(ReceivePrice);
		}

		private void RetriveVolumes() {
			while (volumeCandlesLock != null) {
				Action invokeTarget = null;
				lock (volumeCandlesLock) {
					if (latestS5Candles == null) {
						latestS5Candles = RetrieveS5Candles(orderBooks[0]);
						/*						latestS5Candles = oandaApi.GetCandles(new DateTime(firstDateTime.Year, firstDateTime.Month, firstDateTime.Day, firstDateTime.Hour, firstDateTime.Minute, 0, DateTimeKind.Local),
													DateTime.Now, "USD_JPY", "S5").ToList();*/
						invokeTarget = new Action(() => {
							if (orderBookList.SelectedIndex == 0) {
								lock (volumeCandlesLock) {
									if (latestS5Candles != null) {
										LoadVolumeSeries(latestVolumeSeries, latestS5Candles);
									}
								}
							}
						});
					} else {
						invokeTarget = RetriveVolumeLatest();
					}
				}
				if(invokeTarget != null) {
					Invoke(invokeTarget);
				}

				try {
					Thread.Sleep(5000);
				} catch(ThreadInterruptedException) {
					return;
				}
			}
		}

		private List<Candlestick> RetrieveS5Candles(OrderBookDao.Entity orderBook) {
			using(DBUtils.OpenThreadConnection()) {
				DateTime firstDateTime = orderBook.DateTime;
				DateTime startDateTime = new DateTime(
					firstDateTime.Year, firstDateTime.Month, firstDateTime.Day, firstDateTime.Hour, firstDateTime.Minute, 0, DateTimeKind.Local);
				return new CandlesticksGetter() {
					Start = startDateTime,
					End = startDateTime.AddMinutes(20),
					Granularity = "S5"
				}.Execute().ToList();
			}
		}

		private readonly float VOLUME_PRICE_GRANURALITY = 0.01f;

		private void LoadVolumeSeries(Series series, List<Candlestick> s5candles) {
			Dictionary<float, float> priceVolumes = new Dictionary<float, float>();
			foreach (var candle in s5candles) {
				float min = GetRoundPrice(candle.Low);
				float max = GetRoundPrice(candle.High);
				float average = (float)candle.Volume / (int)((max - min + 1) / VOLUME_PRICE_GRANURALITY);
				for (float i = min; i <= max; i += VOLUME_PRICE_GRANURALITY) {
					if (priceVolumes.ContainsKey(i)) {
						priceVolumes[i] += average;
					} else {
						priceVolumes[i] = average;
					}
				}
			}
			series.Points.Clear();
			foreach (var price in priceVolumes.Keys.OrderBy(k => k)) {
				series.Points.Add(new DataPoint(price, priceVolumes[price] / (100 * VOLUME_PRICE_GRANURALITY)));
			}
		}

		private Action RetriveVolumeLatest() {
			var candle = oandaApi.GetCandles(1,"USD_JPY", "S5").First();
			if(latestS5Candles.Where(c=>c.DateTime.Equals(candle.DateTime)).Count()==0) {
				latestS5Candles.Add(candle.Candlestick);

				return new Action(() => {
					if (orderBookList.SelectedIndex == 0) {
						volumeLabel.Text = candle.volume.ToString();
						volumeLabel.ForeColor = candle.volume >= 10 ? Color.Red : (candle.volume >= 5 ? Color.Orange : (candle.volume >= 2 ? Color.Blue : Color.Gray));
						float min = GetRoundPrice(candle.lowMid);
						float max = GetRoundPrice(candle.highMid);
						float average = (float)candle.volume / (int)((max - min + 1) / VOLUME_PRICE_GRANURALITY);
						Console.WriteLine("candle lowMid:" + min + "(" + candle.lowMid+") highMid:" +max + "(" + candle.highMid+") volume:"+candle.volume+" avg:"+ average);
						for (float i = min; i <= max; i += VOLUME_PRICE_GRANURALITY) {
							DataPoint dp = latestVolumeSeries.Points.Where(p => p.XValue == i).FirstOrDefault();
							if(dp == null) {
								dp = new DataPoint(i, 0);
								latestVolumeSeries.Points.Add(dp);
							}
							dp.YValues[0] += average / (100 * VOLUME_PRICE_GRANURALITY);
						}


						foreach (var dataPoint in latestVolumeSeries.Points.Where(p => min <= p.XValue && p.XValue <= max)) {
//							dataPoint.YValues[0] += average / (100 * VOLUME_PRICE_GRANURALITY);
						}
					}
				});
			}
			return null;
		}

		private float GetRoundPrice(float price) {
			int n = (int)((price + VOLUME_PRICE_GRANURALITY/2) / VOLUME_PRICE_GRANURALITY);
			return n * VOLUME_PRICE_GRANURALITY;
		}

		private void ReceivePrice(DateTime dateTime,float bid, float ask) {
			Invoke(new Action(() => {
				timeLabel.Text = dateTime.ToString("M/d HH:mm:ss");
				bidLabel.Text = bid.ToString("f3");
				askLabel.Text = ask.ToString("f3");
				if (streamPriceSeries != null) {
					latestPrice = (bid + ask) / 2;
					streamPriceSeries.Points[0].XValue = latestPrice;
					streamPriceSeries.Points[1].XValue = latestPrice;
				}
			}));
		}

		private void OnOrderBookUpdated(OrderBookUpdated orderBookUpdated) {
			Invoke(new Action(() => {
				int selectedIndex = orderBookList.SelectedIndex;
				LoadOrderBookList(orderBookUpdated.DateTime);
				if (selectedIndex == 0) {
					orderBookList.SelectedIndex = 0;
				}
			}));
			lock (volumeCandlesLock) {
				latestS5Candles = null;
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
		
		private void LoadChart(Chart chart, DateTime dateTime, OrderBookDao.Entity orderBook, OrderBookDao.Entity previousOrderBook, bool isLatestChart) {

			PricePoints pricePoints = GetPricePoints(orderBook);
			PricePoints previousPricePoints = previousOrderBook!=null ? GetPricePoints(previousOrderBook) : null;

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

/*			chartArea.AxisX2.Enabled = AxisEnabled.True;
			chartArea.AxisX2.Minimum = chartArea.AxisX.Minimum;
			chartArea.AxisX2.Maximum = chartArea.AxisX.Maximum;*/
//			chartArea.AxisX2.Interval = VOLUME_PRICE_GRANURALITY;


			chartArea.AxisY.Maximum = 5.0f;
			chartArea.AxisY.Minimum = 0f;
			chartArea.AxisY.Interval = 1.0f;
			chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

			chartArea.AxisY2.Enabled = AxisEnabled.True;
			chartArea.AxisY2.Maximum = 2.5f;
			chartArea.AxisY2.Minimum = -2.5f;
			chartArea.AxisY2.Interval = 1.0f;

			chart.ChartAreas.Add(chartArea);

			Series volumeSeries = new Series();
			volumeSeries.ChartType = SeriesChartType.Column;
			//				volumeSeries.XAxisType = AxisType.Secondary;
			volumeSeries.Color = Color.LightGray;
			volumeSeries.SetCustomProperty("PixelPointWidth", "2");
			//				volumeSeries.SetCustomProperty("PointWidth", "0.5");
			chart.Series.Add(volumeSeries);

			if (isLatestChart) {
				latestVolumeSeries = volumeSeries;
				lock (volumeCandlesLock) {
					if (latestS5Candles != null) {
						LoadVolumeSeries(latestVolumeSeries,latestS5Candles);
					}
				}
			} else {
				LoadVolumeSeries(volumeSeries, RetrieveS5Candles(orderBook));
			}

			Series seriesDeltaPos = new Series();
			chart.Series.Add(seriesDeltaPos);
			seriesDeltaPos.YAxisType = AxisType.Secondary;
			seriesDeltaPos.ChartType = SeriesChartType.StackedColumn;
			seriesDeltaPos.LegendText = "buy - sell position";
			seriesDeltaPos.SetCustomProperty("PixelPointWidth", "14");
			seriesDeltaPos.Color = Color.FromArgb(100, 0, 255, 0);


			Series seriesPreviousDeltaPos = new Series();
			chart.Series.Add(seriesPreviousDeltaPos);
			seriesPreviousDeltaPos.YAxisType = AxisType.Secondary;
			seriesPreviousDeltaPos.ChartType = SeriesChartType.StackedColumn;
			seriesPreviousDeltaPos.SetCustomProperty("PixelPointWidth", "14");
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
			seriesRate.Color = Color.Pink;

			if (isLatestChart) {
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
			OrderBookDao.Entity previousOrderBook = (index + 1 < orderBooks.Count()) ? orderBooks[index + 1] : null;
			LoadChart(chart, orderBook.DateTime, orderBook, previousOrderBook, index == 0);
		}

		private void orderBookList_SelectedIndexChanged(object sender, EventArgs e) {
			LoadChart(chart1,orderBookList.SelectedIndex);
			if(orderBookList.SelectedIndex+1 < orderBooks.Count()) {
				LoadChart(chart2, orderBookList.SelectedIndex + 1);
			}
		}

		private void OrderBookForm_FormClosing(object sender, FormClosingEventArgs e) {
			Console.WriteLine("OrderBookForm_FormClosing");
			PriceObserver.Get("USD_JPY").UnOnserve(ReceivePrice);
			EventReceiver.Instance.OrderBookUpdatedEvent -= OnOrderBookUpdated;

			if (connection != null) {
				connection.Close();
				connection = null;
			}
			if(tcpClient != null) {
				tcpClient.Close();
				tcpClient = null;
			}
			if(oandaApi != null) {
				oandaApi.Dispose();
				oandaApi = null;
			}
			if(retriveVolumesThread != null) {
				retriveVolumesThread.Interrupt();
				retriveVolumesThread = null;
			}
		}

		private void volumeLabel_Click(object sender, EventArgs e) {

		}
	}
}
