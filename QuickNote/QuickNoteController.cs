using System;
using QuLib;

namespace QuickNote
{
	public class QuickNoteController : IQuModule
	{
		#region モジュールとして必要な情報

		// ホストに対して操作するため
		private IQuHost Host = null!;

		private readonly QuickNoteForm NoteForm = new();
		
		private Dictionary<string,string> Notes = new();

		#endregion

		#region 設定できるようにする項目


		#endregion

		#region インターフェースの実装（実装者が編集する必要がある項目）

		// TODO モジュールの基本的な情報を設定してください
		public string Name => "QuickNote";
		public string Version => System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "??";

		#endregion

		#region インターフェースの実装：ホストからの捜査

		// アプリの初期化時に実行
		public void Initialize(IQuHost h)
		{
			Host = h;

			// TODO
			// 操作のためのキー操作
			Hotkeys = new() {
				// 操作           メソッド
				{ Host.LoadModuleSetting("Launch", "ctrl+alt+n"), Launch },
			};
			
			// 設定読み込み

			// TODO
			// 配下のコントロールで、
			// マウスポイントしたときにマウスカーソルを変化させたくないものの一覧
			CursorFixer = new Control[] {
			};

			// TODO
			// GUIを所持する場合、MainContainer に関連付けます
			MainContainer = NoteForm;

			// TODO
			// その他、初期化が必要な場合はここに書きます
			
			NoteForm.Clear();
			NoteForm.NoteUpdated += Update;
			NoteForm.NoteRemoved += Remove;

			var keys = Host.GetAllStatusKeys();
			foreach( var key in keys )
			{
				string note = Host.LoadModuleStatus( key, "" );
				if (note != "")
				{
					Notes[key] = Decode(note);
					Host.LogN($"Loaded [{key}] {note}");
				}
				else
				{
					Host.LogN($"Loaded [{key}] was removed");
				}
			}

			NoteForm.Hide();
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

		public Dictionary<string, Action> Hotkeys { get; private set; } = null!;
		public Control[] CursorFixer { get; private set; } = null!;

		public System.Windows.Forms.UserControl MainContainer { get; private set; } = null!;

		#endregion

		#region ホットキーから実行するメソッド

		private void Launch()
		{
			if( NoteForm.Visible )
			{
				Host.LogN("Hide");
				NoteForm.Hide();
				Host.RequestDeactivate();
			}
			else
			{
				Host.LogN("Show");
				
				NoteForm.Clear();
				foreach( var n in Notes )
				{
					NoteForm.Add( n.Key, n.Value );
				}
				
				NoteForm.Show();
				NoteForm.AdjustWidth( Host.Width );
				Host.RequestActivate();
				
				NoteForm.Invoke( () =>
				{
					NoteForm.Focus();
				} );
			}
		}

		#endregion
		
		#region その他
		
		private void Update( string key, string val )
		{
			Host.LogN( $"Update note [{key}] -> {val}" );
			if( val == "" )
			{
				Remove(key);
			}
			else
			{
				Notes[key] = val;
				NoteForm.Add(key, val);
				Host.SaveModuleStatus(key, Encode(val));
			}
		}

		private void Remove( string key )
		{
			Host.LogN($"Remove note [{key}]");
			Notes.Remove( key );
			NoteForm.Remove( key );
			Host.SaveModuleStatus(key, "");
		}
		
		private static string Encode( string str )
		{
			return str.Replace("\r", "").Replace("\n", "\\n");
		}

		private static string Decode( string str )
		{
			return str.Replace("\\n", "\n");
		}

		#endregion
	}
}
