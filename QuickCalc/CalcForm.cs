using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using QuLib;

namespace QuickCalc
{
	public partial class CalcForm : UserControl
	{
		////////////////////////////////////////////////////////////////
		#region メンバ変数等
		////////////////////////////////////////////////////////////////

		// 計算用
		private string ErasedExpression = ""; // for undo
		private readonly History Hist = new();

		#endregion

		////////////////////////////////////////////////////////////////
		#region プロパティ
		////////////////////////////////////////////////////////////////

		// 色
		public Color ForeResult                { get; set; } = Color.White;
		public Color BackResult                { get => lbResult.BackColor; set => lbResult.BackColor = value; }
		public Color ForeResultHex             { get; set; } = Color.White;
		public Color BackResultHex             { get => lbResultHex.BackColor; set => lbResultHex.BackColor = value; }
		public Color ForeExpression            { get => txtExpression.ForeColor; set => txtExpression.ForeColor = value; }
		public Color BackExpression            { get => txtExpression.BackColor; set => txtExpression.BackColor = value; }
		public Color ForeResultPast            { get; set; } = Color.LightGray;
		public Color ForeResultHexPast         { get; set; } = Color.LightGray;
		public Color ForeExpressionDeactivated { get; set; } = Color.White;
		public Color BackExpressionDeactivated { get; set; } = Color.Black;

		// フォント
		public Font ResultFont     { get => lbResult.Font;      set => lbResult.Font = value; }
		public Font ResultHexFont  { get => lbResultHex.Font;   set => lbResultHex.Font = value; }
		public Font ExpressionFont { get => txtExpression.Font; set => txtExpression.Font = value; }

		public Control[] CursorFixer { get; private set; }
		
		// その他
		public int MaxHistory { get => Hist.Max; set => Hist.Max = value; }

		#endregion

		////////////////////////////////////////////////////////////////
		#region イベント関連
		////////////////////////////////////////////////////////////////

		public event RequestEventHandler RequestCalculate = null!;
		public event RequestLogHandler   RequestLog = null!;
		public event RequestInitializeHandler RequestInitialize = null!;
		
		private void Log( LogLevel l, string msg ) => RequestLog?.Invoke( l, msg );
		private void Log( string msg ) => RequestLog?.Invoke( LogLevel.Debug, msg );
		private void LogV( string msg ) => RequestLog?.Invoke( LogLevel.Verbose, msg );
		
		#endregion

		////////////////////////////////////////////////////////////////
		#region 初期化関係
		////////////////////////////////////////////////////////////////

		public CalcForm()
		{
			InitializeComponent();

			CursorFixer =
			[
				txtExpression,
			];
		}

		private void CalcForm_Load(object sender, EventArgs e)
		{
			//// 一度計算処理を走らせて、キャッシュする
			TmpCalc("ERROR"); // 例外が投げられる場合
			TmpCalc("0");     // 通常の計算 ... (#ans は 0 となる必要あり)

			//// 全体的な初期化処理
			lbResult.Text = "-";
			lbResultHex.Text = "-";
			txtExpression.Text = "";
			Hist.Clear();

			return;

			////////////////////////////////////////////////////////////
			// Loacl functions

			void TmpCalc(string s)
			{
				Log( $"Calculate to cache : {s}" );
				txtExpression.Text = s;
				Calc(true);
			}
		}

		#endregion

		////////////////////////////////////////////////////////////////
		#region UI関係のイベント：電卓本体
		////////////////////////////////////////////////////////////////

		private void txtExpression_KeyPress(object sender, KeyPressEventArgs e)
		{
			var self = (TextBox)sender;
			// if( e.KeyChar is ( >= 0x20 and <= 0x7F ) or '\b' )
			if (((e.KeyChar >= 0x20) && (e.KeyChar <= 0x7F)) || (e.KeyChar == '\b'))
			{
				// Legal
				if (self.SelectionLength > 0)
				{
					if ((e.KeyChar == '(') || (e.KeyChar == ')'))
					{
						string s = self.Text;
						ErasedExpression = s;
						int selTop = self.SelectionStart;
						int selEnd = self.SelectionStart + self.SelectionLength;
						s = s.Insert(selEnd, ")");
						s = s.Insert(selTop, "(");
						self.Text = s;

						if (e.KeyChar == '(')
						{
							self.SelectionStart = selTop + 1;
						}
						if (e.KeyChar == ')')
						{
							self.SelectionStart = selEnd + 2;
						}

						e.Handled = true;
					}
				}

				// 最初の入力が演算子ならば、それは #ans を対象にしたものと見做す
				if (self.Text.Length == 0)
				{
					if (IsOperator(e.KeyChar))
					{
						self.Text = "#ans ";
						self.SelectionStart = self.Text.Length;
					}
				}
			}
			else
			{
				e.Handled = true;
			}

			return;

			//////////////////////////////////////////////////////

			bool IsOperator(char c)
			{
				bool ret = false;
				// if( c is '-' or '*' or '/' or '%' or '&' or '|' or '^' or '>' or '<' or '!' )
				if ((c == '+') ||
					(c == '-') ||
					(c == '*') ||
					(c == '/') ||
					(c == '%') ||
					(c == '&') ||
					(c == '|') ||
					(c == '^') ||
					(c == '>') ||
					(c == '<') ||
					(c == '!'))
				{
					ret = true;
				}
				return ret;
			}
		}

