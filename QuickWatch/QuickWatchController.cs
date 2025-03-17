using System;
using System.Diagnostics;
using QuLib;

namespace QuickWatch
{
	public class QuickWatchController : IQuModule
	{
		#region モジュールとして必要な情報
		
		// ホストに対して操作するため
		private IQuHost Host = null!;
		
		// 処理本体
		private QuickWatch SwFunction = new();
		
		// UI
		private QuickWatchForm SwForm;
		private LapView LapForm = new();
		
		#endregion
		
		#region 初期化関係
		
		public QuickWatchController()
		{
			SwForm = new( this );
			SwFunction.Interval += (s) => Update(s.CurrentTime, s.LapList);
		}
		
		#endregion
		
		#region インターフェースの実装（実装者が編集する必要がある項目）
		
		public string Name    => "QuickWatch";
		public string Version => System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "??";
		
		#endregion
		
		#region インターフェースの実装：ホストからの操作
		
		// アプリの初期化時に実行
		public void Initialize( IQuHost h )
		{
			Host = h;

			// 設定読み込み
			static string EscapeTimeFormat(string src) => src.Replace(":", @"\:").Replace(".", @"\.");
			SwForm.Format = EscapeTimeFormat( Host.LoadModuleSetting("Format", @"hh:mm:ss.ff") );

			// TODO
			// 操作のためのキー操作
			Hotkeys = new() {
				// 操作           メソッド
				{ Host.LoadModuleSetting("KeyToggle",    "ctrl+alt+q"), Toggle },
				{ Host.LoadModuleSetting("KeyLap",       "ctrl+alt+w"), SetLap },
				{ Host.LoadModuleSetting("KeyShowLap",   "ctrl+alt+l"), ShowLaps },
				{ Host.LoadModuleSetting("KeyStartStop", "ctrl+alt+a"), StartStop },
				{ Host.LoadModuleSetting("KeyReset",     "ctrl+alt+e"), Reset },
			};
			
			// TODO
			// 配下のコントロールで、
			// マウスポイントしたときにマウスカーソルを変化させたくないものの一覧
			CursorFixer = SwForm.CursorFixer;
			
			// TODO
			// GUIを所持する場合、MainContainer に関連付けます
			MainContainer = SwForm;
			
			// TODO
			// その他、初期化が必要な場合はここに書きます
			SwForm.RequestStartStop += (s,e) => StartStop();
			SwForm.RequestLap       += (s,e) => SetLap();
			SwForm.RequestReset     += (s,e) => Reset();
			SwForm.Hide();
		}
		
		// アプリの終了時に実行
		public void Deinitialize()
		{
			// TODO
			// 終了処理が必要な場合はここに書きます
		}
		
		// ホストがモジュールを活性化するときに実行
		public void Activate()
		{
			// TODO
			// モジュールを起動するときに必要な処理をここに書きます
			Toggle();
		}
		
		// ホストがモジュールを非活性化するときに実行
		public void Deactivate()
		{
			// TODO
			// モジュールを終了するときに必要な処理をここに書きます
			SwForm.Hide();
			SwFunction.Stop();
		}
		
		public void AppsActivated()
		{
			; // nothing to do
		}
		
		public void AppsDeactivated()
		{
			; // nothing to do
		}
		
		#endregion
		
		#region インターフェースの実装（定義のみ）
		
		public Dictionary<string,Action> Hotkeys { get; private set; } = null!;
		public Control[] CursorFixer { get; private set; } = null!;
		
		public System.Windows.Forms.UserControl MainContainer { get; private set; } = null!;
		
		#endregion
		
		#region 表示からの操作
		
		public List<TimeSpan> LapList => SwFunction.LapList;
		
		public TimeSpan CurrentTime => SwFunction.CurrentTime;
		public TimeSpan Lap   => SwFunction.Lap;
		public TimeSpan Split => SwFunction.Split;
		
		public bool IsRunning => SwFunction.IsRunning;
		
		public event QuickWatchEventHandler RunningChanged   { add => SwFunction.RunningChanged+=value;    remove => SwFunction.RunningChanged-=( value );   }
		public event QuickWatchEventHandler LapModified      { add => SwFunction.LapModified+=value;       remove => SwFunction.LapModified-=( value );      }
		
		#endregion
		
		#region 操作
		
		private void Toggle()
		{
			Host.Log(LogLevel.Notice, "Toggle");

			if( SwFunction.IsRunning.IsFalse() )
			{
				if( SwForm.Visible )
				{
					Host.Log(LogLevel.Notice, "Stop -> Hide");
					SwFunction.Stop();
					SwForm.Invoke( () => SwForm.Hide() );
					Host.RequestDeactivate();
				}
				else
				{
					Host.Log(LogLevel.Notice, "Stop -> Run");
					SwFunction.Reset();
					SwFunction.ResetLap();
					LapForm.Clear();
					SwFunction.Start();
					SwForm.Invoke( () =>
					{
						SwForm.Start();
						SwForm.Show();
					} );
					Host.RequestActivate();
				}
			}
			else
			{
				Host.Log(LogLevel.Notice, "Run -> Stop");
				SwFunction.Stop();
				SwForm.Invoke( () => SwForm.Stop() );
			}
		}
		private void StartStop()
		{
			Host.Log(LogLevel.Notice, "StartStop");

			if( SwForm.Visible.IsFalse() )
			{
				Host.Log(LogLevel.Notice, "Hide -> Show -> Stop -> Run");
				SwFunction.Reset();
				SwFunction.Start();
				SwForm.Invoke( () =>
				{
					SwForm.Start();
					SwForm.Show();
				} );
				Host.RequestActivate();
			}
			else if( SwFunction.IsRunning == false )
			{
				Host.Log(LogLevel.Notice, "Stop -> Run");
				SwFunction.Start();
				SwForm.Invoke( () =>
				{
					SwForm.Start();
					SwForm.Show();
				} );
			}
			else
			{
				Host.Log(LogLevel.Notice, "Run -> Stop");
				SwFunction.Stop();
				SwForm.Invoke(() => SwForm.Stop());
			}
		}

		private void SetLap()
		{
			if (SwFunction.IsRunning == false)
			{
				Host.Log(LogLevel.Warning, "Lap - failed because it is stopped");
			}
			else
			{
				( TimeSpan lap, TimeSpan spl ) = SwFunction.SetLap();
				LapForm.AddLap( lap, spl );
				Host.Log(LogLevel.Notice, "Lap");
			}
		}
		
		private void ShowLaps()
		{
			if( LapForm.Visible )
			{
				Host.Log( LogLevel.Notice, "Hide Lap" );
				LapForm.Hide();
			}
			else
			{
				Host.Log( LogLevel.Notice, "Show Lap" );
				LapForm.Show();
				LapForm.TopMost = true;
				LapForm.TopMost = false;
			}
		}
		
		private void Reset()
		{
			SwFunction.Stop();
			SwFunction.Reset();
			SwFunction.ResetLap();
			LapForm.Clear();
			SwForm.Invoke( () =>
			{
				SwForm.Stop();
				SwForm.UpdateWatch(SwFunction.CurrentTime, SwFunction.LapList);
			} );
			Host.Log( LogLevel.Notice, "Reset" );
		}

		#endregion

		#region フォームへの操作

		private void Update( TimeSpan t, List<TimeSpan> l )
		{
			SwForm.Invoke( () => SwForm.UpdateWatch(t,l) );
		}
		#endregion
	}
}
