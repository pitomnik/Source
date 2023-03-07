using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.Shared.Data;
using SurfedAndFound.Shared.Tools;
using SurfedAndFound.Shared.Types;
using SurfedAndFound.Logic.UrlContainer;

namespace SurfedAndFound.Logic.Managers
{
	public class SearchManager
	{
		#region Private Members

		private Cache cache;
		private List<string> cachedUrls;
		private WebManager webManager;
		private SearchOptions options;
		private string query;
		private WorkItem workItem;
		private int totalCount, currentCount;
		private event EventHandler<SearchArgs> searchPropress;
		private event EventHandler<ErrorArgs> searchError;

		#endregion

		#region Constructors

		public SearchManager()
		{
			cache = new Cache();
			webManager = new WebManager();

			webManager.SearchCompleted += new EventHandler<WebArgs>(webManager_SearchCompleted);
		}

		#endregion

		#region Public Members

		public SearchOptions Options
		{
			[DebuggerNonUserCode]
			get { return options; }
			[DebuggerNonUserCode]
			set { options = value; }
		}

		public event EventHandler<SearchArgs> SearchProgress
		{
			[DebuggerNonUserCode]
			add { searchPropress += value; }
			[DebuggerNonUserCode]
			remove { searchPropress -= value; }
		}

		public event EventHandler<ErrorArgs> SearchError
		{
			[DebuggerNonUserCode]
			add { searchError += value; }
			[DebuggerNonUserCode]
			remove { searchError -= value; }
		}

		#endregion

		#region Public Methods

		public long GetCacheCount()
		{
			return cache.GetCount();
		}

		public void Search(string query)
		{
			List<string> urls = new List<string>();
			Favorites favorites = new Favorites();
			History history = new History();
			Cookies cookies = new Cookies();
			Mru mru = new Mru();

			this.query = query;

			if (options.HasFlag(LookIn.Favorites))
			{
				DistinctJoin(urls, favorites.GetUrlList());
			}
			if (options.HasFlag(LookIn.History))
			{
				DistinctJoin(urls, history.GetUrlList());
			}
			if (options.HasFlag(LookIn.Recent))
			{
				DistinctJoin(urls, mru.GetUrlList());
			}
			if (options.HasFlag(LookIn.Cookies))
			{
				DistinctJoin(urls, cookies.GetUrlList());
			}

			totalCount = urls.Count;
			currentCount = 0;

			cachedUrls = cache.GetUrlList();

			Dictionary<string, string> foundUrls = cache.Search(query, options);
			List<string> oldUrls = new List<string>();

			foreach (string url in urls)
			{
				if (cachedUrls.Contains(url))
				{
					bool found = foundUrls.ContainsKey(url);
					string title = found ? foundUrls[url] : null;

					OnSearchProgress(url, title, found);
					
					continue;
				}

				Uri uri;

				if (Uri.TryCreate(url, UriKind.Absolute, out uri))
				{
					if (!uri.IsLoopback && uri.Scheme.StartsWith(Uri.UriSchemeHttp))
					{
						webManager.SearchAsync(url);
						
						continue;
					}
				}

				OnSearchProgress(url, null, false);
			}
		}

		public void SearchAsync(string query)
		{
			workItem = ThreadPoolEx.QueueUserWorkItem(new WaitCallback(SearchAsync), query);
		}

		public void CancelAsync()
		{
			ThreadPoolEx.Cancel(workItem, true);

			webManager.CancelAsync();
		}

		#endregion

		#region Private Methods

		private void DistinctJoin(List<string> target, List<string> source)
		{
			foreach (string url in source)
			{
				string normalUrl = HtmlTool.NormalizeUrl(url);

				if (!target.Contains(normalUrl))
				{
					target.Add(normalUrl);
				}
			}
		} 

		private void SearchAsync(object state)
		{
			try
			{
				Search((string)state);
			}
			catch (ThreadAbortException)
			{
				// do nothing
			}
			catch (Exception ex)
			{
				//TODO: Extract method.
				if (searchError != null)
				{
					searchError(this, new ErrorArgs(ex));
				}
			}
		}

		private void webManager_SearchCompleted(object sender, WebArgs e)
		{
			OnSearchCompleted(e.Page);
		}

		private void OnSearchCompleted(WebPage page)
		{
			if (page != null)
			{
				cache.Add(page);

				if (options.HasFlag(SearchIn.Text))
				{
					page.Found |= SearchEngine.Search(page.Text, query, false);
				}

				if (options.HasFlag(SearchIn.Title))
				{
					page.Found |= SearchEngine.Search(page.Title, query, false);
				}

				if (options.HasFlag(SearchIn.Address))
				{
					page.Found |= SearchEngine.Search(page.Url, query, false);
				}
			}

			OnSearchProgress(page.Url, page.Title, page.Found);
		}

		private void OnSearchProgress(string url, string title, bool found)
		{
			WebInfo info = new WebInfo(url);

			info.Title = title;
			info.Found = found;

			OnSearchProgress(info);
		}

		private void OnSearchProgress(WebInfo info)
		{
			currentCount += 1;

			if (searchPropress != null)
			{
				searchPropress(this, new SearchArgs(info, totalCount, currentCount));
			}
		}

		#endregion
	}
}
