using System;
using System.Collections.Generic;
using System.Text;

namespace QuickCalc.Parser.Parser
{
    internal partial class ParserScanner
    {
		private struct SuffixDef
		{
			public char Suffix;
			public char Suffix2;
			public int  Length;
			public double Mult;
			
			public SuffixDef( char c, double m )
			{
				Suffix = c;
				Suffix2 = '\0';
				Length = 1;
				Mult = m;
			}
			
			public SuffixDef( char c, char c2, double m )
			{
				Suffix = c;
				Suffix2 = c2;
				Length = 2;
				Mult = m;
			}
			
			public bool Match( char last2, char last )
			{
				bool ret = false;
				if( Length == 1 )
				{
					ret = ( Suffix == last );
				}
				else
				{
					ret = ( Suffix == last2 ) && ( Suffix2 == last );
				}
				return ret;
			}
		}
		
		private static SuffixDef[] SuffixTable = new SuffixDef[] {
			new SuffixDef( 'Q',1000000000000000000000000000000.0  ),
			new SuffixDef( 'R',   1000000000000000000000000000.0  ),
			new SuffixDef( 'Y',      1000000000000000000000000.0  ),
			new SuffixDef( 'Z',         1000000000000000000000.0  ),
			new SuffixDef( 'E',            1000000000000000000.0  ),
			new SuffixDef( 'P',               1000000000000000.0  ),
			new SuffixDef( 'T',                  1000000000000.0  ),
			new SuffixDef( 'G',                     1000000000.0  ),
			new SuffixDef( 'M',                        1000000.0  ),
			new SuffixDef( 'k',                           1000.0  ),
			new SuffixDef( 'K',                           1000.0  ),
			new SuffixDef( 'h',                            100.0  ),
			new SuffixDef( 'd', 'a',                        10.0  ),
			new SuffixDef( 'd',      0.1                        ),
			new SuffixDef( 'c',      0.01                       ),
			new SuffixDef( 'm',      0.001                      ),
			new SuffixDef( 'u',      0.000001                   ),
			new SuffixDef( 'n',      0.000000001                ),
			new SuffixDef( 'p',      0.000000000001             ),
			new SuffixDef( 'f',      0.000000000000001          ),
			new SuffixDef( 'a',      0.000000000000000001       ),
			new SuffixDef( 'z',      0.000000000000000000001    ),
			new SuffixDef( 'y',      0.000000000000000000000001 ),
			new SuffixDef( 'r',      0.000000000000000000000000001 ),
			new SuffixDef( 'q',      0.000000000000000000000000000001 ),
			
			new SuffixDef( 'K', 'i', 1024.0 ),
			new SuffixDef( 'M', 'i', 1024.0 * 1024 ),
			new SuffixDef( 'G', 'i', 1024.0 * 1024 * 1024 ),
			new SuffixDef( 'T', 'i', 1024.0 * 1024 * 1024 * 1024 ),
			new SuffixDef( 'P', 'i', 1024.0 * 1024 * 1024 * 1024 * 1024 ),
			new SuffixDef( 'E', 'i', 1024.0 * 1024 * 1024 * 1024 * 1024 * 1024 ),
			new SuffixDef( 'Z', 'i', 1024.0 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 ),
			new SuffixDef( 'Y', 'i', 1024.0 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 ),
		};
		
		private static SuffixDef DefaultSuffix = new SuffixDef( '\0', 1 );
		
		void GetNumber()
		{
			#if true
			var tmp = yytext.Split(',');
			Array.ForEach( tmp, s => yylval.s += s );
			#else
			yylval.s = yytext;
			#endif
			
			var last  = yytext[ yytext.Length - 1 ];
			if( ( last >= '0' ) && ( last <= '9' ) )
			{
				int epos;
				
				epos = yytext.IndexOf( 'E' );
				if( epos < 0 )
				{
					epos = yytext.IndexOf( 'e' );
				}
				
				if( epos > 0 )
				{
					double mult = double.Parse( yytext.Substring( epos + 1 ) );
					yylval.n = double.Parse( yytext.Substring( 0, epos ) ) * Math.Pow( 10, (double)mult );
				}
				else
				{
					yylval.n = double.Parse(yytext);
				}
			}
			else
			{
				var last2 = yytext[ yytext.Length - 2 ];
				var suffix = SuffixTable.Where( t => t.Match( last2, last ) )?.FirstOrDefault() ?? DefaultSuffix;
				var tmpStr = yytext.Substring( 0, yytext.Length - suffix.Length );
				var tmpResult = double.Parse(tmpStr);
				yylval.n = tmpResult * suffix.Mult;
			}
		}
		
		#if false
		void GetImgNumber()
		{
			string str;
			
			if( ( yytext[0] == 'i' ) || ( yytext[0] == 'j' ) )
			{
				str = yytext.Substring( 1 );
			}
			else
			{
				str = yytext.Substring( 0, yytext.Length - 1 );
			}
			
			yylval.n.Imaginary = double.Parse( str );
		}
		#endif
		
		void GetHexNumber()
		{
			#if true
			var tmp = yytext.Split('_');
			yylval.s = "";
			Array.ForEach( tmp, s => yylval.s += s );
			#else
			yylval.s = yytext;
			#endif
			yylval.n = Convert.ToInt64(yylval.s, 16);
		}
		
		void GetBinNumber()
		{
			#if true
			var tmp = yytext.Split('_');
			yylval.s = "";
			Array.ForEach( tmp, s => yylval.s += s );
			#else
			yylval.s = yytext;
			#endif
			yylval.n = Convert.ToInt64(yylval.s.Substring(2), 2);
		}
		
		void GetPi()
		{
			yylval.s = yytext;
			yylval.n = Math.PI;
		}
		
		void SetName()
		{
			yylval.s = yytext;
		}

		public override void yyerror(string format, params object[] args)
		{
			base.yyerror(format, args);
			Console.WriteLine(format, args);
			Console.WriteLine();
		}
    }
}
