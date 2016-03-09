namespace Candlesticks {
	partial class SignalForm {
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
			this.signalDataGrid = new System.Windows.Forms.DataGridView();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.signalDataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// signalDataGrid
			// 
			this.signalDataGrid.AllowUserToAddRows = false;
			this.signalDataGrid.AllowUserToDeleteRows = false;
			this.signalDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.signalDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.signalDataGrid.Location = new System.Drawing.Point(0, 0);
			this.signalDataGrid.MultiSelect = false;
			this.signalDataGrid.Name = "signalDataGrid";
			this.signalDataGrid.RowTemplate.Height = 21;
			this.signalDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.signalDataGrid.ShowEditingIcon = false;
			this.signalDataGrid.Size = new System.Drawing.Size(742, 377);
			this.signalDataGrid.TabIndex = 0;
			this.signalDataGrid.TabStop = false;
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// SignalForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(742, 377);
			this.Controls.Add(this.signalDataGrid);
			this.Name = "SignalForm";
			this.Text = "SignalForm";
			this.Load += new System.EventHandler(this.SignalForm_Load);
			this.Shown += new System.EventHandler(this.SignalForm_Shown);
			((System.ComponentModel.ISupportInitialize)(this.signalDataGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView signalDataGrid;
		private System.Windows.Forms.Timer timer1;
	}
}