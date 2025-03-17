namespace QuLib
{
	/* ------------------------------------------------------------
		各モジュールが本体にアクセスするためのインターフェース
	------------------------------------------------------------ */
	public interface IQuHost : IDefaultSettings, IHostStatus, IHostEvents
	{
		// 本体のバージョン取得
		string Version { get; }
		
		// モジュール個別で定義した設定を取得・保存
		string LoadModuleSetting( string key, string def );
		void   SaveModuleSetting( string key, string val );
		Color  LoadColorSetting( string key, string val );
		Font   LoadFontSetting( string key, string defName, int defSize );
		List<string> GetAllSettingKeys();
		
		// モジュール個別で定義した状態を取得・保存
		string LoadModuleStatus( string key, string def );
		void   SaveModuleStatus( string key, string val );
		List<string> GetAllStatusKeys();
		
		// ホストに対する要求
		void RequestActivate();   // 自身をアクティブにする
		void RequestDeactivate(); // 自身を非アクティブにする
		
		// ログの記録要求
		void Log( LogLevel ll, string message, int depth = 0 );
		void LogF( string message, int depth = 0 ) => Log( LogLevel.Fatal,   message, depth );
		void LogE( string message, int depth = 0 ) => Log( LogLevel.Error,   message, depth );
		void LogW( string message, int depth = 0 ) => Log( LogLevel.Warning, message, depth );
		void LogN( string message, int depth = 0 ) => Log( LogLevel.Notice,  message, depth );
		void LogD( string message, int depth = 0 ) => Log( LogLevel.Debug,   message, depth );
		void LogV( string message, int depth = 0 ) => Log( LogLevel.Verbose, message, depth );
		
		// デフォルト実装 → 下記interface参照
		IDefaultSettings DefaultSettings { get => this; } // 基本設定を得る
		IHostStatus      Status          { get => this; } // 本体の状態を得る
		IHostEvents      Event           { get => this; } // ホスト側に登録できるイベント
		void Log(string message) => Log(LogLevel.Debug, message);
		
		// メインウィンドウに対する要求
		void RequestActivateForm();
	}
	
	// ホスト側から基本設定
	public interface IDefaultSettings
	{
		int Opac  { get; }                // 透過度
		System.Drawing.Font Font { get; } // フォント
	}
	
	// ホスト側の状態
	public interface IHostStatus
	{
		// メインウィンドウのサイズ
		int Width  { get; }
		int Height { get; }
		
		// デスクトップの位置
		int X { get; }
		int Y { get; }
		
		// 現在の表示状態
		bool Visible { get; }
	}
	
	// ホストからのイベント
	// モジュール側のインターフェースに書くと、モジュール実装者の負担になるため
	public delegate void QuHostEventHandler( IQuModule m );
	
	public interface IHostEvents
	{
		event QuHostEventHandler WindowSizeChanged;     // メインウィンドウのサイズが変わった
		event QuHostEventHandler WindowLocationChanged; // メインウィンドウの位置が変わった
		event QuHostEventHandler Shown;                 // メインウィンドウが表示された
		event QuHostEventHandler Hidden;                // メインウィンドウが非表示になった
	}
}
