using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuLib;

namespace WinQu
{
	public class FormForceResizer : IDisposable
	{
		private Form TargetForm;
		private bool Enabled;
		
		public int MinWinWidth  { get; set; } = 0;
		public int MinWinHeight { get; set; } = 0;
		public int ResizingPullSize { get; set; } = 8;
		public bool EnableVertical   { get; set; } = true;
		public bool EnableHorizontal { get; set; } = true;
		
		// public List<Control> Excluded { get; } = new();
		public List<Control> Excluded { get; } = new List<Control>();
		
		public FormForceResizer( Form t ) : this( t, 0, 0 ) {}
		public FormForceResizer( Form t, int minW, int minH )
		{
			TargetForm = t;
			
			MinWinWidth = minW;
			MinWinHeight = minH;
			
			// 配下のコントロールに、ウィンドウ移動用のイベント登録
			SetMouseEvent( t.Controls );
			
			Enable();
		}
		
		private void SetMouseEvent( System.Windows.Forms.Control.ControlCollection c )
		{
			foreach( var child in c )
			{
				if( !( child is Button ) && ( child is Control ) )
				{
					Control tg = (Control)child;
					tg.MouseDown += new MouseEventHandler( Callback_MouseDown );
					tg.MouseMove += new MouseEventHandler( Callback_MouseMove );
				}
				
				// 配下にコンテナ系のコントロールがあるなら、再帰してそちらの子コントロールに登録
				if( child is ScrollableControl )
				{
					Control tg = (ScrollableControl)child;
					SetMouseEvent( tg.Controls );
				}
				
				// GroupBox でも同様
				if( child is GroupBox )
				{
					Control tg = (GroupBox)child;
					SetMouseEvent( tg.Controls );
				}
			}
		}
		
		[Flags]
		private enum ResizePosition
		{
			None   = 0,
			Left   = 1,
			Top    = 2,
			Right  = 4,
			Bottom = 8,
		}
		
		// ウィンドウ移動時のベース座標
		private Point ClkBaseSize;
		private Point WinBasePos;
		private Size  WinBaseSize;
		private ResizePosition Resizing;
		private bool AbortResizing;
		
		/***************************************************************
			Name        MainWindow_MouseDown
			Purpose     ウィンドウ移動開始
		***************************************************************/
		private void Callback_MouseDown(object? sender, MouseEventArgs e)
		{
			TargetForm.Activate();
			if( (e.Button & MouseButtons.Left) == MouseButtons.Left )
			{
				//位置を記憶する
				ClkBaseSize = new Point(e.X, e.Y);
				WinBasePos = new Point( TargetForm.Left, TargetForm.Top );
				WinBaseSize = new Size( TargetForm.Width, TargetForm.Height );
				AbortResizing = false;
			}
		}
		
		/***************************************************************
			Name        MainWindow_MouseMove
			Purpose     ドラッグによりウィンドウ移動
		***************************************************************/
		private void Callback_MouseMove(object? sender, MouseEventArgs e)
		{
			if( (e.Button & MouseButtons.Left) == MouseButtons.Left )
			{
				if( AbortResizing == false )
				{
					if( Resizing == ResizePosition.None )
					{
						TargetForm.Left += e.X - ClkBaseSize.X;
						TargetForm.Top  += e.Y - ClkBaseSize.Y;
					}
					else
					{
						SetNextSize( e );
					}
				}
			}
			else
			{
				SetResizingCursor();
			}
		}
		
		private void SetNextSize( MouseEventArgs e )
		{
			int nx = e.X;
			int ny = e.Y;
			int nextWidth  = TargetForm.Width;
			int nextHeight = TargetForm.Height;
			int nextLeft = TargetForm.Left;
			int nextTop = TargetForm.Top;
			
			if( ( Resizing & ResizePosition.Left ) != 0 )
			{
				nextWidth += ( ClkBaseSize.X - nx );
				nextLeft -= ( ClkBaseSize.X - nx );
			}
			else if( ( Resizing & ResizePosition.Right ) != 0 )
			{
				nextWidth += ( nx - ClkBaseSize.X );
				ClkBaseSize.X += ( nx - ClkBaseSize.X );
			}
			else
			{
				; // Not needed to resize horizontal.
			}
			
			if( ( Resizing & ResizePosition.Top ) != 0 )
			{
				nextHeight += ( ClkBaseSize.Y - ny );
				nextTop -= ( ClkBaseSize.Y - ny );
			}
			else if( ( Resizing & ResizePosition.Bottom ) != 0 )
			{
				nextHeight += ( ny - ClkBaseSize.Y );
				ClkBaseSize.Y += ( ny - ClkBaseSize.Y );
			}
			else
			{
				; // Not needed to resize vertical.
			}
			
			//
			
			if( nextWidth >= MinWinWidth )
			{
				TargetForm.SetBounds( nextLeft, 0, nextWidth, 0, BoundsSpecified.X | BoundsSpecified.Width );
			}
			else
			{
				if( ( Resizing & ResizePosition.Left ) != 0 )
				{
					nextLeft = WinBasePos.X + ( WinBaseSize.Width - MinWinWidth );
					TargetForm.SetBounds( nextLeft, 0, MinWinWidth, 0, BoundsSpecified.X | BoundsSpecified.Width );
				}
				
				if( ( Resizing & ResizePosition.Right ) != 0 )
				{
					TargetForm.Width = MinWinWidth;
				}
				
				Resizing = 0;
				AbortResizing = true;
				TargetForm.Cursor = Cursors.Default;
			}
			
			if( nextHeight >= MinWinHeight )
			{
				TargetForm.SetBounds( 0, nextTop, 0, nextHeight, BoundsSpecified.Y | BoundsSpecified.Height );
			}
			else
			{
				if( ( Resizing & ResizePosition.Top ) != 0 )
				{
					nextTop = WinBasePos.Y + ( WinBaseSize.Height - MinWinHeight );
					TargetForm.SetBounds( 0, nextTop, 0, MinWinHeight, BoundsSpecified.Y | BoundsSpecified.Height );
				}
				
				if( ( Resizing & ResizePosition.Bottom ) != 0 )
				{
					TargetForm.Height = MinWinHeight;
				}
				
				Resizing = 0;
				AbortResizing = true;
				TargetForm.Cursor = Cursors.Default;
			}
		}
		
