using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace QuLib
{
	/***************************************************************
	
		Class Name :    ExtendedMethods
		Extends    :    None
		Interfaces :    None
		
		Purpose
			拡張メソッド用
	
	***************************************************************/
	public static class ExtendedMethods
	{
		public static bool IsIn<T>( this T self, T min, T max, Range rangeMode = Range.Inclusive ) where T : IComparable
		{
			bool ret = false;
			
			if( rangeMode == Range.Exclusive )
			{
				if( ( self.CompareTo( min ) > 0 ) && ( self.CompareTo( max ) < 0 ) )
				{
					ret = true;
				}
			}
			else
			{
				if( ( self.CompareTo( min ) >= 0 ) && ( self.CompareTo( max ) <= 0 ) )
				{
					ret = true;
				}
			}
			
			return ret;
		}
		
		
		public static bool Not( this bool c ) => !c;
		public static bool Invert(this bool c) => !c;
		public static bool IsFalse(this bool c) => !c;

		public static bool ToBool(this string s) => (s.ToLower() is "true" or "1" );
		public static int ToInt(this string s) => Int32.Parse(s);

		// ラムダ式でInvokeするための拡張メソッド
		// 通常通りにInvokeを記述し、ラムダ式で書いた場合にこちらが呼び出される
		// ついでに、Invokeが必要ない場合は通常呼び出しする。
		// (例)  foo.Invoke( () => foo.Text = "Invoked!" );
		public static void Invoke(this Control c, Action act)
		{
			if (c.IsHandleCreated)
			{
				if (c.InvokeRequired)
				{
					c.Invoke((MethodInvoker)(() => act()));
				}
				else
				{
					act();
				}
			}
		}
	}

	public enum Range
	{
		Inclusive,
		Exclusive,
	}
}
