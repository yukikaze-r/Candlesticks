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
			this.bidCursorPosLabel = new System.Windows.Forms.Label();
			this.askCursorPosLabel = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.settleCursorPosLabel = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
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
			this.label2.Location = new System.Drawing.Point(31, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "[b]bid";
			// 
			// bidCursorPosLabel
			// 
			this.bidCursorPosLabel.AutoSize = true;
			this.bidCursorPosLabel.Location = new System.Drawing.Point(91, 38);
			this.bidCursorPosLabel.Name = "bidCursorPosLabel";
			this.bidCursorPosLabel.Size = new System.Drawing.Size(35, 12);
			this.bidCursorPosLabel.TabIndex = 3;
			this.bidCursorPosLabel.Text = "label3";
			// 
			// askCursorPosLabel
			// 
			this.askCursorPosLabel.AutoSize = true;
			this.askCursorPosLabel.Location = new System.Drawing.Point(91, 62);
			this.askCursorPosLabel.Name = "askCursorPosLabel";
			this.askCursorPosLabel.Size = new System.Drawing.Size(35, 12);
			this.askCursorPosLabel.TabIndex = 5;
			this.askCursorPosLabel.Text = "label4";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(31, 62);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 12);
			this.label5.TabIndex = 4;
			this.label5.Text = "[a]ask";
			// 
			// settleCursorPosLabel
			// 
			this.settleCursorPosLabel.AutoSize = true;
			this.settleCursorPosLabel.Location = new System.Drawing.Point(91, 86);
			this.settleCursorPosLabel.Name = "settleCursorPosLabel";
			this.settleCursorPosLabel.Size = new System.Drawing.Size(35, 12);
			this.settleCursorPosLabel.TabIndex = 7;
			this.settleCursorPosLabel.Text = "label6";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(31, 86);
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
			this.button4.Location = new System.Drawing.Point(184, 81);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 23);
			this.button4.TabIndex = 12;
			this.button4.Text = "カーソル移動";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(184, 57);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(75, 23);
			this.button5.TabIndex = 11;
			this.button5.Text = "カーソル移動";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(184, 33);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(75, 23);
			this.button6.TabIndex = 10;
			this.button6.Text = "カーソル移動";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// MouseTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(308, 131);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.settleCursorPosLabel);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.askCursorPosLabel);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.bidCursorPosLabel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
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
		private System.Windows.Forms.Label bidCursorPosLabel;
		private System.Windows.Forms.Label askCursorPosLabel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label settleCursorPosLabel;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
	}
}