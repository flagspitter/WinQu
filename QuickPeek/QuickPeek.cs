using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using QuLib;

namespace QuickPeek
{
	public class QuickPeek : IQuModule
	{
		#region モジュールとして必要な情報
		
		// ホストに対して操作するため
		private IQuHost Host = null!;
		
		#endregion
		
		#region インターフェースの実装（実装者が編集する必要がある項目）
		
		// TODO モジュールの基本的な情報を設定してください
		public string Name    => "QuickPeek";
		public string Version => System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "??";
		
		#endregion
		
		#region インターフェースの実装：ホストからの捜査
		
		// アプリの初期化時に実行
		public void Initialize( IQuHost h )
		{
			Host = h;

			WaitTime = Host.LoadModuleSetting( "Wait", "300" ).ToInt() / tmrKeyChecker.Interval;
			
			// TODO
			// 操作のためのキー操作
			Hotkeys = new() {
				// 操作           メソッド
			};
			
			// TODO
			// 配下のコントロールで、
			// マウスポイントしたときにマウスカーソルを変化させたくないものの一覧
			CursorFixer = new Control[] {
			};
			
			// TODO
			// GUIを所持する場合、MainContainer に関連付けます
			MainContainer = null!;
			
			// TODO
			// その他、初期化が必要な場合はここに書きます
			InitializePeeker();
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
		}
		
		// ホストがモジュールを非活性化するときに実行
		public void Deactivate()
		{
			// TODO
			// モジュールを終了するときに必要な処理をここに書きます
		}
		
		public void AppsActivated()
		{
			
		}
		
		public void AppsDeactivated()
		{
			
		}
		
		#endregion
		
		#region インターフェースの実装（定義のみ）
		
		public Dictionary<string,Action> Hotkeys { get; private set; } = null!;
		public Control[] CursorFixer { get; private set; } = null!;
		
		public System.Windows.Forms.UserControl MainContainer { get; private set; } = null!;
		
		#endregion
		
		#region 独自部分の初期化
		
		private System.Windows.Forms.Timer tmrKeyChecker = new();
		public int WaitTime { get; set; } = 300;
		
		public bool Enabled {
			get => tmrKeyChecker.Enabled;
			set => tmrKeyChecker.Enabled = value;
		}
		
		private bool Status = false;
		private int Count = 0;
		
		private Keys? PrimaryKey = null;
		private Keys? SecondaryKey =null;
		
		private void InitializePeeker()
		{
			tmrKeyChecker.Interval = 100;
			tmrKeyChecker.Tick += (s,e) => tmrKeyChecker_Tick();
			
			Enabled = Host.LoadModuleSetting( "Enabled", "true" ).ToBool();
			Host.Log(LogLevel.Notice, $"Enabled = {Enabled}" );
			
			PrimaryKey   = ReadKey( "PrimaryKey", "LWin" );
			SecondaryKey = ReadKey( "SecondaryKey", "Z" );

			return;
			
			Keys? ReadKey( string name, string def )
			{
				var tmp = Host.LoadModuleSetting( name, def );

				Host.Log(LogLevel.Notice, $"Key for {name} = {tmp}");

				return ( tmp == "" ) ? 
					null :
					(Keys?)Enum.Parse( typeof(Keys), tmp );
			}
		}
		
		#endregion
		
		#region 実装部分のイベント
		
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern short GetKeyState(int nVirtKey);
		
		private void tmrKeyChecker_Tick()
		{
			if ( PrimaryKey == null )
			{
				Host.Log(LogLevel.Error, $"Null primary key");
			}
			else if( Status == false )
			{
				Host.Log( LogLevel.Verbose, "Tick / Shown -> Check to enter" );
				CheckToEnter();
			}
			else
			{
				Host.Log( LogLevel.Verbose, "Tick / Hidden -> Check to release" );
				CheckToRelease();
			}
		}
		
		private void CheckToEnter()
		{
			if( PrimaryKey == null )
			{
				Host.Log(LogLevel.Error, $"Null primary key");
				Count = 0;
			}
			else if( ( GetKeyState( (int)PrimaryKey ) < 0 ) &&
			    ( ( SecondaryKey == null ) || ( GetKeyState( (int)SecondaryKey ) < 0 ) ) )
			{
				Host.Log(LogLevel.Verbose, $"To hide {Count}");

				if ( ++Count > WaitTime )
				{
					Host.Log(LogLevel.Verbose, "Hide");

					HideAllWindows();
					Status = true;
					Count = 0;
				}
			}
			else
			{
				Count = 0;
			}
		}
		
		private void CheckToRelease()
		{
			// Console.WriteLine( $"lwin = {GetKeyState( (int)Keys.LWin )}" );
			if( ( PrimaryKey != null ) && ( GetKeyState( (int)PrimaryKey ) >= 0 ) )
			{
				// Console.WriteLine( "Restore" );
				// RestoreMiminize();
				// EndPeekDesktop();
				RestoreHiddenWindows();
				// tmrKeyChecker.Enabled = false;
				Status = false;
				// keybd_event( (byte)Keys.Escape, 0, 0, (UIntPtr)0 );
				// keybd_event( (byte)Keys.Escape, 0, 2, (UIntPtr)0 );
			}
		}
		
		#endregion
		
		#region 他のウィンドウを操作
		
		// 手動でウィンドウを全て検索して、表示中のウィンドウを手動で隠す
		#if true
		
