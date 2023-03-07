using System;
using System.Text;
using System.Diagnostics;

namespace SurfedAndFound.Shared.Types
{
	public class WebPage : WebInfo
	{
		#region Private Members

		private byte[] data;
		private Encoding encoding;
		private string characterSet;
		private string html;
		private string text;

		#endregion

		#region Constructors

		public WebPage(string url)
			: base(url)
		{
		}

		#endregion

		#region Public Members

		public byte[] Data
		{
			[DebuggerNonUserCode]
			get { return data; }
			[DebuggerNonUserCode]
			set { data = value; }
		}

		public Encoding Encoding
		{
			[DebuggerNonUserCode]
			get { return encoding; }
			[DebuggerNonUserCode]
			set { encoding = value; }
		}

		public string CharacterSet
		{
			[DebuggerNonUserCode]
			get { return characterSet; }
			[DebuggerNonUserCode]
			set { characterSet = value; }
		}

		public string Html
		{
			[DebuggerNonUserCode]
			get { return html; }
			[DebuggerNonUserCode]
			set { html = value; }
		}
		
		public string Text
		{
			[DebuggerNonUserCode]
			get { return text; }
			[DebuggerNonUserCode]
			set { text = value; }
		}

		#endregion
	}
}
