using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickCalc
{
	public class History
	{
		private readonly List<string> Hist = [];
		private int Pos;

		public string Current { get; set; } = "";
		public int Max { get; set; } = 8;
		
		public string this[int i] => Hist[i];
		
		public void Clear()
		{
			Hist.Clear();
			Pos = 0;
		}
		
		public void Reset()
		{
			Pos = 0;
			Current = "";
		}
		
		public void Add( string s )
		{
			if( Hist.Contains( s ) )
			{
				Hist.Remove( s );
			}
			else
			{
				if( Hist.Count >= Max )
				{
					Hist.RemoveAt( 0 );
				}
			}
			Hist.Add( s );
			Reset();
		}
		
		public string CallUp( string cur )
		{
			string ret = cur;
			
			if( Pos == 0 )
			{
				Current = cur;
			}
			
			if( Pos < Hist.Count )
			{
				Pos++;
				ret = Hist[ Hist.Count - Pos ];
			}
			return ret;
		}
		
		public string CallDown( string cur )
		{
			string ret = cur;
			if( Pos > 0 )
			{
				Pos--;
				if( Pos == 0 )
				{
					ret = Current;
				}
				else
				{
					ret = Hist[ Hist.Count - Pos ];
				}
			}
			return ret;
		}
	}
}
