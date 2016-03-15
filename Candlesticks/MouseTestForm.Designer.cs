namespace Candlesticks {
	partial class MouseTestForm {
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
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.label2 = new System.Windows.Forms.Label();
			this.bidCursorPosLabelUsdJpy = new System.Windows.Forms.Label();
			this.askCursorPosLabelUsdJpy = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.settleCursorPosLabelUsdJpy = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.settleCursorPosLabelEurUsd = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.askCursorPosLabelEurUsd = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.bidCursorPosLabelEurUsd = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(38, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "[b]bid";
			// 
			// bidCursorPosLabelUsdJpy
			// 
			this.bidCursorPosLabelUsdJpy.AutoSize = true;
			this.bidCursorPosLabelUsdJpy.Location = new System.Drawing.Point(98, 54);
			this.bidCursorPosLabelUsdJpy.Name = "bidCursorPosLabelUsdJpy";
			this.bidCursorPosLabelUsdJpy.Size = new System.Drawing.Size(35, 12);
			this.bidCursorPosLabelUsdJpy.TabIndex = 3;
			this.bidCursorPosLabelUsdJpy.Text = "label3";
			// 
			// askCursorPosLabelUsdJpy
			// 
			this.askCursorPosLabelUsdJpy.AutoSize = true;
			this.askCursorPosLabelUsdJpy.Location = new System.Drawing.Point(98, 78);
			this.askCursorPosLabelUsdJpy.Name = "askCursorPosLabelUsdJpy";
			this.askCursorPosLabelUsdJpy.Size = new System.Drawing.Size(35, 12);
			this.askCursorPosLabelUsdJpy.TabIndex = 5;
			this.askCursorPosLabelUsdJpy.Text = "label4";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(38, 78);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 12);
			this.label5.TabIndex = 4;
			this.label5.Text = "[a]ask";
			// 
			// settleCursorPosLabelUsdJpy
			// 
			this.settleCursorPosLabelUsdJpy.AutoSize = true;
			this.settleCursorPosLabelUsdJpy.Location = new System.Drawing.Point(98, 102);
			this.settleCursorPosLabelUsdJpy.Name = "settleCursorPosLabelUsdJpy";
			this.settleCursorPosLabelUsdJpy.Size = new System.Drawing.Size(35, 12);
			this.settleCursorPosLabelUsdJpy.TabIndex = 7;
			this.settleCursorPosLabelUsdJpy.Text = "label6";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(38, 102);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(48, 12);
			this.label7.TabIndex = 6;
			this.label7.Text = "[s]settle";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(191, 97);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 23);
			this.button4.TabIndex = 12;
			this.button4.Text = "カーソル移動";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(191, 73);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(75, 23);
			this.button5.TabIndex = 11;
			this.button5.Text = "カーソル移動";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(191, 49);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(75, 23);
			this.button6.TabIndex = 10;
			this.button6.Text = "カーソル移動";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(24, 35);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(254, 94);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "USD_JPY";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(191, 204);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 22;
			this.button1.Text = "カーソル移動";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(191, 180);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 21;
			this.button2.Text = "カーソル移動";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(191, 156);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 20;
			this.button3.Text = "カーソル移動";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// settleCursorPosLabelEurUsd
			// 
			this.settleCursorPosLabelEurUsd.AutoSize = true;
			this.settleCursorPosLabelEurUsd.Location = new System.Drawing.Point(98, 209);
			this.settleCursorPosLabelEurUsd.Name = "settleCursorPosLabelEurUsd";
			this.settleCursorPosLabelEurUsd.Size = new System.Drawing.Size(35, 12);
			this.settleCursorPosLabelEurUsd.TabIndex = 19;
			this.settleCursorPosLabelEurUsd.Text = "label6";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(38, 209);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(48, 12);
			this.label4.TabIndex = 18;
			this.label4.Text = "[3]settle";
			// 
			// askCursorPosLabelEurUsd
			// 
			this.askCursorPosLabelEurUsd.AutoSize = true;
			this.askCursorPosLabelEurUsd.Location = new System.Drawing.Point(98, 185);
			this.askCursorPosLabelEurUsd.Name = "askCursorPosLabelEurUsd";
			this.askCursorPosLabelEurUsd.Size = new System.Drawing.Size(35, 12);
			this.askCursorPosLabelEurUsd.TabIndex = 17;
			this.askCursorPosLabelEurUsd.Text = "label4";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(38, 185);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(37, 12);
			this.label8.TabIndex = 16;
			this.label8.Text = "[2]ask";
			// 
			// bidCursorPosLabelEurUsd
			// 
			this.bidCursorPosLabelEurUsd.AutoSize = true;
			this.bidCursorPosLabelEurUsd.Location = new System.Drawing.Point(98, 161);
			this.bidCursorPosLabelEurUsd.Name = "bidCursorPosLabelEurUsd";
			this.bidCursorPosLabelEurUsd.Size = new System.Drawing.Size(35, 12);
			this.bidCursorPosLabelEurUsd.TabIndex = 15;
			this.bidCursorPosLabelEurUsd.Text = "label3";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(38, 161);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(34, 12);
			this.label10.TabIndex = 14;
			this.label10.Text = "[1]bid";
			// 
			// groupBox2
			// 
			this.groupBox2.Location = new System.Drawing.Point(24, 142);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(254, 94);
			this.groupBox2.TabIndex = 23;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "EUR_USD";
			// 
			// MouseTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(309, 249);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.settleCursorPosLabelEurUsd);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.askCursorPosLabelEurUsd);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.bidCursorPosLabelEurUsd);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.settleCursorPosLabelUsdJpy);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.askCursorPosLabelUsdJpy);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.bidCursorPosLabelUsdJpy);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.KeyPreview = true;
			this.Name = "MouseTestForm";
			this.Text = "MouseTestForm";
			this.Load += new System.EventHandler(this.MouseTestForm_Load);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MouseTestForm_KeyPress);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label bidCursorPosLabelUsdJpy;
		private System.Windows.Forms.Label askCursorPosLabelUsdJpy;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label settleCursorPosLabelUsdJpy;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label settleCursorPosLabelEurUsd;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label askCursorPosLabelEurUsd;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label bidCursorPosLabelEurUsd;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox2;
	}
}