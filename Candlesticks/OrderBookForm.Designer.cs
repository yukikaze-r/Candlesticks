namespace Candlesticks {
	partial class OrderBookForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.orderBookList = new System.Windows.Forms.ListBox();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.timeLabel = new System.Windows.Forms.Label();
			this.bidLabel = new System.Windows.Forms.Label();
			this.askLabel = new System.Windows.Forms.Label();
			this.volumeLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(128, 34);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.AutoScroll = true;
			this.splitContainer1.Panel1.Controls.Add(this.chart1);
			this.splitContainer1.Panel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.splitContainer1_Panel1_Scroll);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.Controls.Add(this.splitter1);
			this.splitContainer1.Panel2.Controls.Add(this.chart2);
			this.splitContainer1.Panel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.splitContainer1_Panel2_Scroll);
			this.splitContainer1.Size = new System.Drawing.Size(1146, 835);
			this.splitContainer1.SplitterDistance = 414;
			this.splitContainer1.TabIndex = 8;
			// 
			// chart1
			// 
			chartArea5.Name = "ChartArea1";
			this.chart1.ChartAreas.Add(chartArea5);
			legend5.Name = "Legend1";
			this.chart1.Legends.Add(legend5);
			this.chart1.Location = new System.Drawing.Point(3, 3);
			this.chart1.Name = "chart1";
			series5.ChartArea = "ChartArea1";
			series5.Legend = "Legend1";
			series5.Name = "Series1";
			this.chart1.Series.Add(series5);
			this.chart1.Size = new System.Drawing.Size(797, 234);
			this.chart1.TabIndex = 0;
			this.chart1.TabStop = false;
			this.chart1.Text = "chart1";
			this.chart1.Click += new System.EventHandler(this.chart1_Click_1);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(0, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 417);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// chart2
			// 
			chartArea6.Name = "ChartArea1";
			this.chart2.ChartAreas.Add(chartArea6);
			legend6.Name = "Legend1";
			this.chart2.Legends.Add(legend6);
			this.chart2.Location = new System.Drawing.Point(0, 3);
			this.chart2.Name = "chart2";
			series6.ChartArea = "ChartArea1";
			series6.Legend = "Legend1";
			series6.Name = "Series1";
			this.chart2.Series.Add(series6);
			this.chart2.Size = new System.Drawing.Size(842, 225);
			this.chart2.TabIndex = 0;
			this.chart2.TabStop = false;
			this.chart2.Text = "chart2";
			// 
			// orderBookList
			// 
			this.orderBookList.Dock = System.Windows.Forms.DockStyle.Left;
			this.orderBookList.FormattingEnabled = true;
			this.orderBookList.ItemHeight = 12;
			this.orderBookList.Location = new System.Drawing.Point(0, 34);
			this.orderBookList.Name = "orderBookList";
			this.orderBookList.Size = new System.Drawing.Size(118, 835);
			this.orderBookList.TabIndex = 4;
			this.orderBookList.SelectedIndexChanged += new System.EventHandler(this.orderBookList_SelectedIndexChanged);
			// 
			// splitter2
			// 
			this.splitter2.Location = new System.Drawing.Point(118, 34);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(10, 835);
			this.splitter2.TabIndex = 6;
			this.splitter2.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.volumeLabel);
			this.panel1.Controls.Add(this.askLabel);
			this.panel1.Controls.Add(this.bidLabel);
			this.panel1.Controls.Add(this.timeLabel);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1274, 34);
			this.panel1.TabIndex = 9;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(145, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(22, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "Bid";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(260, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(25, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "Ask";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(372, 12);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(43, 12);
			this.label3.TabIndex = 2;
			this.label3.Text = "Volume";
			// 
			// timeLabel
			// 
			this.timeLabel.AutoSize = true;
			this.timeLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.timeLabel.Location = new System.Drawing.Point(14, 9);
			this.timeLabel.Name = "timeLabel";
			this.timeLabel.Size = new System.Drawing.Size(127, 16);
			this.timeLabel.TabIndex = 3;
			this.timeLabel.Text = "XX/XX XX:XX:XX";
			// 
			// bidLabel
			// 
			this.bidLabel.AutoSize = true;
			this.bidLabel.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.bidLabel.Location = new System.Drawing.Point(168, 9);
			this.bidLabel.Name = "bidLabel";
			this.bidLabel.Size = new System.Drawing.Size(86, 19);
			this.bidLabel.TabIndex = 4;
			this.bidLabel.Text = "XXX.XXX";
			// 
			// askLabel
			// 
			this.askLabel.AutoSize = true;
			this.askLabel.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.askLabel.Location = new System.Drawing.Point(284, 9);
			this.askLabel.Name = "askLabel";
			this.askLabel.Size = new System.Drawing.Size(86, 19);
			this.askLabel.TabIndex = 5;
			this.askLabel.Text = "XXX.XXX";
			// 
			// volumeLabel
			// 
			this.volumeLabel.AutoSize = true;
			this.volumeLabel.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.volumeLabel.Location = new System.Drawing.Point(412, 8);
			this.volumeLabel.Name = "volumeLabel";
			this.volumeLabel.Size = new System.Drawing.Size(45, 19);
			this.volumeLabel.TabIndex = 6;
			this.volumeLabel.Text = "XXX";
			this.volumeLabel.Click += new System.EventHandler(this.volumeLabel_Click);
			// 
			// OrderBookForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1274, 869);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.orderBookList);
			this.Controls.Add(this.panel1);
			this.Name = "OrderBookForm";
			this.Text = "OrderBookForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OrderBookForm_FormClosing);
			this.Load += new System.EventHandler(this.OrderBookForm_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
		private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ListBox orderBookList;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label volumeLabel;
		private System.Windows.Forms.Label askLabel;
		private System.Windows.Forms.Label bidLabel;
		private System.Windows.Forms.Label timeLabel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}