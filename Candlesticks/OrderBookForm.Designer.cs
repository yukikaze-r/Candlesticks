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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea11 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend11 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea12 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend12 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.orderBookList = new System.Windows.Forms.ListBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.panel1 = new System.Windows.Forms.Panel();
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
			chartArea11.Name = "ChartArea1";
			this.chart1.ChartAreas.Add(chartArea11);
			legend11.Name = "Legend1";
			this.chart1.Legends.Add(legend11);
			this.chart1.Location = new System.Drawing.Point(3, 3);
			this.chart1.Name = "chart1";
			series11.ChartArea = "ChartArea1";
			series11.Legend = "Legend1";
			series11.Name = "Series1";
			this.chart1.Series.Add(series11);
			this.chart1.Size = new System.Drawing.Size(797, 234);
			this.chart1.TabIndex = 0;
			this.chart1.TabStop = false;
			this.chart1.Text = "chart1";
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
			chartArea12.Name = "ChartArea1";
			this.chart2.ChartAreas.Add(chartArea12);
			legend12.Name = "Legend1";
			this.chart2.Legends.Add(legend12);
			this.chart2.Location = new System.Drawing.Point(0, 3);
			this.chart2.Name = "chart2";
			series12.ChartArea = "ChartArea1";
			series12.Legend = "Legend1";
			series12.Name = "Series1";
			this.chart2.Series.Add(series12);
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
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(12, 12);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(72, 16);
			this.checkBox1.TabIndex = 5;
			this.checkBox1.Text = "自動更新";
			this.checkBox1.UseVisualStyleBackColor = true;
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
			this.panel1.Controls.Add(this.checkBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1274, 34);
			this.panel1.TabIndex = 9;
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
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel panel1;
	}
}