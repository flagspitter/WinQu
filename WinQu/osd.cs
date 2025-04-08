using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace WinQu
{
	public class OSD : Form
	{
		public string DisplayText { get; set; }
		public int DisplayTime { get; set; } = 500;
		public Color OsdForeColor { get; set; } = Color.White;
		public System.Drawing.Font OsdFont { get; set; }

		public OSD( string m = "" )
		{
			DisplayText = m;
			OsdFont = Control.DefaultFont;

			this.FormBorderStyle = FormBorderStyle.None;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.BackColor = Color.Black;
			this.Opacity = 0.8; // 半透明
			this.Size = new Size( 200, 50 );
			this.TopMost = true; // 最前面に表示
			this.ShowInTaskbar = false; // タスクバーに表示しない

			this.Paint += OSD_Paint;
		}

		public void ShowOSD()
		{
			this.Show();

			// タスクを遅延実行して一定時間後にフォームを閉じる
			_ = CloseAfterDelayAsync();
		}

		private async Task CloseAfterDelayAsync()
		{
			await Task.Delay(DisplayTime);

			this.Invoke( () =>
			{
				OsdFont.Dispose();
				this.Close();
			} );
		}

		private void OSD_Paint(object? sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			using( var brush = new SolidBrush(OsdForeColor) )
			{
				SizeF textSize = g.MeasureString(DisplayText, OsdFont);
				this.Size = new Size( (int)textSize.Width + 8, (int)textSize.Height + 8 );
				float x = (this.ClientSize.Width - textSize.Width) / 2;
				float y = (this.ClientSize.Height - textSize.Height) / 2;
				g.DrawString(DisplayText, OsdFont, brush, x, y);
			}
		}

		// フォームがフォーカスを奪わないようにする
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
				return cp;
			}
		}
	}
}
