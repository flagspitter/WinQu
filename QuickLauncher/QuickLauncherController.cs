using QuLib;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QuickLauncher
{
	public partial class QuickLauncherController : IQuModule
	{
		#region モジュールとして必要な情報

		// ホストに対して操作するため
		private IQuHost Host = null!;

		private Dictionary<string, string> Launchers = new();

		#endregion

		#region インターフェースの実装（実装者が編集する必要がある項目）

		// TODO モジュールの基本的な情報を設定してください
		public string Name => "QuickLauncher";
		public string Version => "0.0.1";

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
				{ "ctrl+alt+l", Launch },
			};

			// keyof_<Key> = Execution,shortcut
			var settings = Host.GetAllSettingKeys();
			Host.LogD($"settings count = {settings.Count}");
			foreach ( var key in settings )
			{
				Host.LogD($"key = {key}");
				var keyParts = key.Split('_');
				if( keyParts.Length >= 2 )
				{
					if(keyParts[0].ToLower() == "keyof")
					{
						Host.LogD($"keyof_{keyParts[1]}");
						var settingParam = Host.LoadModuleSetting(key, "");
						var settingParams = SplitString( settingParam );
						Launchers[keyParts[1].ToLower()] = settingParams[0];
						Host.LogD($"param = {settingParam}");

						if ( settingParams.Count > 2 )
						{
							Host.LogD($"shotrcut = {settingParams[1]}");
							Host.RegisterHotkey(settingParams[2], () => RunProcess(settingParams[0], settingParams[1]));
						}
					}
				}
			}

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

		private void RunProcess( string cmd, string arg )
		{
			// 引数で与えられた内容を別プロセスで実行する
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = cmd, // 実行したいアプリケーションのパス
				Arguments = arg, // アプリケーションに渡す引数
				UseShellExecute = true, // シェルで実行する
				CreateNoWindow = true,  // ウィンドウを表示しない
			};

			Process process = new Process
			{
				StartInfo = startInfo
			};

			process.Start();

			// 呼び出し元のプロセスと切り離すために、後続の操作を行わない
			process.Dispose();
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
			Host.Log(LogLevel.Notice, "Sample is called");
		}

		#endregion

		#region misc

		private static List<string> SplitString(string input)
		{
			var result = new List<string>();

			// 正規表現: ダブルクォーテーション内と通常の要素を区別
			var regex = MyRegex();

			foreach (Match match in regex.Matches(input))
			{
				var s = match.Groups["match"].Value.Replace("\\\"", "\"");

				if( ( s[0] == '"' ) && (s[^1] == '"') )
				{
					s = s[1..^1]; // ダブルクォーテーションを除去
				}

				result.Add(s); // エスケープ解除
			}

			return result;
		}

		[GeneratedRegex(@"
			(?<match>       # 名前付きキャプチャグループ
			(?:             # 非キャプチャグループの開始
				(?<!\\)""  # ダブルクォーテーション開始（エスケープされていない）
				(?:         # 非キャプチャグループの開始
					[^""\\]|\\. # ダブルクォーテーション外とエスケープシーケンス
				)*          # 0回以上の繰り返し
				(?<!\\)""  # ダブルクォーテーション終了（エスケープされていない）
			|               # または
				[^,]+       # コンマ以外の通常の文字
			)
			)", RegexOptions.IgnorePatternWhitespace)]
		private static partial Regex MyRegex();

		#endregion
	}
}
