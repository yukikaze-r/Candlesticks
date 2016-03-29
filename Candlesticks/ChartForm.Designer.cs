namespace Candlesticks {
	partial class ChartForm {
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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.instrument1ComboBox = new System.Windows.Forms.ComboBox();
			this.candleRadioButton = new System.Windows.Forms.RadioButton();
			this.lineRadioButton = new System.Windows.Forms.RadioButton();
			this.instrument2ComboBox = new System.Windows.Forms.ComboBox();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.granularityComboBox = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
			this.SuspendLayout();
			// 
			// instrument1ComboBox
			// 
			this.instrument1ComboBox.FormattingEnabled = true;
			this.instrument1ComboBox.Location = new System.Drawing.Point(13, 11);
			this.instrument1ComboBox.Name = "instrument1ComboBox";
			this.instrument1ComboBox.Size = new System.Drawing.Size(121, 20);
			this.instrument1ComboBox.TabIndex = 0;
			this.instrument1ComboBox.SelectedIndexChanged += new System.EventHandler(this.instrument1ComboBox_SelectedIndexChanged);
			// 
			// candleRadioButton
			// 
			this.candleRadioButton.AutoSize = true;
			this.candleRadioButton.Location = new System.Drawing.Point(434, 15);
			this.candleRadioButton.Name = "candleRadioButton";
			this.candleRadioButton.Size = new System.Drawing.Size(88, 16);
			this.candleRadioButton.TabIndex = 1;
			this.candleRadioButton.TabStop = true;
			this.candleRadioButton.Text = "radioButton1";
			this.candleRadioButton.UseVisualStyleBackColor = true;
			this.candleRadioButton.CheckedChanged += new System.EventHandler(this.candleRadioButton_CheckedChanged);
			// 
			// lineRadioButton
			// 
			this.lineRadioButton.AutoSize = true;
			this.lineRadioButton.Location = new System.Drawing.Point(528, 15);
			this.lineRadioButton.Name = "lineRadioButton";
			this.lineRadioButton.Size = new System.Drawing.Size(88, 16);
			this.lineRadioButton.TabIndex = 2;
			this.lineRadioButton.TabStop = true;
			this.lineRadioButton.Text = "radioButton2";
			this.lineRadioButton.UseVisualStyleBackColor = true;
			this.lineRadioButton.CheckedChanged += new System.EventHandler(this.lineRadioButton_CheckedChanged);
			// 
			// instrument2ComboBox
			// 
			this.instrument2ComboBox.FormattingEnabled = true;
			this.instrument2ComboBox.Location = new System.Drawing.Point(140, 11);
			this.instrument2ComboBox.Name = "instrument2ComboBox";
			this.instrument2ComboBox.Size = new System.Drawing.Size(133, 20);
			this.instrument2ComboBox.TabIndex = 3;
			this.instrument2ComboBox.SelectedIndexChanged += new System.EventHandler(this.instrument2ComboBox_SelectedIndexChanged);
			// 
			// chart1
			// 
			this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			chartArea4.Name = "ChartArea1";
			this.chart1.ChartAreas.Add(chartArea4);
			legend4.Name = "Legend1";
			this.chart1.Legends.Add(legend4);
			this.chart1.Location = new System.Drawing.Point(-1, 40);
			this.chart1.Name = "chart1";
			series4.ChartArea = "ChartArea1";
			series4.Legend = "Legend1";
			series4.Name = "Series1";
			this.chart1.Series.Add(series4);
			this.chart1.Size = new System.Drawing.Size(768, 362);
			this.chart1.TabIndex = 4;
			this.chart1.Text = "chart1";
			// 
			// granularityComboBox
			// 
			this.granularityComboBox.FormattingEnabled = true;
			this.granularityComboBox.Location = new System.Drawing.Point(280, 11);
			this.granularityComboBox.Name = "granularityComboBox";
			this.granularityComboBox.Size = new System.Drawing.Size(109, 20);
			this.granularityComboBox.TabIndex = 5;
			// 
			// ChartForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(765, 399);
			this.Controls.Add(this.granularityComboBox);
			this.Controls.Add(this.chart1);
			this.Controls.Add(this.instrument2ComboBox);
			this.Controls.Add(this.lineRadioButton);
			this.Controls.Add(this.candleRadioButton);
			this.Controls.Add(this.instrument1ComboBox);
			this.Name = "ChartForm";
			this.Text = "ChartForm";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChartForm_FormClosed);
			this.Load += new System.EventHandler(this.ChartForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox instrument1ComboBox;
		private System.Windows.Forms.RadioButton candleRadioButton;
		private System.Windows.Forms.RadioButton lineRadioButton;
		private System.Windows.Forms.ComboBox instrument2ComboBox;
		private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
		private System.Windows.Forms.ComboBox granularityComboBox;
	}
}