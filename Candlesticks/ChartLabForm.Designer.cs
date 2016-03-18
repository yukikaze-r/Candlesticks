namespace Candlesticks {
	partial class ChartLabForm {
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
			this.急変戻り戻り = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.label1 = new System.Windows.Forms.Label();
			this.急変日時 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// 急変戻り戻り
			// 
			this.急変戻り戻り.Location = new System.Drawing.Point(13, 30);
			this.急変戻り戻り.Name = "急変戻り戻り";
			this.急変戻り戻り.Size = new System.Drawing.Size(123, 23);
			this.急変戻り戻り.TabIndex = 0;
			this.急変戻り戻り.Text = "急変戻り戻り";
			this.急変戻り戻り.UseVisualStyleBackColor = true;
			this.急変戻り戻り.Click += new System.EventHandler(this.急変戻り戻り_Click);
			// 
			// dataGridView1
			// 
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(-1, 158);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 21;
			this.dataGridView1.Size = new System.Drawing.Size(782, 309);
			this.dataGridView1.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(-1, 140);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 12);
			this.label1.TabIndex = 2;
			this.label1.Text = "label1";
			// 
			// 急変日時
			// 
			this.急変日時.Location = new System.Drawing.Point(168, 30);
			this.急変日時.Name = "急変日時";
			this.急変日時.Size = new System.Drawing.Size(75, 23);
			this.急変日時.TabIndex = 3;
			this.急変日時.Text = "急変日時";
			this.急変日時.UseVisualStyleBackColor = true;
			this.急変日時.Click += new System.EventHandler(this.急変日時_Click);
			// 
			// ChartLabForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(776, 459);
			this.Controls.Add(this.急変日時);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.急変戻り戻り);
			this.Name = "ChartLabForm";
			this.Text = "ChartLabForm";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button 急変戻り戻り;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button 急変日時;
	}
}