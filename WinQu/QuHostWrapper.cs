using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Diagnostics;

using QuLib;

namespace WinQu
{
	/**
		モジュールがホストに対してアクセスしやすくするためのクラス
		各モジュールに対してQuHostWrapperのインスンス１つが割り当てられ、
		モジュール情報を付加してHostにアクセスする
	*/
	public class QuHostWrapper : IQuHost
	{
		#region ホストからの操作
		
		public IQuModule       Module { get; init; }
		public IHostController Host   { get; init; }
		
		public QuHostWrapper( IHostController h, IQuModule m )
		{
			Host = h;
			Module = m;
		}
		
		#endregion
		
		#region IQuteHostの実装
		
		public string Version => Host.Version;
		public string LoadModuleSetting( string key, string def ) => Host.LoadModuleSetting( Module.Name, key, def );
		public void   SaveModuleSetting( string key, string val ) => Host.SaveModuleSetting( Module.Name, key, val );
		public Color  LoadColorSetting( string key, string def )  => Host.LoadColorSetting( Module.Name, key, def );
		public Font   LoadFontSetting( string key, string defName, int defSize ) => Host.LoadFontSetting( Module.Name, key, defName, defSize );
		public List<string> GetAllSettingKeys()                   => Host.GetAllSettingKeys( Module.Name );
		public string LoadModuleStatus( string key, string def )  => Host.LoadModuleStatus( Module.Name, key, def );
		public void   SaveModuleStatus( string key, string val )  => Host.SaveModuleStatus( Module.Name, key, val );
		public List<string> GetAllStatusKeys()                    => Host.GetAllStatusKeys( Module.Name );
		public void   RequestActivate()                           => Host.RequestActivate( Module );
		public void   RequestDeactivate()                         => Host.RequestDeactivate( Module );
		public void   Log( LogLevel ll, string message, int depth ) => Host.Log( Module.Name, ll, message, depth );
		
		public int Opac     => Host.Opac;
		public Font Font    => Host.Font;
		public int Width    => Host.Width;
		public int Height   => Host.Height;
		public int X        => Host.X;
		public int Y        => Host.Y;
		public bool Visible => Host.Visible;
		
		public event QuHostEventHandler WindowSizeChanged
		{
			add    => Host.WindowSizeChanged += value;
			remove => Host.WindowSizeChanged -= value;
		}
		
		public event QuHostEventHandler WindowLocationChanged
		{
			add    => Host.WindowLocationChanged += value;
			remove => Host.WindowLocationChanged -= value;
		}
		
		public event QuHostEventHandler Shown
		{
			add    => Host.Shown += value;
			remove => Host.Shown -= value;
		}
		
		public event QuHostEventHandler Hidden
		{
			add    => Host.Hidden += value;
			remove => Host.Hidden -= value;
		}
		
		public void RequestActivateForm() => Host.ActivateForm( Module );
		public void ShowOSD(string message, int time, Font? txtFont, Color txtColor, Color backColor, Action? finished)
		{
			Host.ShowOSD(message, time, txtFont, txtColor, backColor, finished);
		}

		#endregion
	}
}
