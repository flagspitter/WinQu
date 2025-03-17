#if false

using System;
using QuLib;

namespace Hoge
{
	public class Hoge : IQuModule
	{
		#region モジュールとして必要な情報
		
		// ホストに対して操作するため
		private IQuHost Host = null!;
		
		#endregion
		
		#region インターフェースの実装（実装者が編集する必要がある項目）
		
		// TODO モジュールの基本的な情報を設定してください
		public string Name    => "ModuleName";
		public string Version  => "0.0.0";
		
		#endregion
		
		#region インターフェースの実装：ホストからの捜査
		
		// アプリの初期化時に実行
		public void Initialize( IQuHost h )
		{
			Host = h;
			
			// TODO
			// 操作のためのキー操作
			Hotkeys = new() {
				// 操作           メソッド
				{ "ctrl+shift+q", Sample },
			};
			
			// TODO
			// 配下のコントロールで、
			// マウスポイントしたときにマウスカーソルを変化させたくないものの一覧
			CursorFixer = new Control[] {
			};
			
			// TODO
			// GUIを所持する場合、MainContainer に関連付けます
			MainContainer = null!;
			
			// TODO
			// その他、初期化が必要な場合はここに書きます
		}
		
		// アプリの終了時に実行
		public void Deinitialize()
		{
			// TODO
			// 終了処理が必要な場合はここに書きます
		}
		
		// ホストがモジュールを活性化するときに実行
		public void Activate()
		{
			// TODO
			// モジュールを起動するときに必要な処理をここに書きます
		}
		
		// ホストがモジュールを非活性化するときに実行
		public void Deactivate()
		{
			// TODO
			// モジュールを終了するときに必要な処理をここに書きます
		}
		
		public void AppsActivated()
		{
			
		}
		
		public void AppsDeactivated()
		{
			
		}
		
		#endregion
		
		#region インターフェースの実装（定義のみ）
		
		public Dictionary<string,Action> Hotkeys { get; private set; } = null!;
		public Control[] CursorFixer { get; private set; } = null!;
		
		public System.Windows.Forms.UserControl MainContainer { get; private set; } = null!;
		
		#endregion
		
		#region ホットキーから実行するメソッド
		
		private void Sample()
		{
			Host.Log( LogLevel.Notice, "Sample is called" );
		}
		
		#endregion
	}
}

#endif
