using System;
using System.Drawing.Text;
using System.Reflection;
using QuLib;

namespace WinQu
{
	public class QuHost(IWindowController w, HotkeyListener hk, Inifile status, Inifile setting) : IHostController
	{
		#region フィールド
		private IWindowController Win = w;
		private List<(IQuModule Module, IQuHost Host)> Modules = new();
		private Inifile StatusFile = status;
		private Inifile SettingFile = setting;
		private HotkeyListener Hotkey = hk;

		#endregion
		
		#region ウィンドウからの操作
		
		public IEnumerable<IQuModule> ModulesEnum
		{
			get
			{
				foreach( var m in Modules )
				{
					yield return m.Module;
				}
			}
		}

		public void Deinitialize()
		{
			Hotkey.UnregisterAll();

			foreach ( var m in Modules )
			{
				m.Module.Deinitialize();
			}
		}

		public void NotifyAppsActivate()
		{
			foreach (var m in Modules)
			{
				m.Module.Deinitialize();
			}
		}

		public void ActionToModules( Action<IQuModule> act )
		{
			foreach( var m in Modules )
			{
				act( m.Module );
			}

		}

		#endregion

		#region 初期化関係

		#endregion

		#region メインウィンドウとのI/F

		public void NotifyWindowSizeChanged(IQuModule m)      => WindowSizeChanged?.Invoke(m);
		public void NotifyWindowLocationChanged(IQuModule m)  => WindowLocationChanged?.Invoke(m);
		public void NotifyShown(IQuModule m)                  => Shown?.Invoke(m);
		public void NotifyHidden(IQuModule m)                 => Hidden?.Invoke(m);
		public void NotifyAddModule(IQuModule m)              => AddModule?.Invoke(m);
		
		#endregion
		
		#region 個々のモジュール制御
		
		public void RegisterModule( IQuModule m )
		{
			var hostWrapper = new QuHostWrapper( this, m );
			Modules.Add( (m, hostWrapper) );

			m.Initialize( hostWrapper );
			NotifyAddModule( m );

			foreach ( var hk in m.Hotkeys )
			{
				Log( LogLevel.Debug, $"Register Hotkey {hk.Key} in {m.Name}" );
				Hotkey.Register( hk.Key, (m,k) =>
				{
					Log(LogLevel.Debug, $"Occurred hot key {m} {k}");
					hk.Value();
				} );
			}
		}

		#endregion
		
		#region IHostControllerの実装
		
		public string Version
		{
			get
			{
				var asm = Assembly.GetExecutingAssembly();
				return asm?.GetName()?.Version?.ToString() ?? "?";
			}
		}
		
		public string LoadModuleSetting( string module, string key, string def )
		{
			string readSetting = SettingFile.Read( module, key, def );
			Log(LogLevel.Notice, $"ReadSetting of {module} : {key} (def={def}) ... {readSetting}");
			return readSetting;
		}
		
		public void   SaveModuleSetting( string module, string key, string val )
		{
			SettingFile.Write( module, key, val );
			Log(LogLevel.Notice, $"SaveSetting of {module} : {key} to {val}");
		}
		
