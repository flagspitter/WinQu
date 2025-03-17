using System;
using System.Collections.Specialized;

namespace QuickBoard
{
	public interface IClipData
	{
		string Type { get; }
		string Summary { get; }
		void GetFromClipboard();
		void RestoreClipboard();
	}

	public class ClipTextData : IClipData
	{
		public string Type => "Text";
		public string Summary => Data ?? "";
		public string? Data;

		public void GetFromClipboard()
		{
			Data = Clipboard.GetText();
		}
		
		public void RestoreClipboard()
		{
			if( Data != null )
			{
				Clipboard.SetText( Data );
			}
		}
	}

	public class ClipImageData : IClipData
	{
		public string Type => "Image";
		public string Summary => "" ?? "Empty";
		public Image? Data;

		public void GetFromClipboard()
		{
			Data = Clipboard.GetImage();
		}
		
		public void RestoreClipboard()
		{
			if( Data != null )
			{
				Clipboard.SetImage( Data );
			}
		}
	}

	public class ClipFileData : IClipData
	{
		public string Type => "File";
		public string Summary
		{
			get
			{
				string ret = "";
				if( Data == null )
				{
					ret = "Empty";
				}
				else if( Data.Length == 0 )
				{
					ret = "Empty";
				}
				else if( Data.Length == 1 )
				{
					ret = Data[0].ToString();
				}
				else
				{
					ret = $"{Data.Length} files";
				}
				return ret;
			}
		}
		
		private string[]? Data;
		
		public void GetFromClipboard()
		{
			Data = Clipboard.GetFileDropList().Cast<string>().ToArray();
		}
		
		public void RestoreClipboard()
		{
			if( ( Data != null ) && ( Data.Length > 0 ) )
			{
				var fileDropList = new System.Collections.Specialized.StringCollection();
				fileDropList.AddRange( Data );
				Clipboard.SetFileDropList( fileDropList );
			}
		}
	}
}
