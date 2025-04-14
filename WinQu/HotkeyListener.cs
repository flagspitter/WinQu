using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;
using YacsLibrary;

namespace WinQu
{
	/***************************************************************
	
		Class Name :    HotkeyListener
		Extends    :    System.Windows.Forms.Control
		Interfaces :    None
		
		Purpose
			ホットキーの登録と押下確認するクラス
		
		Example:
			foo.Register( HotkeyListener.MOD_CONTROL | HotkeyListener.MOD_ALT, Keys.Q, (m,k) => bar );
			
			このような感じで登録する。
			この場合、これ以降、Ctrl+Alt+Q が押されると、(m,k) => bar が実行される。
			ここで m は修飾キーの状態、 k は押されたキーコードを示す。
			
			終了時は、 foo.UnregisterAll();
	
	***************************************************************/
	public class HotkeyListener : Control
	{
		////////////////////////////////////////////////////////////////
		#region 定数
		////////////////////////////////////////////////////////////////
		
		// メッセージ処理に使う
		private const int WM_HOTKEY = 0x0312;
		
		#endregion
		
		////////////////////////////////////////////////////////////////
		#region WindowsAPIのためのDLL
		////////////////////////////////////////////////////////////////
		
		[DllImport("user32.dll")]
		extern static bool RegisterHotKey(IntPtr HWnd, int ID, int MOD_KEY, int KEY);
		
		[DllImport("user32.dll")]
		extern static bool UnregisterHotKey(IntPtr HWnd, int ID);
		
		#endregion
		
		////////////////////////////////////////////////////////////////
		#region 内外で使用する型
		////////////////////////////////////////////////////////////////
		
		private class RegList(Action<int, Keys> a, int i, int m, Keys k)
		{
			public Action<int, Keys> Act = a;
			public int id = i;
			public int Mod = m;
			public Keys Key = k;
		}

		#endregion

		////////////////////////////////////////////////////////////////
		#region 内部変数
		////////////////////////////////////////////////////////////////

		private readonly List<RegList> Callbacks = new();
		private int Count;

		#endregion

		////////////////////////////////////////////////////////////////
		#region 公開メソッド
		////////////////////////////////////////////////////////////////
		
		public int Register(string key, Action<int, Keys> cf)
		{
			int ret = -1;

			if (key != "")
			{
				string[] keylist = key.Split('+');
				int modKey = 0;
				Keys? normalKey = null;

				for (int i = 0; i < keylist.Length - 1; i++)
				{
					modKey |= KeyCode.Str2ModKey(keylist[i]);
				}

				if (keylist.Length > 0)
				{
					normalKey = KeyCode.Str2Keys(keylist[keylist.Length - 1]);
				}

				if (normalKey != null)
				{
					ret = Register(modKey, normalKey.Value, cf);
				}
			}

			return ret;
		}

		public int Register( int modKey, Keys key, Action<int, Keys> act )
		{
			int curId = Count;
			bool registered = RegisterHotKey( this.Handle, curId, modKey, (int)key );
			if( registered )
			{
				Callbacks.Add( new RegList( act, curId, modKey, key ) );
				Count++;

				Log.D($"Registered hot key {modKey} {key}");
			}
			else
			{
				curId = -1;
				Log.E($"Failed to register hot key {modKey} {key}");
			}

			return curId;
		}
		
		public void Unregister( int id )
		{
			foreach( var tg in Callbacks )
			{
				if( tg.id == id )
				{
					UnregisterHotKey( this.Handle, tg.id );
				}
			}
		}
		
		public void UnregisterAll()
		{
			foreach( var tg in Callbacks )
			{
				UnregisterHotKey( this.Handle, tg.id );
			}
		}
		
		#endregion
		
		////////////////////////////////////////////////////////////////
		#region オーバーライドする関数
		////////////////////////////////////////////////////////////////
		
		protected override void WndProc( ref Message m )
		{
			// Log.D($"begin HotkeyListener.WndProc Msg=0x{m.Msg:X08}");
			if( m.Msg == WM_HOTKEY )
			{
				int key = (int)m.WParam;

				RegList? target = Callbacks.Find( (x) => ( x.id == key ) );
				if( target != null )
				{
					target.Act( target.Mod, target.Key );
				}
			}
			base.WndProc(ref m);
		}

		#endregion

		public HotkeyListener()
		{
			this.Disposed += (_,_) => UnregisterAll();
		}
	}
}
