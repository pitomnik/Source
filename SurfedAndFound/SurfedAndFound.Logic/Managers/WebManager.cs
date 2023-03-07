using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.Shared.Types;
using SurfedAndFound.Shared.Tools;
using System.Net.Sockets;

namespace SurfedAndFound.Logic.Managers
{
	public class WebManager
	{
		#region Private Members

		private readonly int requestTimeout;
		private readonly long maxContentLength;
		private readonly string contentTypeFilter;
		private readonly RequestCachePolicy cachePolicy;
		private event EventHandler<WebArgs> searchCompleted;

		#endregion

		#region Constructors

		public WebManager()
		{
			requestTimeout =  AppConfig.Read<int>(AppConfig.Keys.RequestTimeout, 2 * 1000);
			maxContentLength = AppConfig.Read<long>(AppConfig.Keys.MaxContentLength, 1024 * 1024);
			contentTypeFilter = AppConfig.Read<string>(AppConfig.Keys.ContentTypeFilter, "text/html");

			cachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
		}

		#endregion

		#region Public Members

		public event EventHandler<WebArgs> SearchCompleted
		{
			[DebuggerNonUserCode]
			add { searchCompleted += value; }
			[DebuggerNonUserCode]
			remove { searchCompleted -= value; }
		}

		#endregion

		#region Public Methods

		public WebPage Search(string url)
		{
			LogMaster.Trace("Downloading '{0}' started.", url);

			WebPage page = DownloadPage(url);

			LogMaster.Trace("Downloading '{0}' finished.", url);

			LogMaster.Trace("Parsing '{0}' started.", url);

			ParsePage(page);

			LogMaster.Trace("Parsing '{0}' finished.", url);

			return page;
		}

		public void SearchAsync(string url)
		{
			ThreadPoolEx.QueueUserWorkItem(new WaitCallback(SearchAsync), url);
		}

		public void CancelAsync()
		{
			ThreadPoolEx.CancelAll(true);
		}

		#endregion

		#region Private Methods

		private void SearchAsync(object state)
		{
			string url = (string)state;
			WebPage page = new WebPage(url);
			Exception error = null;

			try
			{
				page = Search(url);
			}
			catch (Exception ex)
			{
				LogMaster.Instance.Error(String.Format("Failed searching '{0}'.", url), ex);
				error = ex;
			}

			OnSearchCompleted(page, error);
		}

		private WebPage DownloadPage(string url)
		{
			WebPage page = new WebPage(url);

			using (WebClientEx client = new WebClientEx())
			{
				client.UseDefaultCredentials = true;
				client.CachePolicy = cachePolicy;
				client.RequestTimeout = requestTimeout;
				client.MaxContentLength = maxContentLength;
				client.ContentTypeFilter = contentTypeFilter;

				try
				{
					page.Data = client.DownloadData(url);
					page.Encoding = client.Encoding;
					page.CharacterSet = client.CharacterSet;
				}
				catch (WebException ex)
				{
					if (ex.Status == WebExceptionStatus.RequestCanceled)
					{
						LogMaster.Trace("Stopped downloading '{0}'. {1}", url, ex.Message);
					}
					else
					{
						LogMaster.Instance.WarnFormat("Failed downloading '{0}'. {1}", url, ex.Message);
					}
				}
			}

			return page;
		}

		private void ParsePage(WebPage page)
		{
			try
			{
				HtmlTool.Parse(page);
			}
			catch (Exception ex)
			{
				if (ex is InvalidOperationException || ex is OverflowException)
				{
					LogMaster.Instance.WarnFormat("Failed parsing '{0}'. {1}", page.Url, ex.Message);
				}
				else
				{
					throw;
				}
			}
		}

		private void OnSearchCompleted(WebPage page, Exception error)
		{
			if (searchCompleted != null)
			{
				searchCompleted(this, new WebArgs(page, error));
			}
		}

		#endregion
	}
}
