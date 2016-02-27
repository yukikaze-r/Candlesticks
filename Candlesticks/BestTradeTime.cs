using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {
	class BestTradeTime {

		private IEnumerable<Candlestick> candlesticksM30;

		public BestTradeTime(IEnumerable<Candlestick> candlesticksM30) {
			this.candlesticksM30 = candlesticksM30;
			this.ShiftHour = 0;
			this.Comparator = f => f[1] - f[0] + f[2] - f[3];
			this.IsSummerTime = null;
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

			foreach (var c in candlesticksM30) {

				if (IsSummerTime!=null && IsSummerTime.Value ==false && est.IsDaylightSavingTime(c.Time)) {
					continue;
				}



				DateTime shiftedTime = c.Time.AddHours(this.ShiftHour);

				if (!oldDate.Equals(shiftedTime.Date)) {
					hmList = new Candlestick[48];
					result.Add(hmList);
					oldDate = shiftedTime.Date;
				}
				hmList[shiftedTime.Hour * 2 + (shiftedTime.Minute == 30 ? 1 : 0)] = c;
			}
			return result;
		}

		public string HMString(int t) {
			t -= this.ShiftHour * 2;
			if(t < 0) {
				t += 48;
			}
			return (t / 2 < 10 ? "0" : "") + t / 2 + ":" + (t % 2 == 0 ? "00" : "30");
		}

		public IEnumerable<Tuple<int[], int[]>> Calculate( int limit) {

			List<Candlestick[]> candlesticksEachDate = GetCandlesticksEachDate();
			List<Tuple<int[], int[]>> summary = new List<Tuple<int[], int[]>>();
			for (int s1 = 0; s1 < 48 - 12; s1++) {
				for (int e1 = s1; e1 < s1 + 12; e1++) {
					for (int s2 = e1 + 1; s2 < 48 - 12; s2++) {
						for (int e2 = s2; e2 < s2 + 12; e2++) {
							int[] dayResult = new int[4];
							summary.Add(new Tuple<int[], int[]>(new int[] { s1, e1, s2, e2 }, dayResult));
							foreach (var hml in candlesticksEachDate) {
								try {
									float d1 = hml[e1].Close - hml[s1].Open;
									float d2 = hml[e2].Close - hml[s2].Open;

									if (hml[e1].Close == 0 || hml[s1].Open == 0 || hml[s2].Open == 0 || hml[e2].Close == 0) {
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
