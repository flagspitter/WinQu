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

namespace QuickNote
{
	public partial class QuickNoteForm : UserControl
	{
		public QuickNoteForm()
		{
			InitializeComponent();
		}

		public void Clear()
		{
			NoteListView.Items.Clear();
		}

		public void Add(string key, string note)
		{
			bool found = false;
			foreach( ListViewItem item in NoteListView.Items )
			{
				if( item.Text == key )
				{
					found = true;
					item.SubItems[1].Text = note;
					break;
				}
			}

			if( found == false )
			{
				var item = new ListViewItem
				{
					Text = key
				};
				item.SubItems.Add(note);
				NoteListView.Items.Add(item);
			}
		}

		public void Remove( string key )
		{
			foreach( ListViewItem item in NoteListView.Items )
			{
				if( item.Text == key )
				{
					NoteListView.Items.Remove(item);
				}
			}
		}

		private void QuickNoteForm_KeyDown(object sender, KeyEventArgs e)
		{
			ProcessKey( e );
		}

		private void QuickNoteForm_Load(object sender, EventArgs e)
		{
			this.Focus();
			NoteListView.Focus();
		}

		private void QuickNoteForm_Enter(object sender, EventArgs e)
		{
			this.Focus();
			NoteListView.Focus();
		}

		private void NoteListView_KeyDown(object sender, KeyEventArgs e)
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

				foreach( ListViewItem item in NoteListView.SelectedItems )
				{
					NoteRemoved?.Invoke( item.Text );
				}
			}
			else
			{
				string key = DecodeKeyString(keyName);
				// Console.WriteLine($"key down {keyName} -> {key}");

				if (String.IsNullOrEmpty(key).Invert())
				{
					bool found = false;
					foreach (ListViewItem item in NoteListView.Items)
					{
						if (item.Text == key)
						{
							found = true;
							InputNote(key, item.SubItems[1].Text);
							break;
						}
					}

					if (found.Not())
					{
						InputNote(key);
					}
				}
			}
		}

		private void InputNote( string key, string note = "" )
		{
			using( var f = new InputForm() )
			{
				f.SetNote( key, note );
				f.ShowDialog();
				if( f.DialogResult == DialogResult.OK )
				{
					NoteUpdated?.Invoke( key, f.Note );
				}
			}
		}

		[DllImport("user32.dll")]
		private static extern int GetSystemMetrics(int nIndex);

		private const int SM_CXVSCROLL = 2;

		public void AdjustWidth( int windowWidth )
		{
			int verticalScrollBarWidth = GetSystemMetrics(SM_CXVSCROLL); 
			TextColumn.Width = windowWidth - KeyColumn.Width - verticalScrollBarWidth;
		}

		public event UpdateEventHandler NoteUpdated = null!;
		public event RemoveEventHandler NoteRemoved = null!;
		
		public delegate void UpdateEventHandler( string key, string note );
		public delegate void RemoveEventHandler(string key);
	}
}
