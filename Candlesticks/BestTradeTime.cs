using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {
	class BestTradeTime {

		private IEnumerable<Candlestick> candlesticks;

		public BestTradeTime(IEnumerable<Candlestick> candlesticks) {
			this.candlesticks = candlesticks;
			this.ShiftHour = 0;
			this.Comparator = f => f[1] - f[0] + f[2] - f[3];
			this.IsSummerTime = null;
			this.Granularity = new TimeSpan(0, 30, 0);
		}

		public int NumOfCandlesInDay {
			get {
				return (int)(new TimeSpan(1,0,0,0).Ticks / this.Granularity.Ticks);
			}
		}

		public TimeSpan Granularity {
			get;
			set;
		}

		public int ShiftHour {
			get;
			set;
		}

		public Func<int[], int> Comparator {
			get;
			set;
		}

		public bool? IsSummerTime {
			get;
			set;
		}

		public List<Candlestick[]> GetCandlesticksEachDate() {
			DateTime oldDate = new DateTime();
			Candlestick[] hmList = null;
			List<Candlestick[]> result = new List<Candlestick[]>();
			TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

			foreach (var c in candlesticks) {

				if (IsSummerTime!=null && IsSummerTime.Value ==false && est.IsDaylightSavingTime(c.DateTime)) {
					continue;
				}



				DateTime shiftedTime = c.DateTime.AddHours(this.ShiftHour);

				if (!oldDate.Equals(shiftedTime.Date)) {
					hmList = new Candlestick[NumOfCandlesInDay];
					result.Add(hmList);
					oldDate = shiftedTime.Date;
				}
				hmList[GetArrayIndex(new TimeSpan(shiftedTime.Hour, shiftedTime.Minute, 0))]= c;
//				hmList[shiftedTime.Hour * 2 + (shiftedTime.Minute == 30 ? 1 : 0)] = c;
			}
			return result;
		}

		private int GetArrayIndex(TimeSpan span) {
			return (int)(span.Ticks / Granularity.Ticks);
		}

		public string HMString(int index) {
			long t = Granularity.Ticks * index - new TimeSpan(this.ShiftHour, 0, 0).Ticks;
			if(t < 0) {
				t += new TimeSpan(1, 0, 0, 0).Ticks;
			}
			return new TimeSpan(t).ToString("h\\:mm");
		}

/*		public string HMString(int t) {
			t -= this.ShiftHour * 2;
			if(t < 0) {
				t += 48;
			}
			return (t / 2 < 10 ? "0" : "") + t / 2 + ":" + (t % 2 == 0 ? "00" : "30");
		}*/

		public IEnumerable<Tuple<int[], int[]>> Calculate( int limit) {

			List<Candlestick[]> candlesticksEachDate = GetCandlesticksEachDate();
			List<Tuple<int[], int[]>> summary = new List<Tuple<int[], int[]>>();
			int dayLength = GetArrayIndex(new TimeSpan(1, 0, 0, 0));
			int searchLength = GetArrayIndex(new TimeSpan(0, 6, 0, 0));

			for (int s1 = 0; s1 < dayLength - searchLength; s1++) {
				for (int e1 = s1 + 1; e1 < s1 + searchLength; e1++) {
					for (int s2 = e1; s2 < dayLength - searchLength; s2++) {
						for (int e2 = s2 + 1; e2 < s2 + searchLength; e2++) {
							int[] dayResult = new int[4];
							summary.Add(new Tuple<int[], int[]>(new int[] { s1, e1, s2, e2 }, dayResult));
							foreach (var hml in candlesticksEachDate) {
								try {
									float d1 = hml[e1].Open - hml[s1].Open;
									float d2 = hml[e2].Open - hml[s2].Open;

									if (hml[e1].IsNull || hml[s1].IsNull || hml[s2].IsNull || hml[e2].IsNull) {
										continue;
									}
									dayResult[(d1 >= 0 ? 0 : 2) + (d2 >= 0 ? 0 : 1)]++;
								} catch (Exception) {
								}
							}
						}
					}
				}
			}

			int i = 0;
			foreach (var t in summary.OrderByDescending(t => this.Comparator(t.Item2)) /*Math.Abs(t.Item2[0] - t.Item2[1]) + Math.Abs(t.Item2[2] - t.Item2[3]))*/) {
				if (i >= limit) {
					break;
				}
				yield return t;
				i++;
			}

		}

	}
}
