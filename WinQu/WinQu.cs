using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Diagnostics;
using System.Reflection;

using QuLib;
using YacsLibrary;
using System.Xml.Linq;

namespace WinQu
{
	public partial class WinQu : Form, IWindowController
	{
		////////////////////////////////////////////////////////////////
		#region メンバ変数等
		////////////////////////////////////////////////////////////////
		
		// 本体
		private readonly QuHost Host;
		
		// ホットキー制御用
		private readonly HotkeyListener Hotkey = new();
		
		// 設定用
		private readonly Inifile SettingIni = new( Const.AplicationPath + "\\Settings.ini" );
		private readonly Inifile StatusIni = new( Const.AplicationPath + "\\Status.ini" );
		private const string CatSystem = "System";
		private const string CatLogToConsole = "LogToConsole";
		private const string CatLogToFile = "LogToFile";
		private const string CatView = "View";
		
		// ウィンドウサイズ制御用
		private FormForceResizer Resizer = null!;
		private int MinWinWidth = 220;
		private int MinWinHeight = 95;
		private double Opac_a;
		private double Opac_d;
		
		#endregion
		
		////////////////////////////////////////////////////////////////
		#region 初期化関係
		////////////////////////////////////////////////////////////////
		
		public WinQu()
		{
			#if false
				Log.D("Boot");
			#else
				InitializeConsoleLog();
				InitializeFileLog();
			#endif

			// ウィンドウの初期化
			InitializeComponent();

			// ウィンドウの初期設定
			ShowInTaskbar = false;
			TopMost = true;

			// タスクトレイの準備
			notifyIcon1.Visible = true;
			notifyIcon1.Text = "WinQu";
			
			// 本体の準備
			Host = new QuHost(this, Hotkey, StatusIni, SettingIni);
			Host.WindowSizeChanged     += ReceivedWindowsSizeChanged;
			Host.WindowLocationChanged += ReceivedWindowLocationChanged;
			Host.Shown                 += ReceivedShown;
			Host.Hidden                += ReceivedHidden;
			Host.AddModule             += ReceivedAddModule;
			this.Height = 0;
		}
		
		private void InitializeConsoleLog()
		{
			string cat = CatLogToConsole;
			const string def = "false";
			
			if( SettingIni.Read( cat, "Enabled", def ).ToBool() )
			{
				Log.Verbose.ConsoleEnabled = SettingIni.Read( cat, "Verbose", def ).ToBool();
				Log.Debug.ConsoleEnabled = SettingIni.Read( cat, "Debug", def ).ToBool();
				SosLogger.BeginConsole();
			}
		}
		
		private void InitializeFileLog()
		{
			string cat = CatLogToFile;
			const string def = "false";
			string place = SettingIni.Read( cat, "Directory", def ).Trim();
			
			if( SettingIni.Read( cat, "Enabled", def ).ToBool() &&
			    String.IsNullOrEmpty( place ).Invert() )
			{
				Log.Verbose.FileEnabled = SettingIni.Read( cat, "Verbose", def ).ToBool();
				Log.Debug.FileEnabled = SettingIni.Read( cat, "Debug", def ).ToBool();
				SosLogger.BeginFile( place );
			}
		}

		private void MainWindow_Load(object sender, EventArgs e)
		{
			LoadModules();
			LoadSettings();
			SetResizer();

			SetupTaskTrayMenu();
		}

		private void SetResizer()
		{
			Resizer = new FormForceResizer(this, MinWinWidth, MinWinHeight);
			Resizer.EnableVertical = false;

			Host.ActionToModules(m =>
			{
				if (m.CursorFixer != null)
				{
					Resizer.Excluded.AddRange(m.CursorFixer.ToArray());
				}
			});
		}

		private void MainWindow_Shown(object sender, EventArgs e)
		{
			Log.N("Showen");
			ResizeVertical();
		}
		
		private void LoadModules()
		{
			// Load all dlls in current directory
			var curFullPath = System.Reflection.Assembly.GetEntryAssembly()?.Location ?? "";
			var curPath = Path.GetDirectoryName( curFullPath ) ?? "";
			
			var dllFiles = System.IO.Directory.GetFiles( curPath, "*.dll", System.IO.SearchOption.TopDirectoryOnly );
			
			foreach( var dll in dllFiles )
			{
				Log.D( $"Found {dll}" );
				LoadModule( dll );
			}
		}
		
