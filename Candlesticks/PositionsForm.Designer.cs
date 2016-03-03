namespace Candlesticks {
	partial class PositionsForm {
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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.previousDayButton = new System.Windows.Forms.Button();
			this.nextDayButton = new System.Windows.Forms.Button();
			this.dateLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
			this.SuspendLayout();
			// 
			// chart1
			// 
			this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			chartArea2.Name = "ChartArea1";
			this.chart1.ChartAreas.Add(chartArea2);
			legend2.Name = "Legend1";
			this.chart1.Legends.Add(legend2);
			this.chart1.Location = new System.Drawing.Point(-12, 37);
			this.chart1.Name = "chart1";
			series2.ChartArea = "ChartArea1";
			series2.Legend = "Legend1";
			series2.Name = "Series1";
			this.chart1.Series.Add(series2);
			this.chart1.Size = new System.Drawing.Size(874, 287);
			this.chart1.TabIndex = 0;
			this.chart1.Text = "chart1";
			// 
			// previousDayButton
			// 
			this.previousDayButton.Location = new System.Drawing.Point(67, 8);
			this.previousDayButton.Name = "previousDayButton";
			this.previousDayButton.Size = new System.Drawing.Size(75, 23);
			this.previousDayButton.TabIndex = 1;
			this.previousDayButton.Text = "前日";
			this.previousDayButton.UseVisualStyleBackColor = true;
			this.previousDayButton.Click += new System.EventHandler(this.previousDayButton_Click);
			// 
			// nextDayButton
			// 
			this.nextDayButton.Location = new System.Drawing.Point(148, 8);
			this.nextDayButton.Name = "nextDayButton";
			this.nextDayButton.Size = new System.Drawing.Size(75, 23);
			this.nextDayButton.TabIndex = 2;
			this.nextDayButton.Text = "次日";
			this.nextDayButton.UseVisualStyleBackColor = true;
			this.nextDayButton.Click += new System.EventHandler(this.nextDayButton_Click);
			// 
			// dateLabel
			// 
			this.dateLabel.AutoSize = true;
			this.dateLabel.Location = new System.Drawing.Point(14, 14);
			this.dateLabel.Name = "dateLabel";
			this.dateLabel.Size = new System.Drawing.Size(35, 12);
			this.dateLabel.TabIndex = 3;
			this.dateLabel.Text = "label1";
			// 
			// PositionsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(859, 323);
			this.Controls.Add(this.dateLabel);
			this.Controls.Add(this.nextDayButton);
			this.Controls.Add(this.previousDayButton);
			this.Controls.Add(this.chart1);
			this.Name = "PositionsForm";
			this.Text = "PositionsForm";
			this.Load += new System.EventHandler(this.PositionsForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
		private System.Windows.Forms.Button previousDayButton;
		private System.Windows.Forms.Button nextDayButton;
		private System.Windows.Forms.Label dateLabel;
	}
}