		public Color LoadColorSetting( string module, string key, string def )
		{
			var colStr = LoadModuleSetting( module, key, def );
			try
			{
				return ColorTranslator.FromHtml( colStr );
			}
			catch
			{
				string msg = $"Color specification {colStr} is invalid.";
				MessageBox.Show(
					msg,
					"Karculator : Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				Log( LogLevel.Error, msg );
				throw;
			}
		}
		
		public Font LoadFontSetting( string module, string key, string defaultName = "", int defaultSize = 12 )
		{
			string director = SettingFile.Read( module, key, "" );
			var dirItems = director.Split(',');
			string name;
			int size;
			
			Log( LogLevel.Notice, $"Load font used in {module}.{key} : \"{director}\"" );
			
			if( director == "" )
			{
				name = defaultName;
				size = defaultSize;
				Log( LogLevel.Debug, $"director length = 0 : {name} {size}" );
			}
			else if( dirItems.Length == 1 )
			{
				name = dirItems[0].Trim();
				size = defaultSize;
				Log( LogLevel.Debug, $"director length = 1 : {name} {size}" );
			}
			else
			{
				name = dirItems[0].Trim();
				size = Int32.Parse( dirItems[1].Trim() );
				Log( LogLevel.Debug, $"director length = {dirItems.Length} : {name} {size}" );
			}
			
			var fontList = new InstalledFontCollection()?.Families ?? null;
			Font ret;
			
			if( ( name != "" ) && ( fontList != null ) )
			{
				if( fontList.FirstOrDefault( v => v.Name == name ) != null )
				{
					ret = new Font( name, size );
					Log( LogLevel.Debug, $"Found {ret.Name}" );
				}
				else
				{
					ret = ChangeDefaultFontSize( size );
					Log( LogLevel.Warning, $"Font not found. Use \"{ret.Name}\" instead" );
				}
			}
			else
			{
				ret = ChangeDefaultFontSize( size );
				Log(LogLevel.Notice, $"Use \"{ret.Name}\" as default");
			}
			
			return ret;
			
			///
			Font ChangeDefaultFontSize( int size ) => new Font( Control.DefaultFont.FontFamily, size, Control.DefaultFont.Style );
		}
		
		public List<string> GetAllSettingKeys( string module ) => SettingFile.GetAllKeys( module );
		public List<string> GetAllStatusKeys( string module ) => StatusFile.GetAllKeys( module );
		
		public string LoadModuleStatus( string module, string key, string def )
		{
			string readStatus = StatusFile.Read( module, key, def );
			Log(LogLevel.Notice, $"ReadStatus of {module} : {key} (def={def}) ... {readStatus}");
			return readStatus;
		}
		
		public void   SaveModuleStatus( string module, string key, string val )
		{
			StatusFile.Write( module, key, val );
			Log(LogLevel.Notice, $"SaveStatus of {module} : {key} to {val}");
		}
		
		public void RequestActivate( IQuModule module )
		{
			Log(LogLevel.Notice, $"RequestActivate from {module.Name}");

			if( module.MainContainer != null )
			{
				Win.Show();
			}

			NotifyShown( module );
			Win.Activate();
		}
		
		public void RequestDeactivate( IQuModule module )
		{
			Log(LogLevel.Notice, $"RequestDeactivate from {module.Name}");

			int activeCount = 0;
			foreach( var m in Modules )
			{
				if( m.Module.Visible )
				{
					activeCount++;
				}
			}
			
			NotifyHidden( module );

			Log(LogLevel.Notice, $"There's {activeCount} activated modules");
			if (activeCount == 0)
			{
				Win.Hide();
			}
		}
		
		public void ActivateForm( IQuModule module )
		{
			Win.Show();
			Win.Activate();
			module.MainContainer?.Focus();
		}


		private void Log( LogLevel ll, string message, int depth = 0 ) => Log("Host", ll, message, depth);


		public void Log( string module, LogLevel ll, string message, int depth = 0 )
		{
			Action<string,int> writer = ll switch {
				LogLevel.Fatal   => YacsLibrary.Log.Fatal.WriteLine,
				LogLevel.Error   => YacsLibrary.Log.Error.WriteLine,
				LogLevel.Warning => YacsLibrary.Log.Warning.WriteLine,
				LogLevel.Notice  => YacsLibrary.Log.Notice.WriteLine,
				LogLevel.Debug   => YacsLibrary.Log.Debug.WriteLine,
				LogLevel.Verbose => YacsLibrary.Log.Verbose.WriteLine,
				_ => YacsLibrary.Log.Debug.WriteLine,
			};
			
			writer( $"[{module}] {message}", 2 + depth );
		}
		
		public void Log( string module, string message, int depth = 0 ) => Log( module, LogLevel.Debug, message, depth );
		
		public int Opac  => 0;
		public System.Drawing.Font Font => System.Drawing.SystemFonts.DefaultFont;
		
		public int Width  => Win.Width;
		public int Height => Win.Height;
		public int X      => Win.Left;
		public int Y      => Win.Top;
		public bool Visible => Win.Visible;
		
		public event QuHostEventHandler WindowSizeChanged     = null!;
		public event QuHostEventHandler WindowLocationChanged = null!;
		public event QuHostEventHandler Shown                 = null!;
		public event QuHostEventHandler Hidden                = null!;
		public event QuHostEventHandler AddModule             = null!;
		
		#endregion
	}
}
