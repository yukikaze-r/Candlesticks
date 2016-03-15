using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Candlesticks {
	public partial class MouseTestForm : Form {

		[DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
		static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		private const int MOUSEEVENTF_LEFTDOWN = 0x2;
		private const int MOUSEEVENTF_LEFTUP = 0x4;
		
		public MouseTestForm() {
			InitializeComponent();
		}
		
		private void timer1_Tick(object sender, EventArgs e) {
		}

		private void button1_Click(object sender, EventArgs e) {
			Cursor.Position = new Point(2375, 756);
			mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
		}
		
		private void MouseTestForm_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == 'b') {
				Setting.Instance.MouseClickPositionUSD_JPY.Bid = Cursor.Position;
				bidCursorPosLabelUsdJpy.Text = Cursor.Position.ToString();
			}
			if (e.KeyChar == 'a') {
				Setting.Instance.MouseClickPositionUSD_JPY.Ask = Cursor.Position;
				askCursorPosLabelUsdJpy.Text = Cursor.Position.ToString();
			}
			if (e.KeyChar == 's') {
				Setting.Instance.MouseClickPositionUSD_JPY.Settle = Cursor.Position;
				settleCursorPosLabelUsdJpy.Text = Cursor.Position.ToString();
			}
			if (e.KeyChar == '1') {
				Setting.Instance.MouseClickPositionEUR_USD.Bid = Cursor.Position;
				bidCursorPosLabelEurUsd.Text = Cursor.Position.ToString();
			}
			if (e.KeyChar == '2') {
				Setting.Instance.MouseClickPositionEUR_USD.Ask = Cursor.Position;
				askCursorPosLabelEurUsd.Text = Cursor.Position.ToString();
			}
			if (e.KeyChar == '3') {
				Setting.Instance.MouseClickPositionEUR_USD.Settle = Cursor.Position;
				settleCursorPosLabelEurUsd.Text = Cursor.Position.ToString();
			}
			Setting.Instance.Save();
		}

		private void button6_Click(object sender, EventArgs e) {
			Cursor.Position = Setting.Instance.MouseClickPositionUSD_JPY.Bid;
		}

		private void button5_Click(object sender, EventArgs e) {
			Cursor.Position = Setting.Instance.MouseClickPositionUSD_JPY.Ask;

		}

		private void button4_Click(object sender, EventArgs e) {
			Cursor.Position = Setting.Instance.MouseClickPositionUSD_JPY.Settle;

		}

		private void button3_Click(object sender, EventArgs e) {
			Cursor.Position = Setting.Instance.MouseClickPositionEUR_USD.Bid;

		}

		private void button2_Click(object sender, EventArgs e) {
			Cursor.Position = Setting.Instance.MouseClickPositionEUR_USD.Ask;

		}

		private void button1_Click_1(object sender, EventArgs e) {
			Cursor.Position = Setting.Instance.MouseClickPositionEUR_USD.Settle;
		}

		private void MouseTestForm_Load(object sender, EventArgs e) {
			bidCursorPosLabelUsdJpy.Text = Setting.Instance.MouseClickPositionUSD_JPY.Bid.ToString();
			askCursorPosLabelUsdJpy.Text = Setting.Instance.MouseClickPositionUSD_JPY.Ask.ToString();
			settleCursorPosLabelUsdJpy.Text = Setting.Instance.MouseClickPositionUSD_JPY.Settle.ToString();

			bidCursorPosLabelEurUsd.Text = Setting.Instance.MouseClickPositionEUR_USD.Bid.ToString();
			askCursorPosLabelEurUsd.Text = Setting.Instance.MouseClickPositionEUR_USD.Ask.ToString();
			settleCursorPosLabelEurUsd.Text = Setting.Instance.MouseClickPositionEUR_USD.Settle.ToString();
		}

	}
}
