using System;
using System.Diagnostics;

using QuickCalc.Parser.Parser;
using QuLib;


namespace QuickCalc
{
	public delegate void QuickCalcEventHandler( QuickCalc sender );
	
	public class QuickCalc
	{
		private readonly ParserParser Parser = new();
		
		public QuickCalc()
		{
		}
		
		public void Initialize()
		{
			Parser.InitVariables();
		}

		public double Calculate( string expression )
		{
			double ret = Double.NaN;
			if( Parser.Parse( expression ) )
			{
				ret = Parser.Answer;
			}
			return ret;
		}
	}
}
