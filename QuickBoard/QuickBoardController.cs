using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using QuLib;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuickBoard
{
	public class QuickBoardController : IQuModule
	{
		#region モジュールとして必要な情報

		// ホストに対して操作するため
		private IQuHost Host = null!;

		private readonly QuickBoardForm BoardForm = new();
		
		private readonly Dictionary<string, IClipData> ClipBank = [];
		private readonly Stack<IClipData> ClipStack = [];

		private int OsdTime = 300;
		private int OsdSize = 16;
		private Color OsdSaveBackColor    = Color.LightGreen;
		private Color OsdRestoreBackColor = Color.LightYellow;
		private Color OsdErrorBackColor   = Color.Pink;
		private Color OsdSaveForeColor    = Color.Black;
		private Color OsdRestoreForeColor = Color.Black;
		private Color OsdErrorForeColor   = Color.Black;

		#endregion

		#region 設定できるようにする項目


		#endregion

		#region インターフェースの実装（実装者が編集する必要がある項目）

		// TODO モジュールの基本的な情報を設定してください
		public string Name => "QuickBoard";
		public string Version => System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "??";

		#endregion

		#region インターフェースの実装：ホストからの捜査

		// アプリの初期化時に実行
		public void Initialize(IQuHost h)
		{
			Host = h;

			// TODO
			// 操作のためのキー操作
			Hotkeys = new() {
				// 操作           メソッド
				{ Host.LoadModuleSetting("KeySave",    "ctrl+alt+s"), LaunchToSave },
				{ Host.LoadModuleSetting("KeyRestore", "ctrl+alt+r"), LaunchToRestore },
				{ Host.LoadModuleSetting("KeyPlain",   "ctrl+alt+p"), Plainize },
				{ Host.LoadModuleSetting("KeyPush",    "ctrl+alt+space"), Push },
				{ Host.LoadModuleSetting("KeyPop",     "ctrl+alt+shift+space"), Pop },
			};

			OsdTime = Host.LoadModuleSetting("OsdTime", "300").ToInt();
			OsdSize = Host.LoadModuleSetting("OsdSize", "16").ToInt();
			OsdSaveBackColor    = Host.LoadColorSetting( "OsdSaveBackColor",         "LightGreen" );
			OsdRestoreBackColor = Host.LoadColorSetting( "OsdRestoreBackColorColor", "LightYellow" );
			OsdErrorBackColor   = Host.LoadColorSetting( "OsdErrorBackColor",        "Pink" );
			OsdSaveForeColor    = Host.LoadColorSetting( "OsdSaveForeColor",         "Black" );
			OsdRestoreForeColor = Host.LoadColorSetting( "OsdRestoreForeColorColor", "Black" );
			OsdErrorForeColor   = Host.LoadColorSetting( "OsdErrorForeColor",        "Black" );

		// 設定読み込み

		// TODO
		// 配下のコントロールで、
		// マウスポイントしたときにマウスカーソルを変化させたくないものの一覧
		CursorFixer = new Control[] {
			};

			// TODO
			// GUIを所持する場合、MainContainer に関連付けます
			MainContainer = BoardForm;

			// TODO
			// その他、初期化が必要な場合はここに書きます
			
			BoardForm.Clear();
			BoardForm.RequestOperation += Operate;
			BoardForm.RequestRemove += Remove;
			BoardForm.Hide();
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

		public Dictionary<string, Action> Hotkeys { get; private set; } = null!;
		public Control[] CursorFixer { get; private set; } = null!;

		public System.Windows.Forms.UserControl MainContainer { get; private set; } = null!;

		#endregion

		#region ホットキーから実行するメソッド
		
		private enum Operation
		{
			Save,
			Restore,
		}
		
		private Operation CurrentOperation;

		private void LaunchToSave()
		{
			BoardForm.TitleText = "Save from clipboard";
			BoardForm.TitleColor = Color.LightBlue;
			CurrentOperation = Operation.Save;
			Launch();
		}

		private void LaunchToRestore()
		{
			BoardForm.TitleText = "Restore to clipboard";
			BoardForm.TitleColor = Color.Pink;
			CurrentOperation = Operation.Restore;
			Launch();
		}

		private void Plainize()
		{
			try
			{
				if( Clipboard.ContainsText() )
				{
					var c = new ClipTextData();
					if( c != null )
					{
						c.GetFromClipboard();
						c.RestoreClipboard();

						var df = Control.DefaultFont;
						var f = new Font(df.FontFamily, 14, df.Style);
						ShowOsd( $"Clipboard\nPlainized", OsdSaveForeColor, OsdSaveBackColor );
					}
				}
				else
				{
					var df = Control.DefaultFont;
					var f = new Font(df.FontFamily, 14, df.Style);
					ShowOsd( $"Not Text Data", OsdErrorForeColor, OsdErrorBackColor );
				}
			}
			catch( Exception e )
			{
				Host.LogE( e.Message );
				ShowOsd( "Clipboard error", OsdErrorForeColor, OsdErrorBackColor );
			}
		}

		private void Launch()
		{
			if (BoardForm.Visible)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}

		private void Hide()
		{
			Host.LogN("Hide");
			BoardForm.Invoke(() =>
			{
				BoardForm.Hide();
				Host.RequestDeactivate();
			});
		}
		
		private void Show()
		{
			Host.LogN("Show");

			BoardForm.Clear();
			foreach( var n in ClipBank )
			{
				BoardForm.Add( n.Key, n.Value.Type, n.Value.Summary );
				
				#if false
				foreach (string f in formats)
				{
					// 各データ形式の内容を表示
					var content = n.Value.GetData(f);
					Host.LogN($"Format: {f}, Content: {content ?? "(NULL)"}");
				}
				#endif
			}

			BoardForm.Show();
			BoardForm.AdjustWidth(Host.Width);
			Host.RequestActivate();

			BoardForm.Invoke(() =>
			{
				BoardForm.Focus();
			});
		}

		#endregion

		#region その他

		private void Operate( string key )
		{
			if( CurrentOperation == Operation.Restore )
			{
				Restore( key );
				Blink( key );
			}
			else if( CurrentOperation == Operation.Save )
			{
				Save( key );
				Blink( key );
			}
			else
			{
				Host.LogF( "Invalid operation" );
			}
		}
		
		private void Blink( string key )
		{
			int repeat = 3;
			int interval = 50;

			BoardForm.Unselect();
			Task.Run( () => {
				for( int i = 0; i < repeat; i++ )
				{
					BoardForm.Invoke(() => BoardForm.Select(key));
					Thread.Sleep(interval);
					BoardForm.Invoke(() => BoardForm.Unselect());
					Thread.Sleep(interval);
				}
				Hide();
			} );
		}

		private void Save( string key )
		{
			try
			{
				var data = GetClipboard();
				
				if( data != null )
				{
					Host.LogN( $"Saved from clipboard to [{key}] : {data.Type} {data.Summary}" );

					ClipBank[ key ] = data;
					BoardForm.Add( key, data.Type, data.Summary );

					var df = Control.DefaultFont;
					var f = new Font( df.FontFamily, 16, df.Style );
					ShowOsd( $"Saved to [{key}]", OsdSaveForeColor, OsdSaveBackColor );
				}
				else
				{
					Host.LogW( "No Clipboard data" );
					ShowOsd( $"Clipboard is empty or\nnot supported format", OsdErrorForeColor, OsdErrorBackColor );
				}
			}
			catch( Exception e )
			{
				Host.LogE( e.Message );
				ShowOsd( "Clipboard error", OsdErrorForeColor, OsdErrorBackColor );
			}
		}
		
		private void Restore( string key )
		{
			try
			{
				if( ClipBank.ContainsKey( key ) )
				{
					var data = ClipBank[ key ];
					if( data == null )
					{
						Host.LogF( $"[{key}] is broken" );
					}
					else
					{
						Host.LogN( $"Restore Clipboard key [{key}]" );
						data.RestoreClipboard();

						var df = Control.DefaultFont;
						var f = new Font(df.FontFamily, 16, df.Style);
						ShowOsd( $"Restored from [{key}]", OsdRestoreForeColor, OsdRestoreBackColor );
					}
				}
				else
				{
					Host.LogE( $"not found [{key}]" );
				}
			}
			catch( Exception e )
			{
				Host.LogE( e.Message );
				ShowOsd( "Clipboard error", OsdErrorForeColor, OsdErrorBackColor );
			}
		}

		private void Push()
		{
			try
			{
				var data = GetClipboard();

				if (data != null)
				{
					Host.LogN($"Push clipboard : {data.Type} {data.Summary}");
					ClipStack.Push(data);
					ShowOsd( $"Push Clipboard ({ClipStack.Count} items)", OsdSaveForeColor, OsdSaveBackColor );
				}
			}
			catch( Exception e )
			{
				Host.LogE( e.Message );
				ShowOsd( "Clipboard error", OsdErrorForeColor, OsdErrorBackColor );
			}
		}

		private void Pop()
		{
			try
			{
				if( ClipStack.Count > 0 )
				{
					var data = ClipStack.Pop();
					data.RestoreClipboard();
					ShowOsd( $"Pop Clipboard ({ClipStack.Count} items)", OsdRestoreForeColor, OsdRestoreBackColor );
				}
				else
				{
					var df = Control.DefaultFont;
					var f = new Font(df.FontFamily, 16, df.Style);
					ShowOsd( $"Clipboard statck is empty", OsdErrorForeColor, OsdErrorBackColor );
				}
			}
			catch( Exception e )
			{
				Host.LogE( e.Message );
				ShowOsd( "Clipboard error", OsdErrorForeColor, OsdErrorBackColor );
			}
		}


		private void Remove( string key )
		{
			Host.LogN($"Remove clip [{key}]");
			ClipBank.Remove( key );
			BoardForm.Remove( key );
			Host.SaveModuleStatus(key, "");
		}
		
		private void ShowOsd( string msg, Color fc, Color bc )
		{
			var df = Control.DefaultFont;
			var f = new Font(df.FontFamily, OsdSize, df.Style);
			Host.ShowOSD( msg, OsdTime, f, fc, bc, () => f.Dispose() );
		}

		#endregion

		#region クリップボード関係
		
		private IClipData? GetClipboard()
		{
			IClipData? ret = null;

			if( Clipboard.ContainsText() )
			{
				ret = new ClipTextData();
			}
			else if( Clipboard.ContainsImage() )
			{
				ret = new ClipImageData();
			}
			else if( Clipboard.ContainsFileDropList() )
			{
				ret = new ClipFileData();
			}
			else
			{
				Host.LogW( "not supported clipboard type" );
			}

			if( ret != null )
			{
				ret.GetFromClipboard();
			}

			return ret;
		}
		
		private void ApplyToClipboard( IDataObject obj )
		{
			Clipboard.SetDataObject(obj);
		}

		#endregion
	}
}
