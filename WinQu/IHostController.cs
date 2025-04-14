using System;
using QuLib;

namespace WinQu
{
	public interface IHostController
	{
		string Version { get; }
		
		// モジュール個別で定義した設定を取得・保存
		string LoadModuleSetting( string module, string key, string def );
		void   SaveModuleSetting( string module, string key, string val );
		Color  LoadColorSetting( string module, string key, string val );
		Font LoadFontSetting( string module, string key, string defName, int defSize );
		List<string> GetAllSettingKeys( string module );
		List<string> GetAllStatusKeys( string module );
		
		// モジュール個別で定義した状態を取得・保存
		string LoadModuleStatus( string module, string key, string def );
		void   SaveModuleStatus( string module, string key, string val );
		
		// ホストに対する要求
		void RequestActivate( IQuModule module );
		void RequestDeactivate( IQuModule module );
		
		// メインウィンドウに対する要求
		void ActivateForm( IQuModule module );
		
		// ログの記録要求
		void Log( string module, LogLevel ll, string message, int depth = 0 );
		void Log( string module, string message, int depth = 0 );
		void ShowOSD(string message, int time, Font? txtFont, Color txtColor, Color backColor, Action? finished);

		// ホットキー個別操作
		List<string> HotkeyKeys { get; } // ホットキーの一覧を取得
		void RegisterHotkey(string key, Action action); // ホットキーの登録	
		void UnregisterHotkey(string key); // ホットキーの解除


		int Opac  { get; } // 透過度
		System.Drawing.Font Font { get; } // フォント
		
		int Width  { get; }
		int Height { get; }
		int X { get; }
		int Y { get; }
		bool Visible { get; }
		
		event QuHostEventHandler WindowSizeChanged;
		event QuHostEventHandler WindowLocationChanged;
		event QuHostEventHandler Shown;
		event QuHostEventHandler Hidden;
	}
}
