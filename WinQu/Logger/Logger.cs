using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

/* ------------------------------------------------------------
	The simple logger
	"Static Ojisan's Simple Logger" means "Static-Man's Simple Logger"
	
	To log via SosLogger, you just call like this:
		Log.WriteLine( "foo" );
	
	You don't need to generate an instance of this class.
	
	If you need Log-Levels, you can modify LogLevels.cs
	to add new Log-Level,
	to remove a Log-Level,
	to change to enable/disable output to file or console.
	
	To log via a Log-Level, you have to call like this:
		Log.Warning.WriteLine( "foo" );
	
 ----------------------------------------------------------- */
namespace YacsLibrary
{
	public static class Settings
	{
		public const bool ConsoleEnabled = false;
		public const bool FileEnabled    = false;
		public const string Folder = "";
		public const int FileCoulmns = 36;
	}

	/* ------------------------------------------------------------
		All logs will be written via this class.
		This library is "Static-Ojisan's Simple Logger",
		but, LogLevel class needs to generate instances.
		(Users don't need to aware of this class to use it)
		
		e.g.)
			Log.Warning.WriteLine("foo");
			    ~
			    this is a LogLevel class
	------------------------------------------------------------ */
	public class LogLevel
	{
		////////////////////////////////////////////////////////////////////////
		#region Properties for configuration
		public bool ConsoleEnabled { get; set; } // Output to Standard output
		public bool FileEnabled    { get; set; } // Output to File
		public bool Trace          { get; set; } // Output file, line
		
		public string Caption { get; set; } // This is written to top of log.

		public ConsoleColor ForeColor { get; set; } // Char color for this level
		public ConsoleColor BackColor { get; set; } // Back color for this level
		
		// Log writer
		public Action<ConsoleColor,ConsoleColor,string,string,string,string,bool,bool> Writer { get; set; }
		#endregion
		
		////////////////////////////////////////////////////////////////////////
		#region Const Values
		private const string Separator = "----\n"; // used by .Separate
		#endregion
		
		////////////////////////////////////////////////////////////////////////
		#region Controller
		
		/* ------------------------------------------------------------
			Log writing
		------------------------------------------------------------ */
		public void WriteLine( string s ) => Write( s + "\n", 1 );
		public void WriteLine( string s, int stackCnt ) => Write( s + "\n", stackCnt + 1 );
		public void Write( string s, int stackCnt = 0 )
		{
			if( ConsoleEnabled || FileEnabled )
			{
				if( Trace )
				{
					var st = new StackTrace( 1, true );
					string source = Path.GetFileName(st?.GetFrame(stackCnt)?.GetFileName() ?? "" ) ?? "";
					int line = st?.GetFrame(stackCnt)?.GetFileLineNumber() ?? -1;
					
					Writer( ForeColor, BackColor, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), $"{source}({line})", Caption, s, ConsoleEnabled, FileEnabled );
				}
				else
				{
					Writer( ForeColor, BackColor, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "", Caption, s, ConsoleEnabled, FileEnabled );
				}
			}
		}
		
		/* ------------------------------------------------------------
			Separate lines for easier reading logs
		------------------------------------------------------------ */
		public void Separate()
		{
			Writer( ConsoleColor.White, ConsoleColor.Black, "", "", "", Separator, ConsoleEnabled, FileEnabled );
		}
		#endregion
		
		////////////////////////////////////////////////////////////////////////
		#region Initialization
		
