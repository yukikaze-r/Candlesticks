using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Candlesticks {
	class Report : IDisposable {
		private StreamWriter writer = null;
		private int columnIndex = 0;
		private int rowIndex = 0;

		public Report() {
		}

		public DataGridView DataGridView {
			get;
			set;
		}

		public Control StatusControl {
			get;
			set;
		}

		public string BasePath {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public int Version {
			get;
			set;
		}

		public string Comment {
			get;
			set;
		}

		public bool IsForceOverride {
			get;
			set;
		}

		public string CsvFilePath {
			get {
				return this.BasePath + "\\" + this.Name + "-" + this.Version + "-" + this.Comment + ".csv";
			}
		}


		public void SetHeader(params string[] args) {
			string path = this.CsvFilePath;
			if (!this.IsForceOverride && File.Exists(path)) {
				LoadReport(path);
				throw new ReportExistedException();
			}
			writer = new StreamWriter(path, false, Encoding.GetEncoding("Shift_JIS"));
			this.DataGridView.Invoke(new Action(() => {
				this.DataGridView.Columns.Clear();
				this.DataGridView.Rows.Clear();
				foreach (string arg in args) {
					this.DataGridView.Columns.Add(arg, arg);
				}
			}));
			foreach (var a in args) {
				writer.Write(a);
				writer.Write(',');
				Console.Write(a);
				Console.Write(' ');
			}
			writer.WriteLine();
			Console.WriteLine();

			this.StatusControl.Invoke(new Action(() => {
				this.StatusControl.Text = "title:" + this.Name + " version:" + this.Version + " comment:" + this.Comment + " status:Running...";
			}));
		}

		private void LoadReport(string path) {
			using (StreamReader reader = new StreamReader(path, Encoding.GetEncoding("Shift_JIS"))) {
				this.DataGridView.Invoke(new Action(() => {
					this.DataGridView.Columns.Clear();
					this.DataGridView.Rows.Clear();
					foreach (var header in reader.ReadLine().Split(',')) {
						Console.Write(header);
						Console.Write(' ');
						this.DataGridView.Columns.Add(header, header);
					}
				}));
				string line = null;
				while ((line = reader.ReadLine()) != null) {
					WriteLine(line.Split(','));
				}
			}
		}

		public void WriteLine(params object[] args) {
			Write(args);
			WriteLine();
		}

		public void Write(params object[] args) {
			foreach (var a in args) {
				if(writer != null) {
					writer.Write(a);
					writer.Write(',');
				}
				Console.Write(a);
				Console.Write(' ');
			}
			this.DataGridView.Invoke(new Action(() => {
				if(columnIndex == 0) {
					this.DataGridView.Rows.Add();
				}
				foreach (var a in args) {
					this.DataGridView.Rows[rowIndex].Cells[columnIndex++].Value = a;
				}}));
		}

		public void WriteLine() {
			if (writer != null) {
				writer.WriteLine();
			}
			Console.WriteLine();
			columnIndex = 0;
			rowIndex++;
		}

		public static void RunTask(object sender, Action<Report> action, DataGridView dataGridView, Control status) {
			Task.Run(() => {
				using (var report = new Report() {
					Name = ((Control)sender).Text,
					BasePath = Setting.Instance.DataFilePath,
					DataGridView = dataGridView,
					StatusControl = status,
				}) {
					try {
						action(report);
					} catch (ReportExistedException) {
					}
				}
			});
		}

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing) {
			this.StatusControl.Invoke(new Action(() => {
				if(writer == null) {
					this.StatusControl.Text = "title:" + this.Name + " version:" + this.Version + " comment:" + this.Comment + " status:Loaded!";
				} else {
					this.StatusControl.Text = "title:" + this.Name + " version:" + this.Version + " comment:" + this.Comment + " status:Finished!";
				}
			}));
			if (!disposedValue) {
				if (disposing) {
					if(writer != null) {
						writer.Close();
						writer.Dispose();
						writer = null;
					}
				}

				disposedValue = true;
			}
		}
		
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}

	class ReportExistedException : Exception {

	}
}
