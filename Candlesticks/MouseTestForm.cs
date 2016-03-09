﻿using System;
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

		private bool task1 = true;
		private bool task2 = true;
		private bool task3 = false;
		private bool task4 = false;

		private void timer1_Tick(object sender, EventArgs e) {
			label1.Text = Cursor.Position.X +"," + Cursor.Position.Y;

			DateTime now = DateTime.Now;
			if(now.Hour == 6 && now.Minute == 0 && task1 == false) {
				Cursor.Position = Setting.Instance.MouseClickPosition.Ask;
				mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
				mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
				task1 = true;
			}
			if (now.Hour == 7 && now.Minute == 10 && task2 == false) {
				Cursor.Position = Setting.Instance.MouseClickPosition.Settle;
				mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
				mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
				task2 = true;
			}


			if (now.Hour == 11 && now.Minute == 0 && task3 == false) {
				Cursor.Position = Setting.Instance.MouseClickPosition.Bid;
				mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
				mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
				task3 = true;
			}
			if (now.Hour == 11 && now.Minute == 29 && task4 == false) {
				Cursor.Position = Setting.Instance.MouseClickPosition.Settle;
				mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
				mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
				task4 = true;
			}
		}

		private void button1_Click(object sender, EventArgs e) {
			Cursor.Position = new Point(2375, 756);
			mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
		}
		
		private void MouseTestForm_KeyPress(object sender, KeyPressEventArgs e) {
			if (e.KeyChar == 'b') {
				Setting.Instance.MouseClickPosition.Bid = Cursor.Position;
				bidCursorPosLabel.Text = Cursor.Position.ToString();
			}
			if (e.KeyChar == 'a') {
				Setting.Instance.MouseClickPosition.Ask = Cursor.Position;
				askCursorPosLabel.Text = Cursor.Position.ToString();
			}
			if (e.KeyChar == 's') {
				Setting.Instance.MouseClickPosition.Settle = Cursor.Position;
				settleCursorPosLabel.Text = Cursor.Position.ToString();
			}
			Setting.Instance.Save();
		}

		private void button6_Click(object sender, EventArgs e) {
			Cursor.Position = Setting.Instance.MouseClickPosition.Bid;
		}

		private void button5_Click(object sender, EventArgs e) {
			Cursor.Position = Setting.Instance.MouseClickPosition.Ask;

		}

		private void button4_Click(object sender, EventArgs e) {
			Cursor.Position = Setting.Instance.MouseClickPosition.Settle;

		}

		private void MouseTestForm_Load(object sender, EventArgs e) {
			bidCursorPosLabel.Text = Setting.Instance.MouseClickPosition.Bid.ToString();
			askCursorPosLabel.Text = Setting.Instance.MouseClickPosition.Ask.ToString();
			settleCursorPosLabel.Text = Setting.Instance.MouseClickPosition.Settle.ToString();
		}
	}
}
