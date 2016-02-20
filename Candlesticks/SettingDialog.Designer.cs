namespace Candlesticks {
	partial class SettingDialog {
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
			this.oandaBearerToken = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.dataFilePath = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// oandaBearerToken
			// 
			this.oandaBearerToken.Location = new System.Drawing.Point(137, 26);
			this.oandaBearerToken.Name = "oandaBearerToken";
			this.oandaBearerToken.Size = new System.Drawing.Size(312, 19);
			this.oandaBearerToken.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(118, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "OANDA Bearer Token";
			// 
			// dataFilePath
			// 
			this.dataFilePath.Location = new System.Drawing.Point(137, 61);
			this.dataFilePath.Name = "dataFilePath";
			this.dataFilePath.Size = new System.Drawing.Size(312, 19);
			this.dataFilePath.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(52, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "Data File Path";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(374, 157);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// SettingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 203);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.dataFilePath);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.oandaBearerToken);
			this.Name = "SettingDialog";
			this.Text = "SettingDialog";
			this.Load += new System.EventHandler(this.SettingDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox oandaBearerToken;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox dataFilePath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
	}
}