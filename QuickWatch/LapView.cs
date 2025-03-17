using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickWatch
{
	public partial class LapView : Form
	{
		public LapView()
		{
			InitializeComponent();
		}
		
		public void AddLap( TimeSpan lap, TimeSpan spl )
		{
			AddLap( lstLap.Items.Count + 1, lap, spl );
		}
		
		public void AddLap( int no, TimeSpan lap, TimeSpan spl )
		{
			var tmp = new string[3];
			
			tmp[0] = no.ToString();
			tmp[1] = lap.ToString( @"hh\:mm\:ss\.ff" );
			tmp[2] = spl.ToString( @"hh\:mm\:ss\.ff" );
			
			lstLap.Items.Add( new ListViewItem( tmp ) );
		}
		
		public void Clear()
		{
			lstLap.Items.Clear();
		}
		
		private void LapView_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}
		
		private void btnCopy_Click(object sender, EventArgs e)
		{
			var sb = new StringBuilder();
			
			sb.Append( "No\tLAP\tSPLIT\n" );
			
			foreach( ListViewItem tg in lstLap.Items )
			{
				sb.Append( tg.SubItems[0].Text );
				sb.Append( "\t" );
				sb.Append( tg.SubItems[1].Text );
				sb.Append( "\t" );
				sb.Append( tg.SubItems[2].Text );
				sb.Append( "\n" );
			}
			
			try
			{
				Clipboard.SetDataObject( sb.ToString() );
			}
			catch( System.Runtime.InteropServices.ExternalException ex )
			{
				MessageBox.Show( ex.Message, "エラー" );
			}
			catch( System.Exception ex )
			{
				MessageBox.Show( ex.GetType() + "\n" + ex.Message, "エラー" );
			}
		}
		
		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Hide();
		}
	}
}
