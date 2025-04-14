namespace QuLib
{
	/* ------------------------------------------------------------
		ホストからモジュールに対するインターフェース
	------------------------------------------------------------ */
	public interface IQuModule
	{
		// Propreties
		string Name    { get; }                    // モジュール名
		string Version { get; }                    // モジュールのバージョン
		Control[] CursorFixer { get; }             // これらコントロールの上ではマウスカーソルを変化させない
		Dictionary<string,Action> Hotkeys { get; } // モジュールに対するホットキーと対応するメソッド
		
		// Controller
		void Initialize( IQuHost host );  // 初期化時にホストが実行
		void Deinitialize();              // アプリ終了時にホストが実行
		void Activate();                  // ホスト側からモジュールを起動するときに実行
		void Deactivate();                // ホスト側からモジュールを終了するときに実行
		void AppsActivated();             // アプリ自体がアクティブになるときに実行
		void AppsDeactivated();           // アプリ自体が非アクティブになるときに実行

		// UserControlとの接続
		// モジュールがGUIである場合、UserControlを非nullにして返す
		System.Windows.Forms.UserControl? MainContainer { get; }
		
		
		// MainContainer関係のデフォルト実装
		bool Visible
		{
			get => MainContainer?.Visible ?? false;
			set
			{
				if( MainContainer != null )
				{
					MainContainer.Visible = value;
				}
			}
		}
		
		int Height
		{
			get => MainContainer?.Height ?? 0;
			set
			{
				if( MainContainer != null )
				{
					MainContainer.Height = value;
				}
			}
		}
		
		int Width
		{
			get => MainContainer?.Width ?? 0;
			set
			{
				if( MainContainer != null )
				{
					MainContainer.Width = value;
				}
			}
		}
		
		void BringToFront()
		{
			if( MainContainer != null )
			{
				MainContainer.BringToFront();
			}
		}
		
		void SendToBack()
		{
			if( MainContainer != null )
			{
				MainContainer.SendToBack();
			}
		}
	}
}
