using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Candlesticks
{
    class OrderBookDao
    {
		private NpgsqlConnection connection;

		public OrderBookDao(NpgsqlConnection connection) {
			this.connection = connection;
		}

		public Int64 GetCountByDateTime(string instrument, DateTime dateTime) {
			using (var cmd = new NpgsqlCommand()) {
				cmd.Connection = connection;
				cmd.CommandText = "select count(*) from order_book where instrument=:instrument and date_time=:date_time";
				cmd.Parameters.Add(new NpgsqlParameter("instrument", DbType.String));
				cmd.Parameters.Add(new NpgsqlParameter("date_time", DbType.DateTime));
				cmd.Parameters["instrument"].Value = instrument;
				cmd.Parameters["date_time"].Value = dateTime;
				using (var dr = cmd.ExecuteReader()) {
					if (dr.Read()) {
						return (Int64)dr[0];
					}
					throw new Exception();
				}
			}
		}

		public IEnumerable<Entity> GetByInstrumentOrderByDateTimeDescendant(string instrument) {
			using (var cmd = new NpgsqlCommand()) {
				cmd.Connection = connection;
				cmd.CommandText = "select id, date_time, rate from order_book where instrument=:instrument order by date_time desc ";
				cmd.Parameters.Add(new NpgsqlParameter("instrument", DbType.String));
				cmd.Parameters["instrument"].Value = instrument;
				using (var dr = cmd.ExecuteReader()) {
					while (dr.Read()) {
						var entity = new Entity();
						entity.Connection = connection;
						entity.Id = (Int64)dr[0];
						entity.DateTime = (DateTime)dr[1];
						entity.Rate =(float) ((decimal)dr[2]);
						yield return entity;
					}
				}
			}
		}

		public Entity GetByInstrumentAndDateTime(string instrument, DateTime dateTime) {
			using (var cmd = new NpgsqlCommand()) {
				cmd.Connection = connection;
				cmd.CommandText = "select id, date_time, rate from order_book where instrument=:instrument and date_time=:date_time";
				cmd.Parameters.Add(new NpgsqlParameter("date_time", DbType.DateTime));
				cmd.Parameters.Add(new NpgsqlParameter("instrument", DbType.String));
				cmd.Parameters["date_time"].Value = dateTime;
				cmd.Parameters["instrument"].Value = instrument;
				using (var dr = cmd.ExecuteReader()) {
					while (dr.Read()) {
						var entity = new Entity();
						entity.Connection = connection;
						entity.Id = (Int64)dr[0];
						entity.DateTime = (DateTime)dr[1];
						entity.Rate = (float)((decimal)dr[2]);
						return entity;
					}
				}
			}
			return null;
		}

		public class Entity {
			public NpgsqlConnection Connection;
			public Int64 Id;
			public string Instrument;
			public DateTime DateTime;
			public float Rate;

			public DateTime NormalizedDateTime {
				get {
					return new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, DateTime.Minute, 0, DateTime.Kind);
				}
			}

			public IEnumerable<OrderBookPricePointDao.Entity> GetPricePointsOrderByPrice() {
				return new OrderBookPricePointDao(Connection).GetByOrderBookOrderByPrice(Id);
			}
		}
	}
}
