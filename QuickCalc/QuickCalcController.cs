using System;
using System.Diagnostics;
using QuLib;

namespace QuickCalc
{
	public class QuickCalcController : IQuModule
	{
		#region モジュールとして必要な情報
		
		// ホストに対して操作するため
		private IQuHost Host = null!;
		
		// 処理本体
		private readonly QuickCalc CalcFunction = new();
		
		// UI
		private readonly CalcForm Form = new();
		
		public string IniCategory { get; } = "QuickCalc";
		
		#endregion
		
		#region 初期化関係
		
		public QuickCalcController()
		{
		}
		
		#endregion
		
		#region インターフェースの実装（実装者が編集する必要がある項目）
		
		public string Name    => "QuickCalc";
		public string Version => System.Reflection.Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "??";
		
		#endregion
		
		#region インターフェースの実装：ホストからの操作
		
		// アプリの初期化時に実行
		public void Initialize( IQuHost h )
		{
			Host = h;
			CalcFunction.Initialize();

			// 設定読み込み
			string launchKey = Host.LoadModuleSetting( "Launch", "Ctrl+Alt+Enter" );
			
			Form.ForeResult                = SetColor( "ForeResult",     "White" );
			Form.BackResult                = SetColor( "BackResult",     "DimGray" );
			Form.ForeResultHex             = SetColor( "ForeResultHex",  "White" );
			Form.BackResultHex             = SetColor( "BackResultHex",  "DimGray" );
			Form.ForeExpression            = SetColor( "ForeExpression", "White" );
			Form.BackExpression            = SetColor( "BackExpression", "Black" );
			Form.ForeResultPast            = SetColor( "ForeResultPast",    "LightGray" );
			Form.ForeResultHexPast         = SetColor( "ForeResultHexPast", "LightGray" );
			Form.ForeExpressionDeactivated = SetColor( "ForeExpressionDeactivated", "White" );
			Form.BackExpressionDeactivated = SetColor( "BackExpressionDeactivated", "DarkGray" );
			
			Form.ResultFont     = Host.LoadFontSetting( "Font_Result", "", 17 );
			Form.ResultHexFont  = Host.LoadFontSetting( "Font_Hex", "", 12 );
			Form.ExpressionFont = Host.LoadFontSetting( "Font_Expression", "", 14 );
			
			Form.MaxHistory = Int32.Parse( Host.LoadModuleSetting( "MaxHistory", "32" ) );

			// TODO
			// 操作のためのキー操作
			Hotkeys = new() {
				// 操作      メソッド
				{ launchKey, Launch },
			};
			
			// TODO
			// 配下のコントロールで、
			// マウスポイントしたときにマウスカーソルを変化させたくないものの一覧
			CursorFixer = Form.CursorFixer;
			
			// TODO
			// GUIを所持する場合、MainContainer に関連付けます
			MainContainer = Form;

			// TODO
			// その他、初期化が必要な場合はここに書きます
			Form.RequestCalculate  += e => Calculate(e);
			Form.RequestLog        += (lv,msg) => Host.Log( lv, $"(Form) {msg}", 2 );
			Form.RequestInitialize += () => CalcFunction.Initialize();
			Form.Hide();
			
			return;
			
			// Locl functions
			Color SetColor( string type, string def )
			{
				var colStr = Host.LoadModuleSetting( type, def );
				try
				{
					return ColorTranslator.FromHtml( colStr );
				}
				catch
				{
					string msg = $"Color specification {colStr} is invalid.";
					MessageBox.Show(
						msg,
						"Karculator : Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
					Host.Log( LogLevel.Error, msg );
					throw;
				}
			}
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
			Host.Log(LogLevel.Notice, "Activated");
			Launch();
		}
		
		// ホストがモジュールを非活性化するときに実行
		public void Deactivate()
		{
			// TODO
			// モジュールを終了するときに必要な処理をここに書きます
			Host.Log(LogLevel.Notice, "Deactivated");
			HideCalculator();
		}
		
		public void AppsActivated()
		{
			; // nothing to do
		}
		
		public void AppsDeactivated()
		{
			; // nothing to do
		}
		
		#endregion
		
		#region インターフェースの実装（定義のみ）
		
		public Dictionary<string,Action> Hotkeys { get; private set; } = null!;
		public Control[] CursorFixer { get; private set; } = null!;
		
		public System.Windows.Forms.UserControl MainContainer { get; private set; } = null!;
		
		#endregion
		
		#region 操作
		
		private void Launch()
		{
			if( Form.Visible )
			{
				HideCalculator();
			}
			else
			{
				ShowCalculator();
			}
		}
		
		private void ShowCalculator()
		{
			Host.Log(LogLevel.Notice, "Launch: Show");
			Form.Invoke(() =>
			{
				Form.Activate();
				Form.Show();
			});
			Host.RequestActivate();
			Host.RequestActivateForm();
		}
		
		private void HideCalculator()
		{
			Host.Log(LogLevel.Notice, "Launch : Hide");
			Form.Invoke(() =>
			{
				Form.Hide();
				Form.Deactivate();
			});
			Host.RequestDeactivate();
		}

		public double Calculate(string expression)
		{
			var ret = CalcFunction.Calculate(expression);
			Host.Log(LogLevel.Notice, $"Calc {expression} -> {ret}");
			return ret;
		}

		#endregion
	}
}
