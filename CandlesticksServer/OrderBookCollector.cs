﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace Candlesticks {
	class OrderBookCollector {

		private static int INTERVAL_MINUTE = 20;

		private EventServer eventServer;
		private OandaAPI oandaApi;
		private NpgsqlConnection connection;

		public OrderBookCollector(EventServer eventServer) {
			this.eventServer = eventServer;

			oandaApi = new OandaAPI();

			var orderbook = oandaApi.GetOrderbookData(3600);

			this.connection = CreateConnection();
			this.connection.Open();

			SaveOrderbook(orderbook);

			var now = DateTime.Now;
			var millisecond = (now.Minute * 60 + now.Second) * 1000 + now.Millisecond;

			var remainMillisecond = INTERVAL_MINUTE * 60 * 1000 - millisecond % (INTERVAL_MINUTE * 60 * 1000) + 60 * 1000;
			Trace.WriteLine("now:" + now);
			Trace.WriteLine("remain:" + remainMillisecond);
			Trace.Flush();
			var timer = new Timer() {
				Interval = (double)remainMillisecond,
				AutoReset = false,
			};
			timer.Elapsed += Timer_FirstElapsed;
			timer.Start();

		}

		private NpgsqlConnection CreateConnection() {
			var dbSetting = Setting.Instance.DBConnection;
			return new NpgsqlConnection(
				"Host=" + dbSetting.Host +
				";Username=" + dbSetting.UserName +
				";Password=" + dbSetting.Password +
				";Database=" + dbSetting.Database);
		}

		private void Timer_FirstElapsed(object sender, ElapsedEventArgs e) {
			try {
				Trace.WriteLine("Timer_FirstElapsed");
				var timer = new Timer() {
					Interval = (double)INTERVAL_MINUTE * 60 * 1000,
					AutoReset = true,
				};
				timer.Elapsed += Timer_Elapsed;
				timer.Start();
				SaveOrderbook(oandaApi.GetOrderbookData(3600));
			} catch(Exception ex) {
				Trace.WriteLine(ex);
			}
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
			try {
				Trace.WriteLine("Timer_Elapsed");
				SaveOrderbook(oandaApi.GetOrderbookData(3600));
			} catch (Exception ex) {
				Trace.WriteLine(ex);
			}
		}

		private void SaveOrderbook(Dictionary<DateTime,PricePoints> orderbook) {
			DateTime? lastUpdated = null;

			using (var cmd = new NpgsqlCommand()) {
				cmd.Connection = connection;
				cmd.CommandText = "select count(*) from order_book where date_time=:date_time";
				cmd.Parameters.Add(new NpgsqlParameter("date_time", DbType.DateTime));

				foreach (var time in orderbook.Keys.OrderBy(k => k)) {

					cmd.Parameters["date_time"].Value = time;
					NpgsqlDataReader dr = cmd.ExecuteReader();
					Int64 count = 0;
					while (dr.Read()) {
						count = (Int64)dr[0];
					}
					dr.Close();
					if ( count == 0) {
						Trace.WriteLine(time+" is not exists");
						Trace.Flush();
						SavePricePoints(time,orderbook[time]);
						lastUpdated = time;
					}
				}
			}

			if(lastUpdated != null) {
				this.eventServer.Send(new OrderBookUpdated());
			}
		}

		private void SavePricePoints(DateTime time, PricePoints pricePoints) {
			using (var transaction = connection.BeginTransaction()) {
				Int64 id = 0;
				using (var cmd = new NpgsqlCommand()) {
					cmd.Connection = connection;
					cmd.CommandText = "insert into order_book(date_time,rate) values(:date_time,:rate) returning id";
					cmd.Parameters.Add(new NpgsqlParameter("date_time", DbType.DateTime));
					cmd.Parameters.Add(new NpgsqlParameter("rate", DbType.Single));
					cmd.Parameters["date_time"].Value = time;
					cmd.Parameters["rate"].Value = pricePoints.rate;
					id = (Int64)cmd.ExecuteScalar();
				}
			
				using (var cmd = new NpgsqlCommand()) {
					cmd.Connection = connection;
					cmd.CommandText = "insert into order_book_price_point(order_book_id,price,os,ps,ol,pl) values(:order_book_id,:price,:os,:ps,:ol,:pl)";
					cmd.Parameters.Add(new NpgsqlParameter("order_book_id", DbType.Int64));
					cmd.Parameters.Add(new NpgsqlParameter("price", DbType.Single));
					cmd.Parameters.Add(new NpgsqlParameter("os", DbType.Single));
					cmd.Parameters.Add(new NpgsqlParameter("ps", DbType.Single));
					cmd.Parameters.Add(new NpgsqlParameter("ol", DbType.Single));
					cmd.Parameters.Add(new NpgsqlParameter("pl", DbType.Single));

					foreach(var price in pricePoints.price_points.Keys.OrderBy(k=>k)) {
						var price_point = pricePoints.price_points[price];
						cmd.Parameters["order_book_id"].Value = id;
						cmd.Parameters["price"].Value = price;
						cmd.Parameters["os"].Value = price_point.os;
						cmd.Parameters["ps"].Value = price_point.ps;
						cmd.Parameters["ol"].Value = price_point.ol;
						cmd.Parameters["pl"].Value = price_point.pl;
						cmd.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}

		}

	}
}