		private void CalcForm_KeyDown(object sender, KeyEventArgs e)
		{
		}

		private void txtExpression_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.Modifiers & Keys.Alt) == Keys.Alt)
			{
				ProcessKeyDown_Alt(e);
			}

			if ((e.Modifiers & Keys.Control) == Keys.Control)
			{
				ProcessKeyDown_Control(e);
			}

			if (e.KeyCode == Keys.Enter)
			{
				ProcessKeyDown_Enter(e);
			}

			var cas = (Keys.Alt | Keys.Control | Keys.Shift);
			if ((e.Modifiers & cas) == 0)
			{
				ProcessKeyDown_Neutral(e);
			}
		}

		private void CalcForm_KeyPress(object sender, KeyPressEventArgs e)
		{

		}

		private void ProcessKeyDown_Control(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.V)
			{
				var cas = (Keys.Alt | Keys.Control | Keys.Shift);
				if ((e.Modifiers & cas) == cas)
				{
					ShowVersion();
				}
				else
				{
					txtExpression.Paste();
				}
			}

			if (e.KeyCode == Keys.C)
			{
				txtExpression.Copy();
			}

			if (e.KeyCode == Keys.Z)
			{
				if (txtExpression.CanUndo)
				{
					txtExpression.Undo();
				}
				else
				{
					if (ErasedExpression != "")
					{
						txtExpression.Text = ErasedExpression;
						txtExpression.SelectionStart = txtExpression.Text.Length;
					}
				}
			}

			if (e.KeyCode == Keys.Up)
			{
				string s = Hist.CallUp(txtExpression.Text);
				txtExpression.Text = s;
				Calc(true);
				txtExpression.Select(s.Length, 0);
			}

			if (e.KeyCode == Keys.Down)
			{
				string s = Hist.CallDown(txtExpression.Text);
				txtExpression.Text = s;
				Calc(true);
				txtExpression.Select(s.Length, 0);
			}
		}

		private void ProcessKeyDown_Alt(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.A)
			{
				txtExpression.SelectedText = "#ans ";
			}

			if (e.KeyCode == Keys.C)
			{
				if ((e.Modifiers & Keys.Shift) == Keys.Shift)
				{
					CopyHex();
				}
				else
				{
					Copy();
				}
			}
		}

		private void ProcessKeyDown_Enter(KeyEventArgs e)
		{
			Log( $"Enter - {txtExpression.Text}" );
			
			ErasedExpression = "";
			if (txtExpression.Text == "initialize")
			{
				Hist.Clear();
				txtExpression.Text = "";
				lbResult.Text = "-";
				lbResultHex.Text = "-";
				RequestInitialize?.Invoke();
			}
			else if( txtExpression.Text == "" )
			{
				LogV( "Empty expression" );
			}
			else
			{
				Calc();
			}
		}

		private void Calc(bool temporal = false)
		{
			LogV( $"Calc {temporal}" );
			
			if (txtExpression.Text.Trim() != "")
			{
				double result = 0;
				try
				{
					result = ParseAndCalc();

					#if false
						
						string tmp;
						tmp = result.ToString("N0");
						// double result_up = result % 1;
						double result_up = result - (ulong)result;
						if( result_up != 0 )
						{
							tmp += ( "." + result_up.ToString().Split('.')[1] );
						}
						lbResult.Text = tmp;
						
						#elif false
						
						lbResult.Text = result.ToString();
						
					#else
						
						if ((result == Double.PositiveInfinity) ||
							(result == Double.NegativeInfinity))
						{
							lbResult.Text = $"Div/0 ({result})";
						}
						else
						{
							result = Math.Round(result,15);
							lbResult.Text = SetSeparator(result);
						}
						
					#endif
					
					lbResultHex.Text = GetHexString(result);
				}
				catch (ApplicationException ex)
				{
					lbResult.Text = ex.Message;
					lbResultHex.Text = "";
					Log( LogLevel.Warning, $"Error {ex.Message}" );
				}
				catch (DivideByZeroException)
				{
					lbResult.Text = "Div/0";
					lbResultHex.Text = "";
					Log( LogLevel.Warning, "Error Div/0" );
				}
				catch (OverflowException)
				{
					lbResult.Text = "Overflow";
					lbResultHex.Text = "";
					Log( LogLevel.Warning, "Error Overflow" );
				}
				catch (Exception ex)
				{
					lbResult.Text = "ERROR";
					lbResultHex.Text = "";
					Log( LogLevel.Error, $"Error {ex.Message}" );
				}

				if (temporal == false)
				{
					Hist.Add(txtExpression.Text);
					txtExpression.Text = "";
				}
				lbResult.ForeColor = ForeResult;
				lbResultHex.ForeColor = ForeResultHex;
			}
		}

		private string GetHexString(double result)
		{
			string ret;
			try
			{
				string tmpHex = (result >= 0) ? ((ulong)result).ToString("X") : ((long)result).ToString("X");

				int defLength = tmpHex.Length;
				for (int i = 0; i < (defLength - 1) / 4; i++)
				{
					tmpHex = tmpHex.Insert(tmpHex.Length - i * 5 - 4, "_");
				}

				ret = "0x" + tmpHex;
			}
			catch
			{
				ret = "-"; // 16進数変換エラーは無視
			}
			return ret;
		}

		private string SetSeparator(double val)
		{
			string ret = "";
			string str = val.ToString();
			var tmp2 = str.Split('E');

			if (tmp2.Length >= 2)
			{
				int eVal = Int32.Parse(tmp2[1]);
				int mod3 = eVal % 3;
				if (mod3 != 0)
				{
					var rVal = double.Parse(tmp2[0]);
					eVal -= mod3;
					tmp2[0] = (rVal * Math.Pow(10, mod3)).ToString();
					tmp2[1] = (eVal >= 0) ? ("+" + eVal.ToString()) : eVal.ToString();

					// Console.WriteLine( $"E {tmp2[0]} , {tmp2[1]}" );
				}
			}

			var tmp = tmp2[0].Split('.');
			double intVal = double.Parse(tmp[0]);

			if ((intVal == 0) && (val < 0))
			{
				ret = $"-{intVal:N0}";
			}
			else
			{
				ret = $"{intVal:N0}";
			}

			if (tmp.Length >= 2)
			{
				string udTmp = tmp[1].TrimEnd('0');
				string ud = "";
				for (int i = 0; i < udTmp.Length; i++)
				{
					if ((i != 0) && (i % 3 == 0))
					{
						ud += " ";
					}
					ud += udTmp[i];
				}

				if (ud != "")
				{
					ret += ".";
					ret += ud;
				}
			}

			if (tmp2.Length >= 2)
			{
				ret += " E" + tmp2[1];
			}

			return ret;
		}

		private void ProcessKeyDown_Neutral(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Up)
			{
				txtExpression.Text = Hist.CallUp(txtExpression.Text);
				txtExpression.Select(txtExpression.Text.Length, 0);
			}

			if (e.KeyCode == Keys.Down)
			{
				txtExpression.Text = Hist.CallDown(txtExpression.Text);
				txtExpression.Select(txtExpression.Text.Length, 0);
			}

			if (e.KeyCode == Keys.Escape)
			{
				if (txtExpression.Text == "")
				{
					lbResult.Text = "-";
					lbResultHex.Text = "-";
					Hist.Reset();
				}
				else
				{
					ErasedExpression = txtExpression.Text;
				}
				txtExpression.Text = "";
			}
		}

		private double ParseAndCalc() => RequestCalculate?.Invoke(txtExpression.Text.Trim()) ?? Double.NaN;

		private void Copy()
		{
			if ((lbResult.Text != "") && (lbResult.Text != "-"))
			{
				try
				{
					var tmp = lbResult.Text.Split(',');
					string str = "";
					Array.ForEach(tmp, s => str += s);

					var tmp2 = str.Split(' ');
					string str2 = "";
					Array.ForEach(tmp2, s => str2 += s);

					// Clipboard.SetText( lbResult.Text );
					Clipboard.SetText(str2);
					tmrInterval.Enabled = true;
					lbResult.BackColor = Color.FromArgb(
						(byte)~lbResult.BackColor.R,
						(byte)~lbResult.BackColor.G,
						(byte)~lbResult.BackColor.B);
				}
				catch (System.Runtime.InteropServices.ExternalException ex)
				{
					MessageBox.Show(ex.Message, "Error");
				}
				catch (System.Exception ex)
				{
					MessageBox.Show(ex.GetType() + "\n" + ex.Message, "Error");
				}
			}
		}

		private void CopyHex()
		{
			if ((lbResultHex.Text != "") && (lbResultHex.Text != "-"))
			{
				try
				{
					var tmp = lbResultHex.Text.Split('_');
					string str = "";
					Array.ForEach(tmp, s => str += s);

					// Clipboard.SetText( lbResultHex.Text );
					Clipboard.SetText(str);
					tmrInterval.Enabled = true;
					lbResultHex.BackColor = Color.FromArgb(
						(byte)~lbResultHex.BackColor.R,
						(byte)~lbResultHex.BackColor.G,
						(byte)~lbResultHex.BackColor.B);
				}
				catch (System.Runtime.InteropServices.ExternalException ex)
				{
					MessageBox.Show(ex.Message, "Error");
				}
				catch (System.Exception ex)
				{
					MessageBox.Show(ex.GetType() + "\n" + ex.Message, "Error");
				}
			}
		}

		private void Paste()
		{
			if (Clipboard.ContainsText())
			{
				string tmp = Clipboard.GetText();
				if (String.IsNullOrEmpty(tmp) == false)
				{
					txtExpression.SelectedText = tmp;
				}
			}
		}

		private void ShowVersion()
		{
			var asm = System.Reflection.Assembly.GetExecutingAssembly();
			var ver = asm.GetName().Version;
			lbResult.Text = "Karculator in Jittoq";
			lbResultHex.Text = "Ver." + ver?.ToString() ?? "??";
			lbResult.ForeColor = ForeResult;
		}

		#endregion

		////////////////////////////////////////////////////////////////
		#region UI関係のイベント：補助
		////////////////////////////////////////////////////////////////

		private void txtExpression_TextChanged(object sender, EventArgs e)
		{
			lbResult.ForeColor = ForeResultPast;
			lbResultHex.ForeColor = ForeResultHexPast;
		}

		private void tmrInterval_Tick(object sender, EventArgs e)
		{
			lbResult.BackColor = BackResult;
			lbResultHex.BackColor = BackResultHex;
			tmrInterval.Enabled = false;
		}

		private void lbResult_Paint(object sender, PaintEventArgs e)
		{
			DrawLabel((Label)sender, e);
		}

		private void DrawLabel(Label target, PaintEventArgs e)
		{
			var g = e.Graphics;
			int w = target.Width;
			int h = target.Height;
			var rect = new Rectangle(0, 0, w, h);

			// 背景を描画
			using (var b = new SolidBrush(target.BackColor))
			{
				g.FillRectangle(b, rect);
			}

			if (string.IsNullOrEmpty(target.Text) == false)
			{
				// 測定
				var curSize = g.MeasureString(target.Text, target.Font);

				Font newFont;
				float sizeRatio = 1.0f;
				if (curSize.Width > target.Width)
				{
					// 収まらない場合
					sizeRatio = target.Width / curSize.Width;
					newFont = new Font(target.Font.Name, (int)(target.Font.Size * sizeRatio), target.Font.Style);
				}
				else
				{
					// 収まる場合はそのまま描画
					newFont = target.Font;
				}

				// 表示位置は固定
				var sf = new StringFormat();
				sf.Alignment = StringAlignment.Far;
				sf.LineAlignment = StringAlignment.Center;

				using (var b = new SolidBrush(target.ForeColor))
				{
					g.DrawString(target.Text, newFont, b, rect, sf);
				}
			}
		}

		#endregion

		////////////////////////////////////////////////////////////////
		#region インターフェースの実装
		////////////////////////////////////////////////////////////////

		public void Activate()
		{
			LogV( "Activate" );
			SetFocus();
		}

		public void Deactivate()
		{
			LogV( "Deactivate" );
			this.Hide();
		}

		#endregion

		private void CalcForm_Enter(object sender, EventArgs e)
		{
			LogV( "Enter" );
			SetFocus();
		}
		
		private void SetFocus()
		{
			this.BeginInvoke( () => {
				txtExpression.Focus();
			} );
		}
	}

	public delegate double RequestEventHandler( string expression );
	public delegate void   RequestLogHandler( LogLevel lv, string msg );
	public delegate void   RequestInitializeHandler();
}
