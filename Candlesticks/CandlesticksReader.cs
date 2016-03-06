using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Candlesticks {
	class CandlesticksReader {
		public IEnumerable<Candlestick> Read(string filePath) {
			var file = new StreamReader(filePath);
			string line;
			while(true) {
				line = file.ReadLine();
				if(line == null) {
					break;
				}
				var e = line.Split(',');
				Candlestick c = new Candlestick();
				c.DateTime = DateTime.Parse(e[0]+" "+e[1]);
				c.Open = float.Parse(e[2]);
				c.High = float.Parse(e[3]);
				c.Low = float.Parse(e[4]);
				c.Close = float.Parse(e[5]);
				yield return c;
			}
			file.Close();
		}
	}
}
