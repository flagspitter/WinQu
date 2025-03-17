using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace QuickWatch
{
	public partial class QuickWatchForm : UserControl
	{
		////////////////////////////////////////////////////////////////
		#region 初期化関係
		////////////////////////////////////////////////////////////////

		public QuickWatchForm( QuickWatchController c )
		{
			InitializeComponent();
			Control = c;
			Control.RunningChanged += (s) => tmrInterval.Enabled = s.IsRunning;
			DefaultLapString = btnLapView.Text;
		}

		private void SwForm_Load(object sender, EventArgs e)
		{
			// OffsetTime = TimeSpan.Parse( "-00:00:03.00" );
		}

		#endregion

		////////////////////////////////////////////////////////////////
		#region 管理用フィールド
		////////////////////////////////////////////////////////////////

		private string DefaultLapString;

		#endregion

		////////////////////////////////////////////////////////////////
		#region プロパティ
		////////////////////////////////////////////////////////////////

		public string IniCategory { get; } = "StopWatch";

		public Color SwForeColor { get; set; }
		public Color SwBackColor { get; set; }
		public Font TextFont { get; set; } = System.Drawing.SystemFonts.DefaultFont;

		public string Format { get; set; } = @"hh\:mm\:ss\.ff";
		
		public QuickWatchController Control { get; set; }

		public Control[] CursorFixer { get; } = [];

		#endregion

		////////////////////////////////////////////////////////////////
		#region イベント
		////////////////////////////////////////////////////////////////
		
		public event EventHandler RequestStartStop = null!;
		public event EventHandler RequestLap  = null!;
		public event EventHandler RequestReset = null!;
		
		#endregion
		
		////////////////////////////////////////////////////////////////
		#region 操作
		////////////////////////////////////////////////////////////////

		private void btnStartStop_Click(object sender, EventArgs e)
		{
			RequestStartStop?.Invoke(this,null!);
		}

		private void btnLap_Click(object sender, EventArgs e)
		{
			RequestLap?.Invoke(this,null!);
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			RequestReset?.Invoke(this,null!);
		}

		private void btnLapView_Click(object sender, EventArgs e)
		{
		}

		private void lbCounter_DoubleClick(object sender, EventArgs e)
		{
		}

		#endregion

		////////////////////////////////////////////////////////////////
		#region ModelからUIへの指示
		////////////////////////////////////////////////////////////////
		
		public void UpdateWatch( TimeSpan elapsed, List<TimeSpan> laps )
		{
			lbCounter.Text = elapsed.ToString( Format );

			if ( (laps == null) || (laps.Count == 0) )
			{
				btnLapView.Text = DefaultLapString;
			}
			else
			{
				string spl = laps[laps.Count - 1].ToString( Format );
				string lap = ( laps.Count == 1 ) ?
					laps[0].ToString(Format) :
					(laps[laps.Count - 1] - laps[laps.Count - 2] ).ToString(Format);

				btnLapView.Text = $"[{laps.Count}] LAP {lap}  SPL {spl}";
			}
		}

		public void Start()
		{
			btnStartStop.Text = "STOP";
		}
		
		public void Stop()
		{
			btnStartStop.Text = "START";
		}

		#endregion
	}
}
