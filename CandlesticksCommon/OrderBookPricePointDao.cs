using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Candlesticks
{
    class OrderBookPricePointDao
    {
		private NpgsqlConnection connection;

		public OrderBookPricePointDao(NpgsqlConnection connection) {
			this.connection = connection;
		}

		public IEnumerable<Tuple<DateTime, float, float>> GetPositionSummaryGroupByOrderBookDateTime(DateTime start, DateTime end) {
			using (var cmd = new NpgsqlCommand()) {
				cmd.Connection = connection;
				cmd.CommandText = "select ob.date_time, sum(pp.ps), sum(pp.pl) from order_book_price_point as pp, order_book as ob where pp.order_book_id = ob.id and :start <= ob.date_time and ob.date_time < :end group by ob.date_time order by ob.date_time";
				cmd.Parameters.Add(new NpgsqlParameter("start", DbType.DateTime));
				cmd.Parameters.Add(new NpgsqlParameter("end", DbType.DateTime));
				cmd.Parameters["start"].Value = start;
				cmd.Parameters["end"].Value = end;
				using (var dr = cmd.ExecuteReader()) {
					while (dr.Read()) {
						yield return new Tuple<DateTime, float, float>((DateTime)dr[0], (float)((decimal)dr[1]), (float)((decimal)dr[2]));
					}
				}
			}
		}

		public IEnumerable<Entity> GetByOrderBookOrderByPrice(Int64 orderBookId) {

			using (var cmd = new NpgsqlCommand()) {
				cmd.Connection = connection;
				cmd.CommandText = "select id, price, os, ps, ol, pl from order_book_price_point where order_book_id = :order_book_id order by price";
				cmd.Parameters.Add(new NpgsqlParameter("order_book_id", DbType.Int64));
				cmd.Parameters["order_book_id"].Value = orderBookId;

				using (var dr = cmd.ExecuteReader()) {
					while (dr.Read()) {
						var entity = new Entity();
						entity.Connection = connection;
						entity.Id = (Int64)dr[0];
						entity.OrderBookId = orderBookId;
						entity.Price = (float)((decimal)dr[1]);
						entity.Os = (float)((decimal)dr[2]);
						entity.Ps = (float)((decimal)dr[3]);
						entity.Ol = (float)((decimal)dr[4]);
						entity.Pl = (float)((decimal)dr[5]);
						yield return entity;
					}
				}
			}


		}

		public class Entity {
			public NpgsqlConnection Connection;
			public Int64 Id;
			public Int64 OrderBookId;
			public float Price;
			public float Os;
			public float Ps;
			public float Ol;
			public float Pl;
		}
	}
}