		private void LoadModule( string file )
		{
			var asm = Assembly.LoadFrom( file );

			IQuModule? instance = null;

			var typeList = asm.GetTypes();

			foreach( var type in typeList )
			{
				if( ( type.IsInterface == false ) && typeof( IQuModule ).IsAssignableFrom( type ) )
				{
					instance = Activator.CreateInstance( type ) as IQuModule;
					break;
				}
			}

			if( instance == null )
			{
				Log.N( $"{file} does not includ IQuModule" );
			}
			else
			{
				Log.N( $"Loaded {instance.Name} from {file}" );
				Host.RegisterModule( instance );
			}
		}

		private void LoadSettings()
		{
			int x  = Int32.Parse(  StatusIni.Read( CatView, "Left", "0" ) );
			int y  = Int32.Parse(  StatusIni.Read( CatView, "Top", "0" ) );
			int w  = Int32.Parse(  StatusIni.Read( CatView, "Width", "225" ) );
			Opac_a = Double.Parse( SettingIni.Read( CatSystem, "Opac_Active", "0.9" ) );
			Opac_d = Double.Parse( SettingIni.Read( CatSystem, "Opac_Deactive", "0.8" ) );
			
			this.Left = x;
			this.Top = y;
			this.Width = w;
			this.Opacity = ( Form.ActiveForm == this ) ? Opac_a : Opac_d;
		}

		private void SetupTaskTrayMenu()
		{
			// アセンブリのバージョン情報を取得
			var assembly = Assembly.GetExecutingAssembly();
			var version = assembly?.GetName()?.Version?.ToString() ?? "??";           
			
			// タスクトレイでの右クリックメニュー
			ContextMenuStrip menu = new ContextMenuStrip();

			menu.Items.Add(new ToolStripMenuItem( $"WinQu Ver.{version}", null, (_,_) => ShowVersion() ));
			menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

			Host.ActionToModules(m =>
			{
				string name = m.Name;
				#if false
				if (m.AcceleratorKey != '\0')
				{
					name += $"(&{m.AcceleratorKey})";
				}
				#endif
				menu.Items.Add(new ToolStripMenuItem(name, null, (_, _) => m.Activate()));
			});

			menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
			menu.Items.Add(new ToolStripMenuItem("ResetPosition(&P)", null, (_s, _e) =>
			{
				Log.D( $"ResetPosition : from ({Left},{Top}) to (0,0)");
				Left = Top = 0;
			}));
			menu.Items.Add(new ToolStripMenuItem("Exit(&X)", null, (_s, _e) =>
			{
				Log.D( $"Exit from tasktray");
				Exit();
			}));
			notifyIcon1.ContextMenuStrip = menu;
		}

		private void ShowVersion()
		{
			using( var f = new About() )
			{
				string verList = "";
				foreach( var m in Host.ModulesEnum )
				{
					verList += $"  {m.Name} {m.Version}\n";
				}

				f.ModulesVersion = verList;
				f.ShowDialog();
			}
		}

		#endregion

		////////////////////////////////////////////////////////////////
		#region 終了処理
		////////////////////////////////////////////////////////////////

		private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			Log.N( "Closed" );
			Hotkey.UnregisterAll();
			Host.ActionToModules( m => m.Deinitialize() );
		}
		
		private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			Log.N( "Closing" );
			
			StatusIni.Write( CatView, "Top",  this.Top );
			StatusIni.Write( CatView, "Left", this.Left );
			StatusIni.Write( CatView, "Width",  this.Width );
			StatusIni.Write( CatView, "Height", this.Height );
			
