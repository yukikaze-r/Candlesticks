using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Candlesticks
{
    class CandlestickDao {
		private NpgsqlConnection connection;

		public CandlestickDao(NpgsqlConnection connection) {
			this.connection = connection;
		}

		public IEnumerable<Entity> GetBy(string instrument, string granularity, DateTime start, DateTime end) {

			using (var cmd = new NpgsqlCommand()) {
				cmd.Connection = connection;
				cmd.CommandText = "select id,date_time,open,high,low,close,volume from candlestick where instrument = :instrument and granularity = :granularity and :start <= date_time and date_time < :end order by date_time";
				cmd.Parameters.Add(new NpgsqlParameter("instrument", DbType.String));
				cmd.Parameters.Add(new NpgsqlParameter("granularity", DbType.String));
				cmd.Parameters.Add(new NpgsqlParameter("start", DbType.DateTime));
				cmd.Parameters.Add(new NpgsqlParameter("end", DbType.DateTime));
				cmd.Parameters["instrument"].Value = instrument;
				cmd.Parameters["granularity"].Value = granularity;
				cmd.Parameters["start"].Value = start;
				cmd.Parameters["end"].Value = end;
				

				using (var dr = cmd.ExecuteReader()) {
					while (dr.Read()) {
						var entity = new Entity();
						entity.Dao = this;
						entity.Id = (Int64)dr[0];
						entity.Instrument = instrument;
						entity.Granularity = granularity;
						entity.DateTime = (DateTime)dr[1];
						entity.Open = (float)((decimal)dr[2]);
						entity.High = (float)((decimal)dr[3]);
						entity.Low = (float)((decimal)dr[4]);
						entity.Close = (float)((decimal)dr[5]);
						entity.Volume = (int)dr[6];
						yield return entity;
					}
				}
			}
		}

		public Entity CreateNewEntity() {
			return new Entity() { Dao = this };
		}
		

		public class Entity {
			public CandlestickDao Dao;
			public Int64 Id;
			public string Instrument;
			public string Granularity;
			public DateTime DateTime;
			public float Open;
			public float High;
			public float Low;
			public float Close;
			public int Volume;

			public void Save() {
				using (var cmd = new NpgsqlCommand()) {
					cmd.Connection = Dao.connection;
					cmd.CommandText = "insert into candlestick(instrument,granularity,date_time,open,high,low,close,volume) values(:instrument,:granularity,:date_time,:open,:high,:low,:close,:volume) returning id";
					cmd.Parameters.Add(new NpgsqlParameter("instrument", DbType.String));
					cmd.Parameters.Add(new NpgsqlParameter("granularity", DbType.String));
					cmd.Parameters.Add(new NpgsqlParameter("date_time", DbType.DateTime));
					cmd.Parameters.Add(new NpgsqlParameter("open", DbType.Single));
					cmd.Parameters.Add(new NpgsqlParameter("high", DbType.Single));
					cmd.Parameters.Add(new NpgsqlParameter("low", DbType.Single));
					cmd.Parameters.Add(new NpgsqlParameter("close", DbType.Single));
					cmd.Parameters.Add(new NpgsqlParameter("volume", DbType.Int32));
					cmd.Parameters["instrument"].Value = this.Instrument;
					cmd.Parameters["granularity"].Value = this.Granularity;
					cmd.Parameters["date_time"].Value = this.DateTime;
					cmd.Parameters["open"].Value = this.Open;
					cmd.Parameters["high"].Value = this.High;
					cmd.Parameters["low"].Value = this.Low;
					cmd.Parameters["close"].Value = this.Close;
					cmd.Parameters["volume"].Value = this.Volume;
					this.Id = (Int64)cmd.ExecuteScalar();
				}
			}

			public Candlestick Candlestick {
				get {
					Candlestick result = new Candlestick();
					result.Time = this.DateTime;
					result.Open = this.Open;
					result.High = this.High;
					result.Low = this.Low;
					result.Close = this.Close;
					result.Volume = this.Volume;
					return result;
				}
			}
		}
	}
}
