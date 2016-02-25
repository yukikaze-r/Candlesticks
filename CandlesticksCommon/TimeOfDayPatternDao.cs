using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Candlesticks
{
	class TimeOfDayPatternDao {
		struct Column {
			public string Name;
			public DbType Type;
			public Func<Entity, object> Binder;
		}

		private static Column[] columns = new Column[] {
			new Column(){ Name = "check_start_time", Type = DbType.Time, Binder = e => e.Pattern.CheckStartTime.TimeSpan },
			new Column(){ Name = "check_end_time", Type = DbType.Time, Binder = e => e.Pattern.CheckEndTime.TimeSpan },
			new Column(){ Name = "is_check_up", Type = DbType.Boolean, Binder = e => e.Pattern.IsCheckUp  },
			new Column(){ Name = "trade_start_time", Type = DbType.Time, Binder = e => e.Pattern.TradeStartTime.TimeSpan },
			new Column(){ Name = "trade_end_time", Type = DbType.Time, Binder = e => e.Pattern.TradeEndTime.TimeSpan },
			new Column(){ Name = "trade_type", Type = DbType.String, Binder = e => e.Pattern.TradeType.ToString() },
			new Column(){ Name = "total_verification", Type = DbType.Int32, Binder = e => e.Pattern.TotalVerification },
			new Column(){ Name = "match_verification", Type = DbType.Int32, Binder = e => e.Pattern.MatchVerification },
		};
		
		public IEnumerable<Entity> GetAll() {

			using (var cmd = new NpgsqlCommand()) {
				cmd.Connection = DBUtils.GetConnection();
				cmd.CommandText = "select id,"+ String.Join(",",columns.Select(c=>c.Name)) + " from time_of_day_pattern";

				using (var dr = cmd.ExecuteReader()) {
					while (dr.Read()) {
						var pattern = new TimeOfDayPattern();
						var entity = new Entity();
						entity.Pattern = pattern;
						entity.Id = (Int64)dr[0];
						pattern.CheckStartTime = new TimeOfDayPattern.Time((TimeSpan)dr[1]);
						pattern.CheckEndTime = new TimeOfDayPattern.Time((TimeSpan)dr[2]);
						pattern.IsCheckUp = (Boolean)dr[3];
						pattern.TradeStartTime = new TimeOfDayPattern.Time((TimeSpan)dr[4]);
						pattern.TradeEndTime = new TimeOfDayPattern.Time((TimeSpan)dr[5]);
						pattern.TradeType = (TradeType) Enum.Parse(typeof(TradeType),(string)dr[6]);
						pattern.TotalVerification = (int)dr[7];
						pattern.MatchVerification = (int)dr[8];
						yield return entity;
					}
				}
			}
		}

		public class Entity {
			public Int64 Id;
			public TimeOfDayPattern Pattern;

			public void Save() {
				using (var cmd = new NpgsqlCommand()) {
					cmd.Connection = DBUtils.GetConnection();
					cmd.CommandText = "insert into time_of_day_pattern(" + String.Join(",", columns.Select(c => c.Name)) + ") " +
						"values(" + String.Join(",", columns.Select(c => ":" + c.Name)) + ") returning id";
					foreach (var c in columns) {
						cmd.Parameters.Add(new NpgsqlParameter(c.Name, c.Type)).Value = c.Binder(this);
						this.Id = (Int64)cmd.ExecuteScalar();
					}
				}
			}

		}

	}
}
