namespace Candlesticks {
	partial class CauseAndEffectForm {
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
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.taskStatus = new System.Windows.Forms.Label();
			this.因果 = new System.Windows.Forms.Button();
			this.因果EUR_USD = new System.Windows.Forms.Button();
			this.因果USD_JPY = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(-5, 81);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 21;
			this.dataGridView1.Size = new System.Drawing.Size(846, 372);
			this.dataGridView1.TabIndex = 0;
			// 
			// taskStatus
			// 
			this.taskStatus.AutoSize = true;
			this.taskStatus.Location = new System.Drawing.Point(2, 198);
			this.taskStatus.Name = "taskStatus";
			this.taskStatus.Size = new System.Drawing.Size(35, 12);
			this.taskStatus.TabIndex = 1;
			this.taskStatus.Text = "label1";
			// 
			// 因果
			// 
			this.因果.Location = new System.Drawing.Point(12, 21);
			this.因果.Name = "因果";
			this.因果.Size = new System.Drawing.Size(75, 23);
			this.因果.TabIndex = 2;
			this.因果.Text = "因果";
			this.因果.UseVisualStyleBackColor = true;
			this.因果.Click += new System.EventHandler(this.button1_Click);
			// 
			// 因果EUR_USD
			// 
			this.因果EUR_USD.Location = new System.Drawing.Point(118, 21);
			this.因果EUR_USD.Name = "因果EUR_USD";
			this.因果EUR_USD.Size = new System.Drawing.Size(75, 23);
			this.因果EUR_USD.TabIndex = 3;
			this.因果EUR_USD.Text = "EUR_USD";
			this.因果EUR_USD.UseVisualStyleBackColor = true;
			this.因果EUR_USD.Click += new System.EventHandler(this.因果EUR_USD_Click);
			// 
			// 因果USD_JPY
			// 
			this.因果USD_JPY.Location = new System.Drawing.Point(227, 21);
			this.因果USD_JPY.Name = "因果USD_JPY";
			this.因果USD_JPY.Size = new System.Drawing.Size(75, 23);
			this.因果USD_JPY.TabIndex = 4;
			this.因果USD_JPY.Text = "USD_JPY";
			this.因果USD_JPY.UseVisualStyleBackColor = true;
			this.因果USD_JPY.Click += new System.EventHandler(this.因果USD_JPY_Click);
			// 
			// CauseAndEffectForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(838, 454);
			this.Controls.Add(this.因果USD_JPY);
			this.Controls.Add(this.因果EUR_USD);
			this.Controls.Add(this.因果);
			this.Controls.Add(this.taskStatus);
			this.Controls.Add(this.dataGridView1);
			this.Name = "CauseAndEffectForm";
			this.Text = "CauseAndEffectForm";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Label taskStatus;
		private System.Windows.Forms.Button 因果;
		private System.Windows.Forms.Button 因果EUR_USD;
		private System.Windows.Forms.Button 因果USD_JPY;
	}
}