		private delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private extern static bool EnumWindows( EnumWindowsDelegate callback, IntPtr lparam);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private extern static bool IsIconic( IntPtr hWnd );
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private extern static bool IsWindowVisible( IntPtr hWnd );
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private extern static bool IsWindowEnabled( IntPtr hWnd );
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private extern static bool IsWindow( IntPtr hWnd );
		
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int ShowWindow( IntPtr hWnd, int nCmdShow );
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr GetDesktopWindow();
		
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr FindWindow( string lpClassName, string lpWindowName );
		
		[DllImport("user32.dll")]
		private static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
		
		private List<IntPtr> MinimizedList = null!;
		
		private List<IntPtr> EnumWindowHandles()
		{
			var progman = FindWindow( "Progman", null! );
			var desktop = GetDesktopWindow();
			var taskbar = FindWindow( "Shell_traywnd", null! );
			
			// PrintWindowStatus( progman );
			// PrintWindowStatus( desktop );
			// PrintWindowStatus( taskbar );
			
			var ret = new List<IntPtr>();
			EnumWindows(
				(h,lp) => {
					if( IsWindow(h) &&
					    IsWindowEnabled(h) &&
					    IsWindowVisible(h) &&
					    ( IsIconic(h) == false ) &&
					    ( h != progman ) &&
					    ( h != desktop ) &&
					    ( GetClassName(h) != "WorkerW") &&
					    ( h != taskbar ) )
					{
						ret.Add( h );
						PrintWindowStatus( h );
					}
					return true;
				},
				IntPtr.Zero
			);
			
			return ret;
		}
		
		private void PrintWindowStatus( IntPtr h )
		{
			Host.Log( LogLevel.Verbose,
				$"{h} {GetWindowText(h)} / {GetClassName(h)} : Iconic={IsIconic(h)} Visible={IsWindowVisible(h)} Enabled={IsWindowEnabled(h)} Win={IsWindow(h)}" );
		}
		
		private string GetWindowText( IntPtr h )
		{
			var sb = new StringBuilder();
			GetWindowText( h, sb, sb.Capacity );
			return sb.ToString();
		}
		
		private string GetClassName( IntPtr h )
		{
			var sb = new StringBuilder();
			GetClassName( h, sb, sb.Capacity );
			return sb.ToString();
		}
		
		private void HideAllWindows()
		{
			if( ( MinimizedList != null ) && ( MinimizedList.Count > 0 ) )
			{
				RestoreHiddenWindows();
			}
			
			MinimizedList = EnumWindowHandles();
			MinimizedList.ForEach( w => ShowWindow( w, 0 ) );
			MinimizedList.Reverse();
		}
		
		private void RestoreHiddenWindows()
		{
			if( MinimizedList != null )
			{
				MinimizedList.ForEach( w => ShowWindow( w, 5 ) );
				MinimizedList.Clear();
			}
		}
		
		#endif
		
		// Win+M のキーを送り、強引に一時的に最小化
		// 強制TopMostみたいになるウィンドウが出現したりで、都合がよろしくない場合が
		#if false
		[DllImport("user32.dll")]
		private static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
		
		private static void MinimizeAll( int m )
		{
			keybd_event( (byte)Keys.LMenu,       0, 2, (UIntPtr)0 );
			keybd_event( (byte)Keys.RMenu,       0, 2, (UIntPtr)0 );
			keybd_event( (byte)Keys.Space,       0, 2, (UIntPtr)0 );
			
			keybd_event( (byte)Keys.LWin,        0, 0, (UIntPtr)0 );
			keybd_event( (byte)Keys.M,           0, 0, (UIntPtr)0 );
			keybd_event( (byte)Keys.M,           0, 2, (UIntPtr)0 );
			// keybd_event( (byte)Keys.LWin,        0, 2, (UIntPtr)0 );
		}
		
		private static void RestoreMiminize()
		{
			keybd_event( (byte)Keys.LShiftKey, 0, 0, (UIntPtr)0 );
			keybd_event( (byte)Keys.LWin,      0, 0, (UIntPtr)0 );
			keybd_event( (byte)Keys.M,         0, 0, (UIntPtr)0 );
			keybd_event( (byte)Keys.M,         0, 2, (UIntPtr)0 );
			keybd_event( (byte)Keys.LWin,      0, 2, (UIntPtr)0 );
			keybd_event( (byte)Keys.LShiftKey, 0, 2, (UIntPtr)0 );
		}
		#endif
		
		// 実験コード
		// x64 でないと動かない
		// AeroPeekでは、透明化したウィンドウの上からデスクトップを操作できない
		#if false
		public enum PeekTypes : long
		{
			NotUsed = 0,
			Desktop = 1,
			Window = 3
		}
		
		[DllImport("dwmapi.dll", EntryPoint = "#113", SetLastError = true)]
		internal static extern uint DwmpActivateLivePreview( bool peekOn, IntPtr hPeekWindow, IntPtr hTopmostWindow, uint peekType1or3, IntPtr newForWin10 );
		// internal static extern uint DwmpActivateLivePreview( uint sw, IntPtr Handle, IntPtr Caller, uint Method );
		
		public void StartPeekDesktop()
		{
			DwmpActivateLivePreview( true, Parent.Handle, Parent.Handle, 1, IntPtr.Zero );
			// DwmpActivateLivePreview( 1, Parent.Handle, Parent.Handle, 1 );
		}
		
		public void EndPeekDesktop()
		{
			DwmpActivateLivePreview( false, Parent.Handle, Parent.Handle, 1, IntPtr.Zero );
			// DwmpActivateLivePreview( 0, Parent.Handle, Parent.Handle, 1 );
		}
		#endif
		
		#endregion
	}
}