		private void SetResizingCursor()
		{
			var pos = TargetForm.PointToClient( Cursor.Position );
			int x = pos.X;
			int y = pos.Y;
			
			// Console.Write( $"cursor = {TargetForm.Cursor}");
			
			int pull_x1 = ResizingPullSize;
			int pull_y1 = ResizingPullSize;
			int pull_x2 = TargetForm.Width - ResizingPullSize;
			int pull_y2 = TargetForm.Height - ResizingPullSize;
			
			Resizing = 0;
			Resizing |= EnableHorizontal && ( x < pull_x1 ) ? ResizePosition.Left   : ResizePosition.None;
			Resizing |= EnableHorizontal && ( x > pull_x2 ) ? ResizePosition.Right  : ResizePosition.None;
			Resizing |= EnableVertical   && ( y < pull_y1 ) ? ResizePosition.Top    : ResizePosition.None;
			Resizing |= EnableVertical   && ( y > pull_y2 ) ? ResizePosition.Bottom : ResizePosition.None;
			
			var nextCursor = 
				IsResizingTarget( ResizePosition.Left  | ResizePosition.Top )    ? Cursors.SizeNWSE :
				IsResizingTarget( ResizePosition.Right | ResizePosition.Bottom ) ? Cursors.SizeNWSE :
				IsResizingTarget( ResizePosition.Right | ResizePosition.Top )    ? Cursors.SizeNESW :
				IsResizingTarget( ResizePosition.Left  | ResizePosition.Bottom ) ? Cursors.SizeNESW :
				IsResizingTarget( ResizePosition.Left )                          ? Cursors.SizeWE :
				IsResizingTarget( ResizePosition.Right )                         ? Cursors.SizeWE :
				IsResizingTarget( ResizePosition.Top )                           ? Cursors.SizeNS :
				IsResizingTarget( ResizePosition.Bottom )                        ? Cursors.SizeNS :
				                                                                   Cursors.Default;
			
			if( ( nextCursor == Cursors.Default ) && IsExcluded() )
			{
				; // don't change if cursor is in excluded controls.
			}
			else
			{
				TargetForm.Cursor = nextCursor;
				// Console.Write( $" : changed");
			}
			// Console.WriteLine( "" );
			
			return ;
			
			bool IsResizingTarget( ResizePosition p )
			{
				return ( ( Resizing & p ) == p ) ? true : false;
			}
		}
		
		private bool IsExcluded()
		{
			var pos = TargetForm.PointToClient( Cursor.Position );
			int x = pos.X;
			int y = pos.Y;
			
			var found = Excluded.FirstOrDefault( c => x.IsIn( c.Left, c.Right ) && y.IsIn( c.Top, c.Bottom ) );
			
			return ( found != null ) ? true : false;
		}
		
		private void Disable()
		{
			if( Enabled )
			{
				TargetForm.MouseDown -= Callback_MouseDown;
				TargetForm.MouseMove -= Callback_MouseMove;
				Enabled = false;
			}
		}
		
		private void Enable()
		{
			if( Enabled == false )
			{
				TargetForm.MouseDown += Callback_MouseDown;
				TargetForm.MouseMove += Callback_MouseMove;
				Enabled = true;
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // 重複する呼び出しを検出するには

		void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: マネージド状態を破棄します (マネージド オブジェクト)。
					Disable();
				}

				// TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
				// TODO: 大きなフィールドを null に設定します。

				disposedValue = true;
			}
		}

		// TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
		// ~FormForceResizer() {
		//   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
		//   Dispose(false);
		// }

		// このコードは、破棄可能なパターンを正しく実装できるように追加されました。
		public void Dispose()
		{
			// このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
			Dispose(true);
			// TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
			// GC.SuppressFinalize(this);
		}
		#endregion

	}
}
