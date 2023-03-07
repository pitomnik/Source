using System;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Collections.Generic;

namespace SurfedAndFound.Shared.Tools
{
	public class WebClientEx : WebClient
	{
		#region Private Members

		private int requestTimeout;
		private long maxContentLength;
		private string contentTypeFilter;
		private string characterSet;

		#endregion

		#region Constructors

		static WebClientEx()
		{
			ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateCertificate);
		}

		public WebClientEx()
			: base()
		{
			requestTimeout = -1;
			maxContentLength = long.MaxValue;
			contentTypeFilter = String.Empty;
		}

		#endregion

		#region Public Members

		public int RequestTimeout
		{
			[DebuggerNonUserCode]
			get { return requestTimeout; }
			[DebuggerNonUserCode]
			set { requestTimeout = value; }
		}

		public long MaxContentLength
		{
			[DebuggerNonUserCode]
			get { return maxContentLength; }
			[DebuggerNonUserCode]
			set { maxContentLength = value; }
		}

		public string ContentTypeFilter
		{
			[DebuggerNonUserCode]
			get { return contentTypeFilter; }
			[DebuggerNonUserCode]
			set { contentTypeFilter = value; }
		}

		public string CharacterSet
		{
			[DebuggerNonUserCode]
			get { return characterSet; }
		}

		#endregion

		#region Private Methods

		private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);

			request.Timeout = requestTimeout;

			characterSet = null;

			return request;
		}

		protected override WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse response = base.GetWebResponse(request);

			if (response.ContentLength > MaxContentLength ||
				!response.ContentType.Contains(ContentTypeFilter))
			{
				this.CancelAsync();
			}

			characterSet = (response as HttpWebResponse).CharacterSet;

			return response;
		}

		#endregion
	}
}
