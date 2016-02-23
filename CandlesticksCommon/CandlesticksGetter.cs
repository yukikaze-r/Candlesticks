using System;
using System.Collections.Generic;
using System.Text;

namespace Candlesticks
{
    class CandlesticksGetter
    {
		public string Instrument = "USD_JPY";

		/*
			１分の初めにアライン
			“S5” - 5 秒
			“S10” - 10 秒
			“S15” - 15 秒
			“S30” - 30 秒
			“M1” - 1 分
			1時間の初めにアライン
			“M2” - 2 分
			“M3” - 3 分
			“M5” - 5 分
			“M10” - 10 分
			“M15” - 15 分
			“M30” - 30 分
			“H1” - 1 時間
			1日の初めにアライン(17:00, 米国東部標準時)
			“H2” - 2 時間
			“H3” - 3 時間
			“H4” - 4 時間
			“H6” - 6 時間
			“H8” - 8 時間
			“H12” - 12 時間
			“D” - 1 日
			1週間の初めにアライン (土曜日)
			“W” - 1 週
			1か月の初めにアライン (その月の最初の日)
			“M” - 1 か月
		*/
		public string Granularity = "M30";
		public DateTime Start;
		public DateTime End;

/*
		public IEnumerable<Candlestick> Execute() {
			



		}*/
		private DateTime GetAlignTime(DateTime time) {
			if(Granularity[0]=='S') {
				int s = int.Parse(Granularity.Substring(1));
				int aligned = (int)Math.Ceiling((double)time.Second / s) * s;
				return time.AddMilliseconds(-time.Millisecond).AddSeconds(-time.Second + aligned);
			} else if (Granularity[0] == 'M') {
				int s = int.Parse(Granularity.Substring(1));
				int aligned = time.Minute / s * s;
				return time.AddMilliseconds(-time.Millisecond).AddSeconds(-time.Second).AddMinutes(-time.Minute + aligned);
			}


			throw new Exception();
		}

	}
}
