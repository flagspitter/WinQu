using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace WinQu
{
	
	public static class Misc
	{
		////////////////////////////////////////////////////////////////
		#region コンソール表示関係
		////////////////////////////////////////////////////////////////
		
		private const UInt32 StdOutputHandle = 0xFFFFFFF5;
		
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetStdHandle(UInt32 nStdHandle);
		
		[DllImport("kernel32.dll")]
		private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);
		
		[DllImport("kernel32")]
		private static extern bool AllocConsole();
		
		[DllImport("kernel32")]
		private static extern bool FreeConsole();
		
		public static void ShowConsole()
		{
			AllocConsole();
			
			IntPtr defaultStdout = new IntPtr(7);
			IntPtr currentStdout = GetStdHandle(StdOutputHandle);
			
			if( currentStdout != defaultStdout )
			{
				// reset stdout
				SetStdHandle( StdOutputHandle, defaultStdout );
			}
			
			// reopen stdout
			TextWriter writer =
				new StreamWriter( Console.OpenStandardOutput(), System.Text.Encoding.GetEncoding("shift_jis") ) {
					AutoFlush = true
				};
			
			Console.SetOut( writer );
		}
		
		public static void HideConsole()
		{
			FreeConsole();
		}
		
		#endregion
	}
	
	public static class KeyCode
	{
		public const int MOD_ALT = 0x0001;
		public const int MOD_CONTROL = 0x0002;
		public const int MOD_SHIFT = 0x0004;
		public const int MOD_WIN = 0x0008;

		public const int MOD_CONTROL_ALT = MOD_CONTROL | MOD_ALT;
		public const int MOD_CONTROL_SHIFT = MOD_CONTROL | MOD_SHIFT;
		public const int MOD_CONTROL_WIN = MOD_CONTROL | MOD_WIN;

		public const int MOD_SHIFT_ALT = MOD_SHIFT | MOD_ALT;
		public const int MOD_SHIFT_CTRL = MOD_SHIFT | MOD_CONTROL;
		public const int MOD_SHIFT_WIN = MOD_SHIFT | MOD_WIN;

		public const int MOD_WIN_ALT = MOD_WIN | MOD_ALT;
		public const int MOD_WIN_CTRL = MOD_WIN | MOD_CONTROL;
		public const int MOD_WIN_SHIFT = MOD_WIN | MOD_SHIFT;

		public const int MOD_CONTROL_SHIFT_ALT = MOD_CONTROL | MOD_SHIFT | MOD_ALT;
		public const int MOD_CONTROL_SHIFT_WIN = MOD_CONTROL | MOD_SHIFT | MOD_WIN;
		public const int MOD_CONTROL_ALT_WIN = MOD_CONTROL | MOD_ALT | MOD_WIN;
		public const int MOD_ALT_SHIFT_WIN = MOD_ALT | MOD_SHIFT | MOD_WIN;

		////////////////////////////////////////////////////////////////
		#region キーコード変換
		////////////////////////////////////////////////////////////////

		public static Keys Str2Keys(string str)
		{
			return str.ToUpper() switch
			{
				"A" => Keys.A,
				"ADD" => Keys.Add,
				"ALT" => Keys.Alt,
				"APPS" => Keys.Apps,
				"ATTN" => Keys.Attn,
				"B" => Keys.B,
				"BACK" => Keys.Back,
				"BROWSERBACK" => Keys.BrowserBack,
				"BROWSERFAVORITES" => Keys.BrowserFavorites,
				"BROWSERFORWARD" => Keys.BrowserForward,
				"BROWSERHOME" => Keys.BrowserHome,
				"BROWSERREFRESH" => Keys.BrowserRefresh,
				"BROWSERSEARCH" => Keys.BrowserSearch,
				"BROWSERSTOP" => Keys.BrowserStop,
				"C" => Keys.C,
				"CANCEL" => Keys.Cancel,
				"CAPITAL" => Keys.Capital,
				"CAPSLOCK" => Keys.CapsLock,
				"CLEAR" => Keys.Clear,
				"CONTROL" => Keys.Control,
				"CONTROLKEY" => Keys.ControlKey,
				"CRSEL" => Keys.Crsel,
				"D" => Keys.D,
				"D0" => Keys.D0,
				"D1" => Keys.D1,
				"D2" => Keys.D2,
				"D3" => Keys.D3,
				"D4" => Keys.D4,
				"D5" => Keys.D5,
				"D6" => Keys.D6,
				"D7" => Keys.D7,
				"D8" => Keys.D8,
				"D9" => Keys.D9,
				"0" => Keys.D0,
				"1" => Keys.D1,
				"2" => Keys.D2,
				"3" => Keys.D3,
				"4" => Keys.D4,
				"5" => Keys.D5,
				"6" => Keys.D6,
				"7" => Keys.D7,
				"8" => Keys.D8,
				"9" => Keys.D9,
				"DECIMAL" => Keys.Decimal,
				"DELETE" => Keys.Delete,
				"DIVIDE" => Keys.Divide,
				"DOWN" => Keys.Down,
				"E" => Keys.E,
				"END" => Keys.End,
				"ENTER" => Keys.Enter,
				"ERASEEOF" => Keys.EraseEof,
				"ESCAPE" => Keys.Escape,
				"EXECUTE" => Keys.Execute,
				"EXSEL" => Keys.Exsel,
				"F" => Keys.F,
				"F1" => Keys.F1,
				"F10" => Keys.F10,
				"F11" => Keys.F11,
				"F12" => Keys.F12,
				"F13" => Keys.F13,
				"F14" => Keys.F14,
				"F15" => Keys.F15,
				"F16" => Keys.F16,
				"F17" => Keys.F17,
				"F18" => Keys.F18,
				"F19" => Keys.F19,
				"F2" => Keys.F2,
				"F20" => Keys.F20,
				"F21" => Keys.F21,
				"F22" => Keys.F22,
				"F23" => Keys.F23,
				"F24" => Keys.F24,
				"F3" => Keys.F3,
				"F4" => Keys.F4,
				"F5" => Keys.F5,
				"F6" => Keys.F6,
				"F7" => Keys.F7,
				"F8" => Keys.F8,
				"F9" => Keys.F9,
				"FINALMODE" => Keys.FinalMode,
				"G" => Keys.G,
				"H" => Keys.H,
				"HANGUELMODE" => Keys.HanguelMode,
				"HANGULMODE" => Keys.HangulMode,
				"HANJAMODE" => Keys.HanjaMode,
				"HELP" => Keys.Help,
				"HOME" => Keys.Home,
				"I" => Keys.I,
				"IMEACCEPT" => Keys.IMEAccept,
				"IMEACEEPT" => Keys.IMEAceept,
				"IMECONVERT" => Keys.IMEConvert,
				"IMEMODECHANGE" => Keys.IMEModeChange,
				"IMENONCONVERT" => Keys.IMENonconvert,
				"INSERT" => Keys.Insert,
				"J" => Keys.J,
				"JUNJAMODE" => Keys.JunjaMode,
				"K" => Keys.K,
				"KANAMODE" => Keys.KanaMode,
				"KANJIMODE" => Keys.KanjiMode,
				"KEYCODE" => Keys.KeyCode,
				"L" => Keys.L,
				"LAUNCHAPPLICATION1" => Keys.LaunchApplication1,
				"LAUNCHAPPLICATION2" => Keys.LaunchApplication2,
				"LAUNCHMAIL" => Keys.LaunchMail,
				"LBUTTON" => Keys.LButton,
				"LCONTROLKEY" => Keys.LControlKey,
				"LEFT" => Keys.Left,
				"LINEFEED" => Keys.LineFeed,
				"LMENU" => Keys.LMenu,
				"LSHIFTKEY" => Keys.LShiftKey,
				"LWIN" => Keys.LWin,
				"M" => Keys.M,
				"MBUTTON" => Keys.MButton,
				"MEDIANEXTTRACK" => Keys.MediaNextTrack,
				"MEDIAPLAYPAUSE" => Keys.MediaPlayPause,
				"MEDIAPREVIOUSTRACK" => Keys.MediaPreviousTrack,
				"MEDIASTOP" => Keys.MediaStop,
				"MENU" => Keys.Menu,
				"MODIFIERS" => Keys.Modifiers,
				"MULTIPLY" => Keys.Multiply,
				"N" => Keys.N,
				"NEXT" => Keys.Next,
				"NONAME" => Keys.NoName,
				"NONE" => Keys.None,
				"NUMLOCK" => Keys.NumLock,
				"NUMPAD0" => Keys.NumPad0,
				"NUMPAD1" => Keys.NumPad1,
				"NUMPAD2" => Keys.NumPad2,
				"NUMPAD3" => Keys.NumPad3,
				"NUMPAD4" => Keys.NumPad4,
				"NUMPAD5" => Keys.NumPad5,
				"NUMPAD6" => Keys.NumPad6,
				"NUMPAD7" => Keys.NumPad7,
				"NUMPAD8" => Keys.NumPad8,
				"NUMPAD9" => Keys.NumPad9,
				"O" => Keys.O,
				"OEM1" => Keys.Oem1,
				"OEM102" => Keys.Oem102,
				"OEM2" => Keys.Oem2,
				"OEM3" => Keys.Oem3,
				"OEM4" => Keys.Oem4,
				"OEM5" => Keys.Oem5,
				"OEM6" => Keys.Oem6,
				"OEM7" => Keys.Oem7,
				"OEM8" => Keys.Oem8,
				"OEMBACKSLASH" => Keys.OemBackslash,
				"OEMCLEAR" => Keys.OemClear,
				"OEMCLOSEBRACKETS" => Keys.OemCloseBrackets,
				"OEMCOMMA" => Keys.Oemcomma,
				"OEMMINUS" => Keys.OemMinus,
				"OEMOPENBRACKETS" => Keys.OemOpenBrackets,
				"OEMPERIOD" => Keys.OemPeriod,
				"OEMPIPE" => Keys.OemPipe,
				"OEMPLUS" => Keys.Oemplus,
				"OEMQUESTION" => Keys.OemQuestion,
				"OEMQUOTES" => Keys.OemQuotes,
				"OEMSEMICOLON" => Keys.OemSemicolon,
				"OEMTILDE" => Keys.Oemtilde,
				"P" => Keys.P,
				"PA1" => Keys.Pa1,
				"PACKET" => Keys.Packet,
				"PAGEDOWN" => Keys.PageDown,
				"PAGEUP" => Keys.PageUp,
				"PAUSE" => Keys.Pause,
				"PLAY" => Keys.Play,
				"PRINT" => Keys.Print,
				"PRINTSCREEN" => Keys.PrintScreen,
				"PRIOR" => Keys.Prior,
				"PROCESSKEY" => Keys.ProcessKey,
				"Q" => Keys.Q,
				"R" => Keys.R,
				"RBUTTON" => Keys.RButton,
				"RCONTROLKEY" => Keys.RControlKey,
				"RETURN" => Keys.Return,
				"RIGHT" => Keys.Right,
				"RMENU" => Keys.RMenu,
				"RSHIFTKEY" => Keys.RShiftKey,
				"RWIN" => Keys.RWin,
				"S" => Keys.S,
				"SCROLL" => Keys.Scroll,
				"SELECT" => Keys.Select,
				"SELECTMEDIA" => Keys.SelectMedia,
				"SEPARATOR" => Keys.Separator,
				"SHIFT" => Keys.Shift,
				"SHIFTKEY" => Keys.ShiftKey,
				"SLEEP" => Keys.Sleep,
				"SNAPSHOT" => Keys.Snapshot,
				"SPACE" => Keys.Space,
				"SUBTRACT" => Keys.Subtract,
				"T" => Keys.T,
				"TAB" => Keys.Tab,
				"U" => Keys.U,
				"UP" => Keys.Up,
				"V" => Keys.V,
				"VOLUMEDOWN" => Keys.VolumeDown,
				"VOLUMEMUTE" => Keys.VolumeMute,
				"VOLUMEUP" => Keys.VolumeUp,
				"W" => Keys.W,
				"X" => Keys.X,
				"XBUTTON1" => Keys.XButton1,
				"XBUTTON2" => Keys.XButton2,
				"Y" => Keys.Y,
				"Z" => Keys.Z,
				"ZOOM" => Keys.Zoom,

				_ => throw new ArgumentException($"{str} is invalid key specifier.")
			};
		}

		public static int Str2ModKey(string str)
		{
			return str.ToUpper() switch
			{
				"CTRL" or "CONTROL" => MOD_CONTROL,
				"SHIFT" => MOD_SHIFT,
				"ALT" => MOD_ALT,
				"WIN" or "CMD" or "COMMAND" => MOD_WIN,
				_ => 0
			};
		}

		#endregion
	}
}
