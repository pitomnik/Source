using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.Shared.Types;

using Sgml;

namespace SurfedAndFound.Shared.Tools
{
	public class HtmlTool
	{
		#region NodeNames Class

		private sealed class NodeNames
		{
			public const string Style = "style";
			public const string Script = "script";
			public const string Html = "html";
			public const string Title = "title";
			public const string Body = "body";
		}

		#endregion

		#region Constants

		private const string htmlDocType = "HTML";
		private const string defaultPrefix = "a";

		#endregion

		#region Private Members

		private static readonly Regex charsetRegex;

		#endregion

		#region Constructors

		static HtmlTool()
		{
			charsetRegex = new Regex("(?<=<meta http-equiv[^<]*charset=)[a-zA-Z0-9-]+(?=[^<]*>)",
				RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		#endregion

		#region Public Methods

		public static string NormalizeUrl(string url)
		{
			string normalUrl = url;

			if (!String.IsNullOrEmpty(url) && !url.Contains(":"))
			{
				normalUrl = String.Concat(Uri.UriSchemeHttp, Uri.SchemeDelimiter, url);
			}

			return normalUrl.TrimEnd('/');
		}

		public static void Parse(WebPage page)
		{
			if (page.Data == null || page.Data.Length == 0)
			{
				return;
			}

			string html = page.Encoding.GetString(page.Data);
			Match match = charsetRegex.Match(html);
			string charset = null;

			if (match != null)
			{
				charset = match.Value.ToLower();
			}

			Encoding encoding;

			if (!TryParseEncoding(charset, out encoding))
			{
				TryParseEncoding(page.CharacterSet, out encoding);
			}

			if (encoding == null)
			{
				page.Html = html;
			}
			else
			{
				page.Html = encoding.GetString(page.Data);
			}

			using (SgmlReader sgmlReader = new SgmlReader())
			{
				sgmlReader.DocType = htmlDocType;
				sgmlReader.CaseFolding = CaseFolding.ToLower;
				sgmlReader.WhitespaceHandling = WhitespaceHandling.None;

				using (TextReader textReader = new StringReader(page.Html))
				{
					sgmlReader.InputStream = textReader;

					XmlDocument doc = new XmlDocument();

					doc.PreserveWhitespace = false;
					doc.XmlResolver = null;
					doc.Load(sgmlReader);

					if (doc.DocumentElement != null)
					{
						XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);

						if (!String.IsNullOrEmpty(doc.DocumentElement.NamespaceURI))
						{
							nsmgr.AddNamespace(defaultPrefix, doc.DocumentElement.NamespaceURI);
						}

						RemoveNodes(doc.DocumentElement, NodeNames.Style, nsmgr);
						RemoveNodes(doc.DocumentElement, NodeNames.Script, nsmgr);

						page.Title = SelectTitle(doc.DocumentElement, nsmgr);
						page.Text = SelectText(doc.DocumentElement, nsmgr);
					}

					textReader.Close();
				}

				sgmlReader.Close();
			}
		}

		public static string Convert(string text, Encoding source, Encoding target)
		{
			byte[] data = Encoding.Convert(source, target, source.GetBytes(text));

			return target.GetString(data);
		}

		#endregion

		#region Private Methods

		private static bool TryParseEncoding(string s, out Encoding encoding)
		{
			encoding = null;
			
			if (!String.IsNullOrEmpty(s))
			{
				try
				{
					encoding = Encoding.GetEncoding(s);
				}
				catch
				{
					LogMaster.Instance.DebugFormat("Unknown encoding: '{0}'.", s);
				}
			}

			return encoding != null;
		}

		private static void RemoveNodes(XmlNode root, string name, XmlNamespaceManager nsmgr)
		{
			string path = BuildPath(name, nsmgr);

			foreach (XmlNode node in root.SelectNodes(path, nsmgr))
			{
				node.ParentNode.RemoveChild(node);
			}
		}

		private static string SelectTitle(XmlNode root, XmlNamespaceManager nsmgr)
		{
			string path = BuildPath(NodeNames.Title, nsmgr);
			XmlNode node = root.SelectSingleNode(path, nsmgr);

			return node == null ? null : TrimSpace(node.InnerText);
		}

		private static string SelectText(XmlNode root, XmlNamespaceManager nsmgr)
		{
			string path = BuildPath(NodeNames.Html, nsmgr);
			XmlNode html = root.SelectSingleNode(path, nsmgr);
			string text = null;

			if (html != null)
			{
				path = BuildPath(NodeNames.Body, nsmgr);

				XmlNode body = root.SelectSingleNode(path, nsmgr);

				if (body != null)
				{
					text = TrimSpace(body.InnerText);
				}
			}

			return text;
		}

		private static string BuildPath(string name, XmlNamespaceManager nsmgr)
		{
			if (nsmgr.HasNamespace(defaultPrefix))
			{
				name = String.Concat(defaultPrefix, ":", name);
			}

			return String.Concat("//", name);
		}

		private static string TrimSpace(string s)
		{
			return s.Trim('\r', '\n', '\t', ' ');
		}

		#endregion
	}
}
