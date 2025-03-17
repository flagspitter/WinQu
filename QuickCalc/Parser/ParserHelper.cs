using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCalc.Parser.Parser
{
	partial class ParserParser
	{
		#region 数学系の関数
		
		private class FuncElement
		{
			public string Name;
			public Func<double,double> Func;
			public double DefaultArg;
			
			public FuncElement( string n, Func<double,double> f, double d )
			{
				Name = n;
				Func = f;
				DefaultArg = d;
			}
		}
		
		private FuncElement[] FuncTable = new FuncElement[] {
			new FuncElement( "sin",  v => Math.Sin(v),   Double.NaN ),
			new FuncElement( "cos",  v => Math.Cos(v),   Double.NaN ),
			new FuncElement( "tan",  v => Math.Tan(v),   Double.NaN ),
			new FuncElement( "sqrt", v => Math.Sqrt(v),  Double.NaN ),
			new FuncElement( "exp",  v => Math.Exp(v),   1 ),
			new FuncElement( "log",  v => Math.Log10(v), Double.NaN ),
			new FuncElement( "ln",   v => Math.Log(v),   Double.NaN ),
			new FuncElement( "abs",  v => Math.Abs(v),   Double.NaN ),
			new FuncElement( "inv",  v => 1/v, Double.NaN ),
			new FuncElement( "int",  v => (double)(int)v, Double.NaN ),
		};
		
		public double CalcMathFunction( string name, double val )
		{
			string key = name.ToLower();
			return ( FuncTable.Where( t => t.Name == key )?.FirstOrDefault() )?.Func( val ) ?? 0.0;
		}
		
		public double CalcMathFunction( string name )
		{
			string key = name.ToLower();
			var func = FuncTable.Where( t => t.Name == key )?.FirstOrDefault();
			double ret;
			
			if( func != null )
			{
				double arg = func.DefaultArg;
				if( Double.IsNaN( arg ) )
				{
					throw new ExpressionException("Argument errpr");
				}
				ret = func.Func( arg );
			}
			else
			{
				throw new ExpressionException("Unknown function");
			}
			
			return ret;
		}
		
		public double CalcMathFunction2( string name, double val )
		{
			string key = name.ToLower().Substring( 1 );
			return ( FuncTable.Where( t => t.Name == key )?.FirstOrDefault() )?.Func( val ) ?? 0.0;
		}
		
		public double Factorial( double val )
		{
			double ret = 1;
			for( double i=2; i<=val; i++ )
			{
				ret *= i;
			}
			return ret;
		}
		
		public double Permutation( double n, double r )
		{
			double d;
			double ret = n;
			
			for( d=n-r+1; d<n; d++ )
			{
				ret *= d;
			}
			
			return ret;
		}
		public double Combination( double n, double r ) => Permutation( n, r ) / Factorial( r );
		
		#endregion
		
		#region 定数
		
		private class ConstElement
		{
			public string Name;
			public double Val;
			
			public ConstElement( string n, double v )
			{
				Name = n;
				Val = v;
			}
		}
		
		private ConstElement[] ConstTable = new ConstElement[] {
			new ConstElement( "pi",  Math.PI ),
			new ConstElement( "answer to life the universe and everything",  42 ),
		};
		
		public double GetConst( string name )
		{
			double ret;
			
			#if false
			if( name == "answer to life the universe and everything" )
			{
				ret = 42;
			}
			else
			#endif
			{
				string key = name.ToLower();
				ret = ( ConstTable.Where( t => t.Name == key )?.FirstOrDefault() )?.Val ?? 0;
			}
			return ret;
		}
		
		#endregion
		
		#region 変数
		
		private Dictionary<string,double> Variables = new Dictionary<string,double>();
		
		public void SetVariable( string str, double val )
		{
			string key = str.ToLower();
			Variables[key] = val;
			// Console.WriteLine( $"Set key : {key} = {val}" );
		}
		
		public double GetVariable( string str )
		{
			double ret;
			string key = str.ToLower();
			
			if( Variables.ContainsKey( key ) )
			{
				ret = Variables[key];
				// Console.WriteLine( $"found {key} : {ret}" );
			}
			else
			{
				ret = 0;
				// Console.WriteLine( $"unregisterd key : {key}" );
			}
			return ret;
		}
		
		public double Answer {
			set => SetVariable( "#ans", value );
			get => GetVariable( "#ans" );
		}
		
		public void InitVariables()
		{
			Variables.Clear();
		}
		
		#endregion
	}
}
