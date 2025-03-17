using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace YacsLibrary
{
	public class LogUI : Form
	{
		public LogUI()
		{
			this.Text = "LOG Settings";
			this.MaximizeBox = false;
			this.MinimizeBox = false;
		}
		
		private Dictionary<string,LogLevel> Logs = new Dictionary<string,LogLevel>();
		private int Count;
		
		public int ItemMargin { get; set; } = 20;
		public int LeftMargin { get; set; } = 16;
		public int CaptionLength { get; set; } = 100;
		
		public void Add( LogLevel l, string name )
		{
			Count++;
			Logs[name] = l;
			var caption    = new Label()    { AutoSize=false, Tag=l, Text=name };
			var chkConsole = new CheckBox() { AutoSize=true,  Tag=l, Text="Console  ", Checked=l.ConsoleEnabled };
			var chkFile    = new CheckBox() { AutoSize=true,  Tag=l, Text="File",      Checked=l.FileEnabled };
			
			this.Controls.Add( caption );
			this.Controls.Add( chkConsole );
			this.Controls.Add( chkFile );
			
			// Positioning : they need to do after Add, because of AutoSize
			caption.Top    = ItemMargin * Count;
			chkConsole.Top = ItemMargin * Count;
			chkFile.Top    = ItemMargin * Count;
			
			caption.Left    = LeftMargin;
			chkConsole.Left = CaptionLength + LeftMargin;
			chkFile.Left    = chkConsole.Right;
			
			caption.TextAlign = ContentAlignment.MiddleLeft;
			caption.Width = CaptionLength;
			caption.Height = chkFile.Height;
			
			// Events registration
			chkConsole.CheckedChanged += (s,e) => ((LogLevel)chkConsole.Tag).ConsoleEnabled = chkConsole.Checked;
			chkFile.CheckedChanged    += (s,e) => ((LogLevel)chkFile.Tag).FileEnabled = chkFile.Checked;
			
			caption.MouseEnter    += (s,e) => EnterItems(caption,chkConsole,chkFile);
			chkConsole.MouseEnter += (s,e) => EnterItems(caption,chkConsole,chkFile);
			chkFile.MouseEnter    += (s,e) => EnterItems(caption,chkConsole,chkFile);
			caption.MouseLeave    += (s,e) => LeaveItems(caption,chkConsole,chkFile);
			chkConsole.MouseLeave += (s,e) => LeaveItems(caption,chkConsole,chkFile);
			chkFile.MouseLeave    += (s,e) => LeaveItems(caption,chkConsole,chkFile);
			
			// Adjust form size
			int neededHeight = ItemMargin * ( Count + 1 );
			if( this.Height < neededHeight )
			{
				this.Height = neededHeight;
			}
		}
		
		private static void EnterItems( Label l, CheckBox c, CheckBox f )
		{
			l.BackColor = Color.Pink;
			c.BackColor = Color.Pink;
			f.BackColor = Color.Pink;
		}
		
		private static void LeaveItems( Label l, CheckBox c, CheckBox f )
		{
			l.BackColor = SystemColors.Control;
			c.BackColor = SystemColors.Control;
			f.BackColor = SystemColors.Control;
		}
		
		public void BringToForward()
		{
			this.TopMost = true;
			this.TopMost = false;
		}
	}
}
