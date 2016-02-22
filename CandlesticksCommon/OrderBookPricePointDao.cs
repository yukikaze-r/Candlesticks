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
