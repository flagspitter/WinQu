using QuLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QuickBoard
{
	public partial class QuickBoardForm : UserControl
	{
		public QuickBoardForm()
		{
			InitializeComponent();
		}
		
		public string TitleText { get => Title.Text; set => Title.Text = value; }
		public Color TitleColor { get => Title.BackColor; set => Title.BackColor = value; }

		public void Clear()
		{
			ClipListView.Items.Clear();
		}

		public void Add(string key, string type, string clip)
		{
			bool found = false;
			foreach( ListViewItem item in ClipListView.Items )
			{
				if( item.Text == key )
				{
					found = true;
					item.SubItems[1].Text = type;
					item.SubItems[2].Text = clip;
					break;
				}
			}

			if( found == false )
			{
				var item = new ListViewItem
				{
					Text = key
				};
				item.SubItems.Add(type);
				item.SubItems.Add(clip);
				ClipListView.Items.Add(item);
			}
		}

		public void Remove( string key )
		{
			foreach( ListViewItem item in ClipListView.Items )
			{
				if( item.Text == key )
				{
					ClipListView.Items.Remove(item);
				}
			}
		}

		public void Unselect() => ClipListView.SelectedItems.Clear();
		public void Select( string key )
		{
			int idx = GetIndex( key );
			if( idx >= 0 )
			{
				ClipListView.Items[idx].Selected = true;
			}
			
			ClipListView.Invalidate();
			
			return;
			int GetIndex( string val )
			{
				for (int i = 0; i < ClipListView.Items.Count; i++)
				{
					if( ClipListView.Items[i].SubItems[0].Text == val )
					{
						return i; // インデックスを返す
					}
				}
				return -1; // 見つからない場合は -1 を返す
			}
		}

		private void QuickBoardForm_KeyDown(object sender, KeyEventArgs e)
		{
			ProcessKey( e );
		}

		private void QuickBoardForm_Load(object sender, EventArgs e)
		{
			this.Focus();
			ClipListView.Focus();
		}

		private void QuickBoardForm_Enter(object sender, EventArgs e)
		{
			this.Focus();
			ClipListView.Focus();
		}

		private void BoardListView_KeyDown(object sender, KeyEventArgs e)
		{
			ProcessKey( e );
		}
		
		private string DecodeKeyString( string s )
		{
			string ret;
			
			ret = ( s.Length, s ) switch
			{
				( 0, _ )         => "",
				( 1, _ )         => s,
				( _, "NumPad0" ) => "Num 0",
				( _, "NumPad1" ) => "Num 1",
				( _, "NumPad2" ) => "Num 2",
				( _, "NumPad3" ) => "Num 3",
				( _, "NumPad4" ) => "Num 4",
				( _, "NumPad5" ) => "Num 5",
				( _, "NumPad6" ) => "Num 6",
				( _, "NumPad7" ) => "Num 7",
				( _, "NumPad8" ) => "Num 8",
				( _, "NumPad9" ) => "Num 9",
				( _, "D0" )      => "0",
				( _, "D1" )      => "1",
				( _, "D2" )      => "2",
				( _, "D3" )      => "3",
				( _, "D4" )      => "4",
				( _, "D5" )      => "5",
				( _, "D6" )      => "6",
				( _, "D7" )      => "7",
				( _, "D8" )      => "8",
				( _, "D9" )      => "9",
				( _, "F1" )      => "F1",
				( _, "F2" )      => "F2",
				( _, "F3" )      => "F3",
				( _, "F4" )      => "F4",
				( _, "F5" )      => "F5",
				( _, "F6" )      => "F6",
				( _, "F7" )      => "F7",
				( _, "F8" )      => "F8",
				( _, "F9" )      => "F9",
				( _, "F10")      => "F10",
				( _, "F11")      => "F11",
				( _, "F12")      => "F12",
				_ => ""
			};
			
			return ret;
		}
		
		private void ProcessKey( KeyEventArgs e )
		{
			var keyName = Enum.GetName(typeof(Keys), e.KeyCode);

			if (String.IsNullOrEmpty(keyName))
			{
				; // Console.WriteLine($"empty key");
			}
			else if (e.KeyCode == Keys.Delete)
			{
				// Console.WriteLine("Delete");

				foreach( ListViewItem item in ClipListView.SelectedItems )
				{
					RequestRemove?.Invoke( item.Text );
				}
			}
			else
			{
				string key = DecodeKeyString(keyName);
				// Console.WriteLine($"key down {keyName} -> {key}");

				if( String.IsNullOrEmpty(key).Invert() )
				{
					RequestOperation( key );
				}
			}
		}

		[DllImport("user32.dll")]
		private static extern int GetSystemMetrics(int nIndex);

		private const int SM_CXVSCROLL = 2;

		public void AdjustWidth( int windowWidth )
		{
			int verticalScrollBarWidth = GetSystemMetrics(SM_CXVSCROLL); 
			TextColumn.Width = windowWidth - KeyColumn.Width - TypeColumn.Width - verticalScrollBarWidth;
		}

		public event RequestOperationHandler RequestRemove = null!;
		public event RequestOperationHandler RequestOperation = null!;
		
		public delegate void RequestOperationHandler( string key );
	}
}
