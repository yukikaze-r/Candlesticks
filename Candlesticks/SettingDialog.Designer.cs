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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dbPassword = new System.Windows.Forms.TextBox();
			this.dbPort = new System.Windows.Forms.TextBox();
			this.dbHost = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.oandaAccountId = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// oandaBearerToken
			// 
			this.oandaBearerToken.Location = new System.Drawing.Point(86, 49);
			this.oandaBearerToken.Name = "oandaBearerToken";
			this.oandaBearerToken.Size = new System.Drawing.Size(333, 19);
			this.oandaBearerToken.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(74, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "Bearer Token";
			// 
			// dataFilePath
			// 
			this.dataFilePath.Location = new System.Drawing.Point(104, 33);
			this.dataFilePath.Name = "dataFilePath";
			this.dataFilePath.Size = new System.Drawing.Size(333, 19);
			this.dataFilePath.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "Data File Path";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(377, 261);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dbPassword);
			this.groupBox1.Controls.Add(this.dbPort);
			this.groupBox1.Controls.Add(this.dbHost);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(18, 169);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(434, 86);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "DBConnection";
			// 
			// dbPassword
			// 
			this.dbPassword.Location = new System.Drawing.Point(86, 49);
			this.dbPassword.Name = "dbPassword";
			this.dbPassword.PasswordChar = '*';
			this.dbPassword.Size = new System.Drawing.Size(333, 19);
			this.dbPassword.TabIndex = 5;
			// 
			// dbPort
			// 
			this.dbPort.Location = new System.Drawing.Point(286, 21);
			this.dbPort.Name = "dbPort";
			this.dbPort.Size = new System.Drawing.Size(133, 19);
			this.dbPort.TabIndex = 4;
			// 
			// dbHost
			// 
			this.dbHost.Location = new System.Drawing.Point(86, 22);
			this.dbHost.Name = "dbHost";
			this.dbHost.Size = new System.Drawing.Size(148, 19);
			this.dbHost.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(9, 52);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(54, 12);
			this.label5.TabIndex = 2;
			this.label5.Text = "Password";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(254, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(26, 12);
			this.label4.TabIndex = 1;
			this.label4.Text = "Port";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 25);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "Host";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.oandaAccountId);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.oandaBearerToken);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(18, 68);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(434, 86);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "OANDA";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(7, 25);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(62, 12);
			this.label6.TabIndex = 2;
			this.label6.Text = "Account ID";
			// 
			// oandaAccountId
			// 
			this.oandaAccountId.Location = new System.Drawing.Point(86, 18);
			this.oandaAccountId.Name = "oandaAccountId";
			this.oandaAccountId.Size = new System.Drawing.Size(333, 19);
			this.oandaAccountId.TabIndex = 3;
			// 
			// SettingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(473, 299);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.dataFilePath);
			this.Name = "SettingDialog";
			this.Text = "SettingDialog";
			this.Load += new System.EventHandler(this.SettingDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox oandaBearerToken;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox dataFilePath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox dbPassword;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox dbHost;
		private System.Windows.Forms.TextBox dbPort;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox oandaAccountId;
		private System.Windows.Forms.Label label6;
	}
}