		/* ------------------------------------------------------------
			Constuctor
		------------------------------------------------------------ */
		public LogLevel(
			Action<ConsoleColor,ConsoleColor,string,string,string,string,bool,bool> w,
			bool fe, bool ce, ConsoleColor fc, ConsoleColor bc, bool t, string c = "" )
		{
			FileEnabled = fe;
			ConsoleEnabled = ce;
			ForeColor = fc;
			BackColor = bc;
			Trace = t;

			Writer = w;
			
			Caption = c;
		}
		#endregion
	}
	
	/* ------------------------------------------------------------
		Users control almost everything through this class.
		
		I consider it pointless to create an instance for logging.
		In fact, there is almost no need to output logs to multiple files.
		If so, it would be better to be able to use it quickly and without unnecessary processing.
	------------------------------------------------------------ */
	public static class SosLogger
	{
		public static string Version => "1.0.0";
		
		////////////////////////////////////////////////////////////////////////
		#region Properties
		// BASE Settings:
		// If these are disabled, the output is disabled even if it is enabled in the LogLevel class.
		public static bool ConsoleEnabled { get; set; } = Settings.ConsoleEnabled; // Standard output
		public static bool FileEnabled    { get; set; } = Settings.FileEnabled;    // File output
		public static string Folder       { get; set; } = Settings.Folder;         // Output file
		
		public static int FileNameColumns { get; set; } = Settings.FileCoulmns;
		#endregion
		
		#region Internal use
		private static string BaseName => $"\\{DateTime.Now:yyyy-MM-dd}.log";
		private static bool Continuous = false;
		private static object Sem = new object();
		private static int CaptionMaxLength = 0;
		#endregion
		
		#region Basic LogLevel instance
		// Standard:
		// If LogLevel is omitted, Standard is used.
		public static LogLevel Standard { get; } = new LogLevel( WriteLog, true, true, ConsoleColor.White, ConsoleColor.Black, true );
		#endregion
		
		////////////////////////////////////////////////////////////////////////
		#region Bridge to Standard
		
		public static void WriteLine( string s )
		{
			Standard.Write( s + "\n", 1 );
		}
		
		public static void Write( string s )
		{
			Standard.Write( s, 1 );
		}
		
		public static void Separate()
		{
			Standard.Separate();
		}
		
		#endregion
		
		////////////////////////////////////////////////////////////////////////
		#region Registration for LogLevels
		
		public static Dictionary<string,LogLevel> Channels = new Dictionary<string,LogLevel>();
		
		public static LogLevel Register( bool f, bool c, ConsoleColor fc, ConsoleColor bc, bool t, string caption )
		{
			var l = new LogLevel( WriteLog, f, c, fc, bc, t, caption );
			Channels[ caption ] = l;
			
			foreach( var ch in Channels )
			{
				if( ch.Value.Caption.Length > CaptionMaxLength )
				{
					CaptionMaxLength = ch.Value.Caption.Length;
				}
			}
			
			return l;
		}
		
		#endregion
		
		////////////////////////////////////////////////////////////////////////
		#region Output process
		
		/* ------------------------------------------------------------
			To avoid to output date,time and caption,
			in the next log of a log that doesn't end in a NewLine code.
		------------------------------------------------------------ */
		private static bool IsContinuousLine( string s )
		{
			return s[ s.Length - 1 ] != '\n';
		}
		
		private static DateTime LastTimeC;
		private static DateTime LastTimeF;
		
		/* ------------------------------------------------------------
			Write a log
		------------------------------------------------------------ */
		private static void WriteLog( ConsoleColor fc, ConsoleColor bc, string d, string fl, string c, string s, bool ce, bool fe )
		{
			lock( Sem )
			{
				if( ( ce && ConsoleEnabled ) || ( fe && FileEnabled ) )
				{
					var now = DateTime.Now;
					// string dc = ( c != "" ) ? $"[{c.PadRight(CaptionMaxLength)}] " : new string( ' ', CaptionMaxLength + 3 );
					string dc = ( c != "" ) ? $"[{c.PadRight(CaptionMaxLength)}] " : "";
					
					if( fl.Length > FileNameColumns )
					{
						FileNameColumns = fl.Length;
					}
					
					string fileAndLine = fl.PadRight( FileNameColumns );
					
					if( ce && ConsoleEnabled )
					{
						var dif = (int)( now - LastTimeC ).TotalMilliseconds;
						var difS = String.Format( "{0,6}", dif );
						string ds = ( d != "" ) ? $"{d} (+{difS}) " : "";
						string logStr = Continuous ? s : $"{ds}{fileAndLine} {dc} {s}";
						
						Console.ForegroundColor = fc;
						Console.BackgroundColor = bc;
						
						Console.Write( logStr );
						
						Console.ResetColor();
						
						LastTimeC = now;
					}
					
					if( fe && FileEnabled )
					{
						var dif = (int)( now - LastTimeF ).TotalMilliseconds;
						var difS = String.Format( "{0,6}", dif );
						string ds = ( d != "" ) ? $"{d} (+{difS}) " : "";
						string logStr = Continuous ? s : $"{ds}{fileAndLine} {dc} {s}";
						
						try
						{
							using( var sw = new StreamWriter( Folder + BaseName, true ) )
							{
								sw.Write( logStr );
							}
						}
						catch
						{
							Console.Write( "Log writing error to {BaseName}" );
							Console.Write( logStr );
						}
						
						LastTimeF = now;
					}
					
					Continuous = IsContinuousLine(s);
				}
			}
		}
		
		#endregion
		
		////////////////////////////////////////////////////////////////////////
		#region Other controller
		
		/* ------------------------------------------------------------
			Delete old log files
		------------------------------------------------------------ */
		public static void Cleanup( int days )
		{
			var files = Directory.GetFiles( Folder, "*.log", SearchOption.TopDirectoryOnly );
			foreach( var f in files )
			{
				Standard.Write( f );
				var fn = Path.GetFileNameWithoutExtension( f );
				var dt = DateTime.ParseExact( fn, "yyyy-MM-dd", null );
				var dif = DateTime.Now - dt;
				Standard.Write( $" -> Dif={dif.Days}" );
				
				if( dif.Days >= days )
				{
					Standard.Write( " : Expired" );
					File.Delete( f );
				}
				
				Standard.WriteLine( "" );
			}
		}
		
		public static void Begin( string file )
		{
			BeginConsole();
			BeginFile( file );
		}
		
		public static void BeginConsole( bool show = true )
		{
			ConsoleEnabled = true;
			LastTimeC = DateTime.Now;

			if( show )
			{
				ShowConsole();
				SetConsoleSize( 140 * 8, 40 * 16 );
			}
		}
		
		public static void BeginFile( string file )
		{
			Folder = file;
			FileEnabled = true;
			LastTimeF = DateTime.Now;
		}
		
		#endregion
		
		////////////////////////////////////////////////////////////////////////
		#region GUI
		
		// You can use LogLevels setting GUI
		// If you need to use GUI, change this #if to true and execute
		// SosLogger.ShowUI();
		#if true
		private static LogUI LogForm = null!;
		public static void ShowUI()
		{
			if( ( LogForm == null ) || LogForm.IsDisposed )
			{
				LogForm = new LogUI();
				foreach( var c in Channels )
				{
					LogForm.Add( c.Value, c.Key );
				}
			}
			
			LogForm.Show();
			LogForm.BringToForward();
		}
		#endif
		
		#endregion
		
		////////////////////////////////////////////////////////////////////////
		#region Console
		
        [DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		

		public static void ShowConsole()
		{
			AllocConsole();
		}

		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();
		public static void HideConsole()
		{
			FreeConsole();
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetConsoleWindow();
		
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

	    [StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		public static void MoveConsole( int x, int y, int w, int h )
		{
			MoveWindow( GetConsoleWindow(), x, y, w, h, true );
		}

		public static void SetConsoleSize( int w, int h )
		{
			IntPtr hWnd = GetConsoleWindow();
			GetWindowRect( hWnd, out RECT rect );
			MoveWindow( hWnd, rect.Left, rect.Top, w, h, true );
		}
		
		#endregion
	}
}
