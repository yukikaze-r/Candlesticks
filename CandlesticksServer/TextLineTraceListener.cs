using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Candlesticks {
	class TextLineTraceListener : TraceListener {
		private FileStream fs;
		private StreamWriter writer;
		private bool isNewLine = true;

		public TextLineTraceListener(string fileName) {
			fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			writer = new StreamWriter(fs);
		}

		private string GetHeader() {
			DateTime now = DateTime.Now;
			return "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:") + String.Format("{0:D3}",now.Millisecond) + "] ";
		}

		public override void Write(string message) {
			lock (fs) {
				if (isNewLine) {
					writer.Write(GetHeader());
					isNewLine = false;
				}
				writer.Write(message);
				writer.Flush();
			}
		}

		public override void WriteLine(string message) {
			lock(fs) {
				if (isNewLine) {
					writer.Write(GetHeader());
				}
				writer.WriteLine(message);
				writer.Flush();
				isNewLine = true;
			}
		}


		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			writer.Dispose();
		}
	}
}