			if( this.Visible && ( e.CloseReason == CloseReason.UserClosing ) )
			{
				Log.N( "Hide all module instead." );
				e.Cancel = true;
				this.Hide();
			}
		}
		
		#endregion

		////////////////////////////////////////////////////////////////
		#region ウィンドウ制御関係
		////////////////////////////////////////////////////////////////
		
		private void MainWindow_Activated(object sender, EventArgs e)
		{
			Host.ActionToModules( m => m.AppsActivated() );
			this.Opacity = Opac_a;
		}
		
		private void MainWindow_Deactivate(object sender, EventArgs e)
		{
			Host.ActionToModules(m => m.AppsDeactivated());
			this.Opacity = Opac_d;
		}
		
		public new void Show()
		{
			if( this.Visible == false )
			{
				base.Show();
			}
		}
		
		// 閉じる操作を無効化
		private const int WS_EX_NOACTIVATE = 0x8000000;
		protected override CreateParams CreateParams
		{
			get
			{
				var cp = base.CreateParams;
				if( base.DesignMode == false )
				{
					cp.ExStyle |= WS_EX_NOACTIVATE;
				}
				
				return cp;
			}
		}
		
		#endregion
		
		////////////////////////////////////////////////////////////////
		#region ホストからのイベント
		////////////////////////////////////////////////////////////////
		
		private void ReceivedWindowsSizeChanged( IQuModule m )
		{
			Log.N( $"NotifyWindowSizeChanged <- {m.Name}" );
		}
		
		private void ReceivedWindowLocationChanged( IQuModule m )
		{
			Log.N( $"NotifyWindowLocationChanged <- {m.Name}" );
		}
		
		private void ReceivedShown( IQuModule m )
		{
			Log.N( $"NotifyShown <- {m.Name}" );
			
			m.MainContainer?.BringToFront();
			
			ResizeMainWindow();
			
			if( m.MainContainer != null )
			{
				m.MainContainer.Dock = DockStyle.Top;
			}
		}
		
		private void ReceivedHidden( IQuModule m )
		{
			Log.N( $"NotifyHidden <- {m.Name}" );
			ResizeMainWindow();
			if( m.MainContainer != null )
			{
				m.MainContainer.Dock = DockStyle.None;
			}
		}
		
		private void ResizeMainWindow()
		{
			int heightSum = 0;
			Host.ActionToModules( m =>
			{
				if( m.Visible )
				{
					Log.D( $" {m.Name} height = {m.MainContainer?.Height ?? 0}" );
					heightSum += m.Height;
				}
			} );

			this.Height = heightSum;
		}
		
		private void ReceivedAddModule( IQuModule m )
		{
			Log.N($"NotifyAddModule <- {m.Name}");
			if( m.MainContainer != null )
			{
				m.MainContainer.Dock = DockStyle.None;
				this.Controls.Add( m.MainContainer );
			}
		}
		
		#endregion
		
		////////////////////////////////////////////////////////////////
		#region 総合制御
		////////////////////////////////////////////////////////////////
		
		private void Wake( IQuModule c )
		{
			c.Visible = true;
			ResizeVertical();
			
			// Console.WriteLine( $"Wake {c.Name}" );
			
			// 下に見切れないようにする
			var scr = System.Windows.Forms.Screen.FromControl( this );
			if( ( this.Bottom + c.Height ) >= scr.Bounds.Height )
			{
				// Console.WriteLine( $"Add to top: Win( {Top}, {Left} ) - ( {Right}, {Bottom} ), c.Height={c.Height}" );
				c.SendToBack();
				this.Top -= c.Height;
			}
			else
			{
				// Console.WriteLine( $"Add to Bottom: Win( {Top}, {Left} ) - ( {Right}, {Bottom} ), c.Height={c.Height}" );
				c.BringToFront();
			}
			// Console.WriteLine( $"            -> Win( {Top}, {Left} ) - ( {Right}, {Bottom} ), c.Height={c.Height}" );
		}
		
		private void Rest( IQuModule c )
		{
			c.Visible = false;
			ResizeVertical();
		}
		
		private void ResizeVertical()
		{
			int sumHeight = 0;
			
			Host.ActionToModules( m => sumHeight += (m.Visible ? m.Height : 0) );
			
			if( sumHeight == 0 )
			{
				this.Hide();
			}
			else
			{
				this.Show();
				this.Height = sumHeight;
			}
		}
		
		private void Exit()
		{
			this.Hide();
			this.Close();
		}

		#endregion
	}
}
