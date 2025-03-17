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
using YacsLibrary;
using System.Windows.Forms;

namespace WinQu
{
	public class Inifile
	{
		////////////////////////////////////////////////////////////////
		#region 定数
		////////////////////////////////////////////////////////////////
		
		#endregion
		
		////////////////////////////////////////////////////////////////
		#region WindowsAPIのためのDLL
		////////////////////////////////////////////////////////////////
		
		[DllImport("KERNEL32.DLL")]
		public static extern uint WritePrivateProfileString(
			string lpAppName,
			string lpKeyName,
			string lpString,
			string lpFileName );
		
		[DllImport("KERNEL32.DLL")]
		public static extern uint GetPrivateProfileString(
			string lpAppName,
			string? lpKeyName,
			string? lpDefault,
			StringBuilder lpReturnedString,
			uint nSize,
			string lpFileName );

		[DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringA")]
		public static extern uint GetPrivateProfileStringByByteArray(
			string lpAppName,
			string? lpKeyName,
			string? lpDefault,
			byte[] lpReturnedString,
			uint nSize,
			string lpFileName);

		#endregion

		////////////////////////////////////////////////////////////////
		#region プロパティ
		////////////////////////////////////////////////////////////////

		public string FileName { get; set; }
		public int    WorkSize { get; set; }
		
		public bool Exists {
			get {
				return System.IO.File.Exists( FileName );
			}
		}
		
		#endregion
		
		////////////////////////////////////////////////////////////////
		#region コンストラクタ
		////////////////////////////////////////////////////////////////
		
		public Inifile( string fn )
		{
			FileName = fn;
			WorkSize = 10240;
		}
		
		public Inifile() : this( "" ) // : this( ... ) で、自身のオーバーロードを呼び出せる
		{
		}
		
		#endregion
		
		////////////////////////////////////////////////////////////////
		#region 公開メソッド
		////////////////////////////////////////////////////////////////
		
		public void Write( string section, string key, object val )
		{
			// objectは、全ての型の基底クラス
			// .ToString はobjectクラスで定義されている
			// (このクラスも、実はこっそりobject派生になっている)
			// つまり、object型へはどんな型でも代入することが出来る。(値型でさえも！→boxing)
			WritePrivateProfileString( section, key, val?.ToString() ?? "", FileName );
			Log.N( $"{FileName} {section} {key} ... written {val}" );
		}
		
		// 面倒な制約があるので、一旦stringで受ける
		public string Read( string section, string key, string def )
		{
			const string tempDef = "\x15";
			StringBuilder sb = new StringBuilder( WorkSize );
			
			GetPrivateProfileString(
				section,
				key,
				tempDef,
				sb,
				Convert.ToUInt32( sb.Capacity ),
				FileName );
			
			string ret = sb.ToString();
			if( ret == tempDef )
			{
				ret = def;
				Log.W( $"{Path.GetFileName(FileName)} {section} {key} ... used default {def}" );
			}
			else
			{
				Log.N( $"{Path.GetFileName(FileName)} {section} {key} ... read {def}" );
			}
			
			// GetPrivateProfileString で読むと、コメントが除去されない
			// 値に ; が必要なケースは無いであろう
			var tmp = ret.Split( ';' );
			
			return tmp.Length > 0 ? tmp[0].Trim() : ret;
		}
		
		public void Init( string section, string key, string def )
		{
			string data = Read( section, key, def );
			Write( section, key, data );
		}
		
		public IEnumerable<string> ReadMulti( string section, string key, string def, char sepa )
		{
			string full = Read( section, key, def );
			var list = full.Split( sepa );
			
			foreach( var val in list )
			{
				yield return val;
			}
		}
		
		public void Exec( string section, string key, string def, char sepa, Action<string> act )
		{
			foreach( var val in ReadMulti( section, key, def, sepa ) )
			{
				act( val );
			}
		}
		
		public List<string> GetAllKeys( string section )
		{
			Log.N( $"GetAllKeys from {FileName} , {section}" );

			var result = new byte[WorkSize];

			var size = GetPrivateProfileStringByByteArray(
				section,
				null,
				null,
				result,
				(uint)result.Length,
				FileName);

			var ret = new List<string>();
			if (size > 0)
			{
				var keys = Encoding.Default.GetString(result, 0, (int)size - 1).Split('\0');
				ret.AddRange(keys);
				foreach (var k in keys)
				{
					Log.N($" {section} has \"{k}\"");
				}
			}
			else
			{
				Log.W($"{section} has no keys.");
			}
			
			return ret;
		}
		
		#endregion
	}
}
