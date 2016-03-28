using Npgsql;
using System;
using System.Collections.Concurrent;
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
		class InstrumentInfo {
			public string Name;
			public float GraphMinPrice;
			public float GraphMaxPrice;
			public float GraphInterval;
			public string AxisLabelFormat;

			public float Bid;
			public float Ask;
			public DateTime LastPriceTime;

			public override string ToString() {
				return Name;
			}
		}

		private InstrumentInfo[] instruments = new InstrumentInfo[] {
			new InstrumentInfo() {
				Name = "USD_JPY",
				GraphMinPrice = 105f,
				GraphMaxPrice = 120f,
				GraphInterval = 0.05f,
				AxisLabelFormat = "f2",
			},
			new InstrumentInfo() {
				Name = "EUR_JPY",
				GraphMinPrice = 120f,
				GraphMaxPrice = 135f,
				GraphInterval = 0.05f,
				AxisLabelFormat = "f2",
			},
			new InstrumentInfo() {
				Name = "EUR_USD",
				GraphMinPrice = 1.05f,
				GraphMaxPrice = 1.2f,
				GraphInterval = 0.0005f,
				AxisLabelFormat = "f4",
			},
		};

		private NpgsqlConnection connection = null;
		private TcpClient tcpClient = null;
		private Dictionary<string, List<OrderBookDao.Entity>> orderBooks = new Dictionary<string, List<OrderBookDao.Entity>>();
		private Series streamPriceSeries = null;
		private float latestPrice;
		private Series latestVolumeSeries = null;
		private List<Candlestick> latestS5Candles;
		private Thread retriveVolumesThread = null;
		private InstrumentInfo selectedInstrument = null;
		private Dictionary<string, Dictionary<OrderBookDao.Entity, PricePoints>> pricePointsCache = null;
		private BlockingCollection<Action> retriveVolumeQueue = new BlockingCollection<Action>();

		public OrderBookForm() {
			InitializeComponent();

			foreach (var instrument in instruments) {
				comboBox1.Items.Add(instrument);
			}
		}

		
		private void OrderBookForm_Load(object sender, EventArgs ev) {
			
			connection = DBUtils.OpenConnection();

			comboBox1.SelectedIndex = 0;

//			LoadOrderBookList(INSTRUMENTS[0]);

	
/*			splitContainer1.Panel2.HorizontalScroll.Value = splitContainer1.Panel1.HorizontalScroll.Value
				= (splitContainer1.Panel1.HorizontalScroll.Maximum + splitContainer1.Panel1.HorizontalScroll.Minimum - splitContainer1.Panel1.ClientSize.Width) / 2;*/
				

			EventReceiver.Instance.OrderBookUpdatedEvent += OnOrderBookUpdated;

			retriveVolumesThread = new Thread(new ThreadStart(RetriveVolumes));
			retriveVolumesThread.Start();

			PriceObserver.Get().Observe(ReceivePrice);
		}

		private void RetriveVolumes() {
			while (true) {
				var action = retriveVolumeQueue.Take();
				if(action == null) {
					break;
				}
				action();
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
			var candle = OandaAPI.Instance.GetCandles(1,"USD_JPY", "S5").First();
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

		private InstrumentInfo GetInstrumentInfo(string instrument) {
			return instruments.Where(i => i.Name == instrument).First();
		}

		private void ReceivePrice(string instrument, DateTime dateTime,float bid, float ask) {
			var info = GetInstrumentInfo(instrument);

			info.Bid = bid;
			info.Ask = ask;
			info.LastPriceTime = dateTime;

			if (selectedInstrument.Name == instrument) {
				Invoke(new Action(() => {
					UpdatePrice();
				}));
			}
		}

		private void UpdatePrice() {
			timeLabel.Text = selectedInstrument.LastPriceTime.ToString("M/d HH:mm:ss");
			bidLabel.Text = selectedInstrument.Bid.ToString("f3");
			askLabel.Text = selectedInstrument.Ask.ToString("f3");
			if (streamPriceSeries != null) {
				latestPrice = (selectedInstrument.Bid + selectedInstrument.Ask) / 2;
				streamPriceSeries.Points[0].XValue = latestPrice;
				streamPriceSeries.Points[1].XValue = latestPrice;
			}
		}

		private void OnOrderBookUpdated(OrderBookUpdated orderBookUpdated) {

			if (orderBookUpdated.Instrument == selectedInstrument.Name) {
				Invoke(new Action(() => {
					int selectedIndex = orderBookList.SelectedIndex;
					LoadOrderBookList(orderBookUpdated.Instrument, orderBookUpdated.DateTime);
					if (selectedIndex == 0) {
						orderBookList.SelectedIndex = 0;
					}
					latestS5Candles = null;
				}));
			}
		}
		

		private void LoadOrderBookList(string instrument) {
			orderBookList.Items.Clear();
			if(orderBooks.ContainsKey(instrument) == false) {
				orderBooks[instrument] = new OrderBookDao(connection).GetByInstrumentOrderByDateTimeDescendant(instrument).ToList();
			}
			foreach (var entity in orderBooks[instrument]) {
				orderBookList.Items.Add(entity.DateTime);
			}
		}

		private void LoadOrderBookList(string instrument, DateTime dateTime) {
			if (orderBooks.ContainsKey(instrument) == false) {
				return;
			}
			var list = orderBooks[instrument];
			if (list.Count(e=>e.DateTime==dateTime) == 0) {
				var entity = new OrderBookDao(connection).GetByInstrumentAndDateTime(instrument, dateTime);
				orderBookList.Items.Insert(0,entity.DateTime);
				list.Insert(0, entity);
			}
		}

		private HashSet<Task<List<Candlestick>>> getS5CandlesTasks = new HashSet<Task<List<Candlestick>>>();
		
		private void LoadChart(Chart chart, DateTime dateTime, OrderBookDao.Entity orderBook, OrderBookDao.Entity previousOrderBook, bool isLatestChart) {

			PricePoints pricePoints = GetPricePoints(orderBook);
			PricePoints previousPricePoints = previousOrderBook!=null ? GetPricePoints(previousOrderBook) : null;

			chart.Series.Clear();
			chart.ChartAreas.Clear();
			chart.Titles.Clear();

			chart.Size = new Size(6000, 400);
			
			ChartArea chartArea = new ChartArea();
			chartArea.AxisX.Interval = selectedInstrument.GraphInterval;
			chartArea.AxisX.Minimum = selectedInstrument.GraphMinPrice;
			chartArea.AxisX.Maximum = selectedInstrument.GraphMaxPrice;
			chartArea.AxisX.LabelStyle.Format = selectedInstrument.AxisLabelFormat;
			chartArea.AxisX.MajorGrid.Enabled = true;
			chartArea.AxisX.MajorGrid.Interval = selectedInstrument.GraphInterval * 2;
			chartArea.AxisX.MinorGrid.Enabled = true;
			chartArea.AxisX.MinorGrid.LineColor = Color.LightGray;
			chartArea.AxisX.MinorGrid.Interval = selectedInstrument.GraphInterval;

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

			var UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();

			if (isLatestChart) {
				if(latestS5Candles == null) {
					latestVolumeSeries = volumeSeries;
					retriveVolumeQueue.Add(() => {
					var candlesticks = RetrieveS5Candles(orderBook);
					Invoke(new Action(()=>{
						latestS5Candles = candlesticks.ToList();
						LoadVolumeSeries(volumeSeries, latestS5Candles);
					}));
				});
			}

				//				, UISyncContext);
				/*
								lock (volumeCandlesLock) {
									if (latestS5Candles != null) {
										LoadVolumeSeries(latestVolumeSeries,latestS5Candles);
									}
								}*/
			} else {
				retriveVolumeQueue.Add(() => {
					var candlesticks = RetrieveS5Candles(orderBook);
					Invoke(new Action(() => {
						latestS5Candles = candlesticks.ToList();
						LoadVolumeSeries(volumeSeries, latestS5Candles);
					}));
				});

/*
					new TaskFactory().StartNew(() => RetrieveS5Candles(orderBook)).ContinueWith(candlesticks => {
					latestS5Candles = candlesticks.Result.ToList();
					LoadVolumeSeries(volumeSeries, latestS5Candles);
				}, UISyncContext);*/

/*
				if (gettingVolumes == false) {
					gettingVolumes = true;
					RetrieveS5CandlesAsync(orderBook).ContinueWith(candlesticks => {
						LoadVolumeSeries(volumeSeries, candlesticks.Result);
						gettingVolumes = false;
					}, UISyncContext);
				}*/
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
			seriesRate.BorderWidth = 3;

			if (isLatestChart) {
				streamPriceSeries = new Series();
				chart.Series.Add(streamPriceSeries);
				streamPriceSeries.ChartType = SeriesChartType.Line;
				//				streamPriceSeries.SetCustomProperty("PointWidth", "0.01");
				streamPriceSeries.Points.Add(new DataPoint(latestPrice, 5.0f));
				streamPriceSeries.Points.Add(new DataPoint(latestPrice, -1.0f));
				streamPriceSeries.Color = Color.Red;
				streamPriceSeries.BorderWidth = 3;
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
			Setting.Instance.OrderBookScrollPositions[selectedInstrument.Name] = splitContainer1.Panel1.HorizontalScroll.Value;
		}

		private void splitContainer1_Panel2_Scroll(object sender, ScrollEventArgs e) {
			splitContainer1.Panel1.HorizontalScroll.Value = splitContainer1.Panel2.HorizontalScroll.Value;
			Setting.Instance.OrderBookScrollPositions[selectedInstrument.Name] = splitContainer1.Panel2.HorizontalScroll.Value;
		}


		protected override Point ScrollToControl(Control activeControl)
		{
		    return splitContainer1.Panel1.AutoScrollPosition;
		}

		private PricePoints GetPricePoints(OrderBookDao.Entity orderBook) {
			if(pricePointsCache == null) {
				pricePointsCache = new Dictionary<string, Dictionary<OrderBookDao.Entity, PricePoints>>();
			}
			if(pricePointsCache.ContainsKey(selectedInstrument.Name) == false) {
				pricePointsCache.Add(selectedInstrument.Name, new Dictionary<OrderBookDao.Entity, PricePoints>());
			}
			var orderBookPricePoints = pricePointsCache[selectedInstrument.Name];

			if(orderBookPricePoints.ContainsKey(orderBook)) {
				return orderBookPricePoints[orderBook];
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
			orderBookPricePoints[orderBook] = pricePoints;
			return pricePoints;
		}

		private void LoadChart(Chart chart, int index) {
			var list = orderBooks[selectedInstrument.Name];
			OrderBookDao.Entity orderBook = list[index];
			OrderBookDao.Entity previousOrderBook = (index + 1 < list.Count()) ? list[index + 1] : null;
			LoadChart(chart, orderBook.DateTime, orderBook, previousOrderBook, index == 0);



				/*				splitContainer1.Panel2.HorizontalScroll.Value = splitContainer1.Panel1.HorizontalScroll.Value
									= (int) chart.ChartAreas[0].AxisX.ValueToPixelPosition(orderBook.Rate);*/

//				Console.WriteLine("ratePixel"+chart.ChartAreas[0].AxisX.ValueToPixelPosition(orderBook.Rate));
//				Console.WriteLine("splitContainer1.Panel2.HorizontalScroll.Value" + splitContainer1.Panel2.HorizontalScroll.Value);
		}


		private void SetHorizonalScrollPosition(int x) {
			var p = splitContainer1.Panel2.AutoScrollPosition;
			p.X = x;
			splitContainer1.Panel1.AutoScrollPosition = p;

			p = splitContainer1.Panel2.AutoScrollPosition;
			p.X = x;
			splitContainer1.Panel2.AutoScrollPosition = p;
		}

		private void orderBookList_SelectedIndexChanged(object sender, EventArgs ev) {
			try {
				LoadChart(chart1, orderBookList.SelectedIndex);
				if (orderBookList.SelectedIndex + 1 < orderBooks[selectedInstrument.Name].Count()) {
					LoadChart(chart2, orderBookList.SelectedIndex + 1);
				}
				if (orderBookList.SelectedIndex == 0) {
//					splitContainer1.Update();
					if (!Setting.Instance.OrderBookScrollPositions.ContainsKey(selectedInstrument.Name)) {
						double scrollRatio = (orderBooks[selectedInstrument.Name][0].Rate - selectedInstrument.GraphMinPrice) / (selectedInstrument.GraphMaxPrice - selectedInstrument.GraphMinPrice);
						int value = (int)((splitContainer1.Panel1.HorizontalScroll.Maximum - splitContainer1.Panel1.HorizontalScroll.Minimum) * scrollRatio + splitContainer1.Panel1.HorizontalScroll.Minimum - splitContainer1.Panel1.ClientSize.Width / 2);
						Console.WriteLine("scrollRatio:" + scrollRatio + " value:" + value + " min:" + splitContainer1.Panel1.HorizontalScroll.Minimum + " max:" + splitContainer1.Panel1.HorizontalScroll.Maximum);
						Console.WriteLine("scrollRatio:" + scrollRatio + " value:" + value + " min:" + splitContainer1.Panel2.HorizontalScroll.Minimum + " max:" + splitContainer1.Panel2.HorizontalScroll.Maximum);
//						splitContainer1.Panel2.HorizontalScroll.Value = splitContainer1.Panel1.HorizontalScroll.Value = value;

						SetHorizonalScrollPosition(value);
						Setting.Instance.OrderBookScrollPositions.Add(selectedInstrument.Name, value);
					} else {
						SetHorizonalScrollPosition(Setting.Instance.OrderBookScrollPositions[selectedInstrument.Name]);
					}

				}


			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		private void OrderBookForm_FormClosing(object sender, FormClosingEventArgs e) {
			Console.WriteLine("OrderBookForm_FormClosing");
			PriceObserver.Get().UnOnserve(ReceivePrice);
			EventReceiver.Instance.OrderBookUpdatedEvent -= OnOrderBookUpdated;
			Setting.Instance.Save();

			if (connection != null) {
				connection.Close();
				connection = null;
			}
			if(tcpClient != null) {
				tcpClient.Close();
				tcpClient = null;
			}
			if(retriveVolumeQueue != null) {
				retriveVolumeQueue.Add(null);
//				retriveVolumeQueue = null;
			}

/*			if(oandaApi != null) {
				oandaApi.Dispose();
				oandaApi = null;
			}*/
			if(retriveVolumesThread != null) {
				retriveVolumesThread.Interrupt();
				retriveVolumesThread = null;
			}
		}
		
		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
			selectedInstrument = (InstrumentInfo) comboBox1.SelectedItem;
			LoadOrderBookList(selectedInstrument.Name);
			orderBookList.SelectedIndex = 0;
			UpdatePrice();
		}

		private void timer1_Tick(object sender, EventArgs e) {
			retriveVolumeQueue.Add(() => {
				Action invokeTarget = null;
				if (latestS5Candles == null) {
				} else {
					invokeTarget = RetriveVolumeLatest();
				}
				if (invokeTarget != null) {
					Invoke(invokeTarget);
				}
			});
		}
	}
}
