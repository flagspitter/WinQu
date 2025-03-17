/***********************************************************************

  SortableListView.cs
    
    Purpose  : 並べ替え可能なListView
               (ListView派生)
    
    Autour / Revision history
        2016/--/--  Watanabe / Initial Version

***********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;

namespace QuickNote
{
	public class SortableListView : ListView
	{
		private int LatestColumn = -1;
		private bool Reverse;
		
		public int SortTarget {
			get {
				return LatestColumn;
			}
			set {
				LatestColumn = value;
				ChangeSortTarget( value );
			}
		}
		
		private void ChangeSortTarget( int tg )
		{
			if( tg == LatestColumn )
			{
				Reverse = !Reverse;
			}
			else
			{
				Reverse = false;
			}
			
			LatestColumn = tg;
			
			if( tg >= 0 )
			{
				this.ListViewItemSorter = new ListViewItemComparer( tg, Reverse );
			}
			else
			{
				this.ListViewItemSorter = null;
			}
			
			this.Refresh();
		}
		
		protected override void OnColumnClick( ColumnClickEventArgs e )
		{
			base.OnColumnClick(e);
			ChangeSortTarget( e.Column );
		}
		
		protected override void OnDrawColumnHeader( DrawListViewColumnHeaderEventArgs e )
		{
			base.OnDrawColumnHeader( e );
			
			if( ( e == null ) || ( e.Header == null ) )
			{
				// Fatal
			}
			else if( e.ColumnIndex == LatestColumn )
			{
				using( StringFormat sf = new StringFormat() )
				{
					e.DrawBackground();
					
					Font headerFont = this.Font;
					int x_pos = e.Bounds.Location.X + 4;
					int y_pos = e.Bounds.Location.Y + 2;
					
					sf.Alignment = StringAlignment.Near;
					e.Graphics.DrawString( e.Header.Text, headerFont, Brushes.Black, new Point( x_pos, y_pos ), sf );
					
					using( Font triFont = new Font( this.Font.Name, this.Font.Size - 3 ) )
					{
						SizeF stringSize = e.Graphics.MeasureString( e.Header.Text, headerFont, 1000, sf );
						int xOfs = x_pos + 4;
						int x0 = (int)stringSize.Width + xOfs;
						int y0 = (int)stringSize.Height / 4 + 1;
						
						if( Reverse == false )
						{
							e.Graphics.DrawString( "▲", triFont, Brushes.Gray, new Point( x0, y0 ), sf );
						}
						else
						{
							e.Graphics.DrawString( "▼", triFont, Brushes.Gray, new Point( x0, y0 ), sf );
						}
					}
				}
			}
			else
			{
				e.DrawDefault = true;
			}
		}
		
		protected override void OnDrawItem( DrawListViewItemEventArgs e )
		{
			base.OnDrawItem( e );
			e.DrawDefault = true;
		}
		
		protected override void OnDrawSubItem( DrawListViewSubItemEventArgs e )
		{
			base.OnDrawSubItem( e );
			e.DrawDefault = true;
		}
		
		public SortableListView()
		{
			this.OwnerDraw = true;
		}
	}
	
	public class ListViewItemComparer : IComparer
	{
		private int Column;
		private bool Reverse;
		
		public ListViewItemComparer( int col, bool rev )
		{
			Column = col;
			Reverse = rev;
		}
		
		public int Compare( object? x, object? y )
		{
			int ret;

			if ( (x == null) || (y == null) )
			{
				ret = ( x == y ) ? 1 : 0;
			}
			else
			{
				var tmpX = (ListViewItem)x;
				var tmpY = (ListViewItem)y;

				if (Reverse == false)
				{
					ret = string.Compare(tmpX.SubItems[Column].Text, tmpY.SubItems[Column].Text);
				}
				else
				{
					ret = string.Compare(tmpY.SubItems[Column].Text, tmpX.SubItems[Column].Text);
				}
			}
			
			return ret;
		}
	}
}
