using System;
using System.Diagnostics;

namespace QuickWatch
{
	public delegate void QuickWatchEventHandler( QuickWatch sender );
	
	public class QuickWatch
	{
		#region 初期化
		
		public QuickWatch()
		{
			IntervalTimer.Interval = 10; // TODO 設定から
			IntervalTimer.Elapsed += (s, e) => Interval?.Invoke(this);
			IntervalTimer.AutoReset = true;
			IntervalTimer.Enabled = false;
		}
		
		#endregion
		
		#region フィールド
		
		private readonly Stopwatch Sw = new();
		private readonly TimeSpan ZeroTime = new(0);
		private readonly System.Timers.Timer IntervalTimer = new();
		
		#endregion
		
		#region プロパティ
		
		public List<TimeSpan> LapList { get; } = [];
		
		public TimeSpan CurrentTime => Sw.Elapsed;
		public TimeSpan Lap   { get; private set; }
		public TimeSpan Split { get; private set; }
		
		public bool IsRunning => Sw.IsRunning;

		public event QuickWatchEventHandler? Interval = null!;
		
		#endregion
		
		#region イベント
		public event QuickWatchEventHandler? RunningChanged = null;
		public event QuickWatchEventHandler? LapModified = null;
		#endregion
		
		#region 操作
		
		public void Start()
		{
			Sw.Start();
			RunningChanged?.Invoke( this );
			IntervalTimer.Enabled = true;
		}
		
		public void Stop()
		{
			Sw.Stop();
			RunningChanged?.Invoke( this );
			IntervalTimer.Enabled = false;
		}
		
		public void Reset()
		{
			if( Sw.IsRunning )
			{
				Sw.Stop();
				Sw.Reset();
				RunningChanged?.Invoke( this );
			}
			else
			{
				Sw.Reset();
			}
		}
		
		public (TimeSpan, TimeSpan) SetLap()
		{
			Split = Sw.Elapsed;
			
			Lap = ( LapList.Count > 0 ) ?
				Split - LapList[ LapList.Count - 1 ] :
				Split;
			
			LapList.Add( Split );
			
			LapModified?.Invoke( this );
			
			return ( Lap, Split );
		}
		
		public void ResetLap()
		{
			LapList.Clear();
			Split = ZeroTime;
			Lap = ZeroTime;
			
			LapModified?.Invoke( this );
		}
		
		#endregion
	}
}
