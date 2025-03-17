using System;

using static System.ConsoleColor;

// This file is treated as a configuration file.
// It is expected to be modified.

namespace YacsLibrary
{
	public static class Log
	{
		#region Syntax Sugar
		public static LogLevel Standard => SosLogger.Standard;
		public static void WriteLine( string s, int depath = 0 ) => Standard.Write( s + "\n", 1 );
		public static void Write( string s, int depth = 0 )      => Standard.Write( s, 1 );
		public static void Separate() => Standard.Separate();

		public static void F( string s, int depth = 0 ) => Fatal.Write( s + "\n", depth + 1 );
		public static void E( string s, int depth = 0 ) => Error.Write( s + "\n", depth + 1 );
		public static void W( string s, int depth = 0 ) => Warning.Write( s + "\n", depth + 1 );
		public static void N( string s, int depth = 0 ) => Notice.Write( s + "\n", depth + 1 );
		public static void D( string s, int depth = 0 ) => Debug.Write( s + "\n", depth + 1 );
		public static void V( string s, int depth = 0 ) => Verbose.Write( s + "\n", depth + 1 );
		#endregion

		#region Level definition
		//                                                            File   Console ForeColor BackColor Trace  Tag
		public static LogLevel Fatal   { get; } = SosLogger.Register( true,  true,   White,    Red,      true,  "FATAL" );
		public static LogLevel Error   { get; } = SosLogger.Register( true,  true,   Red,      Black,    true,  "ERROR" );
		public static LogLevel Warning { get; } = SosLogger.Register( true,  true,   Yellow,   Black,    true,  "WARNING" );
		public static LogLevel Notice  { get; } = SosLogger.Register( true,  true,   Green,    Black,    true,  "NOTICE" );
		public static LogLevel Debug   { get; } = SosLogger.Register( false, true,   White,    Black,    true,  "DEBUG" );
		public static LogLevel Verbose { get; } = SosLogger.Register( false, true,   DarkGray, Black,    false, "VERBOSE" );
		#endregion
	}
}
