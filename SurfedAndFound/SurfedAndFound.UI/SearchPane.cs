using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;

using SurfedAndFound.UI.Properties;
using SurfedAndFound.Logic.Managers;
using SurfedAndFound.Shared.Types;
using SurfedAndFound.Shared.Tools;

using Microsoft.SqlServer.MessageBox;

namespace SurfedAndFound.UI
{
	public partial class SearchPane : UserControl
	{
		#region Private Members

		private ContextMenu configMenu;
		private SynchronizationContext syncContext;
		private SearchManager searchManager;

		private delegate void ShowProgressDelegate(SearchArgs e);

		private event EventHandler<SearchArgs> searchProgress;
		private event EventHandler<ResultArgs> resultClick;

		#endregion

		#region Constructors

		public SearchPane()
		{
			InitializeComponent();
			InitializeControls();

			syncContext = SynchronizationContext.Current;
		
			searchManager = new SearchManager();

			searchManager.SearchProgress += new EventHandler<SearchArgs>(searchManager_SearchProgress);
			searchManager.SearchError += new EventHandler<ErrorArgs>(searchManager_SearchError);

			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
		}

		#endregion

		#region Public Events

		public event EventHandler<SearchArgs> SearchProgress
		{
			[DebuggerNonUserCode]
			add { searchProgress += value; }
			[DebuggerNonUserCode]
			remove { searchProgress -= value; }
		}

		public event EventHandler<ResultArgs> ResultClick
		{
			[DebuggerNonUserCode]
			add { resultClick += value; }
			[DebuggerNonUserCode]
			remove { resultClick -= value; }
		}

		#endregion

		#region Private Events

		private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			ApplicationException exception = new ApplicationException(Resources.UnexpectedError, e.Exception);

			HandleException(exception);
		}

		private void messageBox_OnCopyToClipboard(object sender, CopyToClipboardEventArgs e)
		{
			ExceptionMessageBox messageBox = sender as ExceptionMessageBox;

			if (messageBox != null)
			{
				Clipboard.SetText(messageBox.Message.ToString());

				e.EventHandled = true;
			}
		}

		private void item_Click(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem)sender;

			item.Checked = !item.Checked;
		}

		private void txtQuery_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && btnSearch.Enabled)
			{
				btnSearch_Click(sender, e);
			}
			else if (e.KeyCode == Keys.Escape && btnStop.Enabled)
			{
				btnStop_Click(sender, e);
			}
		}

		private void btnSearch_Click(object sender, EventArgs e)
		{
			EnableSearch(false);

			tvResult.Nodes.Clear();

			SearchOptions options = GetOptions();

			if (options.LookInFlags == LookIn.None || options.SearchInFlags == SearchIn.None)
			{
				MessageBox.Show(Resources.NoSearchOptions, ProgramInfo.Instance.Name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				EnableSearch(true);

				return;
			}

			if (searchManager.GetCacheCount() == 0)
			{
				CheckMesageBox.Show(CheckMesageBox.Message.EmptyCache, this, Resources.EmptyCache,
					ProgramInfo.Instance.Name, ExceptionMessageBoxButtons.OK, SystemIcons.Information);
			}

			searchManager.Options = options;

			searchManager.SearchAsync(txtQuery.Text);
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			StopSearch();
		}

		private void lnkOptions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Control control = (Control)sender;

			configMenu.Show(this, control.Location);
		}

		private void searchManager_SearchProgress(object sender, SearchArgs e)
		{
			OnSearchProgress(e);
		}

		private void searchManager_SearchError(object sender, ErrorArgs e)
		{
			e.Accepted = true;

			syncContext.Send(new SendOrPostCallback(HandleException), e.Error);
		}

		private void tvResult_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape && btnStop.Enabled)
			{
				btnStop_Click(sender, e);
			}
		}

		private void tvResult_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag == null)
			{
				tvResult.CollapseAll();
				e.Node.Expand();
			}
			else
			{
				OnResultClick((WebInfo)e.Node.Tag);
			}
		}

		#endregion

		#region Public Methods

		public void SaveState()
		{
			WriteOptions(GetOptions());
		}

		#endregion

		#region Private Methods

		private void HandleException(object exception)
		{
			HandleException(exception as Exception);

			EnableSearch(true);
		}

		private void HandleException(Exception exception)
		{
			ExceptionMessageBox messageBox = new ExceptionMessageBox(exception);

			messageBox.Caption = ProgramInfo.Instance.Name;

			messageBox.OnCopyToClipboard += new CopyToClipboardEventHandler(messageBox_OnCopyToClipboard);

			messageBox.Show(this);

			messageBox.OnCopyToClipboard -= messageBox_OnCopyToClipboard;
		}

		private void InitializeControls()
		{
			configMenu = new ContextMenu();

			AddMenuItems(typeof(LookIn));
			AddMenuItems(typeof(SearchIn));

			SetOptions(ReadOptions());

			btnStop.Enabled = false;
		}

		private void AddMenuItems(Type type)
		{
			MenuItem root = new MenuItem();

			root.Name = GetTypeName(type);
			root.Text = Translate(root.Name);

			int index = configMenu.MenuItems.Add(root);

			foreach (Enum value in Enum.GetValues(type))
			{
				if (Convert.ToInt32(value) == 0)
				{
					continue;
				}

				MenuItem item = new MenuItem();

				item.Tag = value;
				item.Name = value.ToString();
				item.Text = Translate(item.Name);
				item.Click += new EventHandler(item_Click);

				configMenu.MenuItems[index].MenuItems.Add(item);
			}
		}

		private string GetTypeName(Type type)
		{
			return type.Name;
		}

		private string GetEnumValueName(Enum value)
		{
			return GetTypeName(value.GetType()) + value;
		}

		private MenuItem.MenuItemCollection GetMenuItems(Type type)
		{
			return configMenu.MenuItems[GetTypeName(type)].MenuItems;
		}

		private string Translate(string s)
		{
			string translation = Resources.ResourceManager.GetString(s);

			return translation ?? s;
		}

		private void EnableSearch(bool enable)
		{
			btnSearch.Enabled = enable;
			btnStop.Enabled = !enable;
		}

		private void ShowProgress(SearchArgs e)
		{
			if (e.Info.Found)
			{
				string site = new Uri(e.Info.Url).Authority;
				TreeNode node = new TreeNode();

				node.Name = e.Info.Url;
				node.Text = e.Info.ToString();
				node.Tag = e.Info;

				TreeNode[] nodes = tvResult.Nodes.Find(site, false);
				TreeNode root;

				Debug.Assert(nodes.Length <= 1, String.Format("Unexpected nodes count: {0}.", nodes.Length));

				if (nodes.Length == 0)
				{
					root = new TreeNode();

					root.Name = site;
					root.Text = site;

					InsertTreeNode(tvResult.Nodes, root);
				}
				else
				{
					root = nodes[0];
				}

				InsertTreeNode(root.Nodes, node);
			}

			if (e.CurrentCount == e.TotalCount)
			{
				EnableSearch(true);
			}
		}

		private void InsertTreeNode(TreeNodeCollection nodes, TreeNode node)
		{
			int index = -1;

			foreach (TreeNode child in nodes)
			{
				if (String.Compare(node.Text, child.Text) <= 0)
				{
					index = child.Index;
					break;
				}
			}

			if (index == -1)
			{
				nodes.Add(node);
			}
			else
			{
				nodes.Insert(index, node);
			}
		}

		protected virtual void OnSearchProgress(SearchArgs args)
		{
			if (InvokeRequired)
			{
				Invoke(new ShowProgressDelegate(ShowProgress), new object[] { args });
			}
			else
			{
				ShowProgress(args);
			}

			if (searchProgress != null)
			{
				searchProgress(this, args);
			}
		}

		protected virtual void OnResultClick(WebInfo info)
		{
			if (resultClick != null)
			{
				resultClick(this, new ResultArgs(info));
			}
		}

		private SearchOptions ReadOptions()
		{
			SearchOptions options = new SearchOptions();

			options.LookInFlags = (LookIn)UserSettings.Read<int>(GetTypeName(typeof(LookIn)), (int)(LookIn.Favorites | LookIn.History | LookIn.Recent));
			options.SearchInFlags = (SearchIn)UserSettings.Read<int>(GetTypeName(typeof(SearchIn)), (int)(SearchIn.Text | SearchIn.Title));

			return options;
		}

		private void WriteOptions(SearchOptions options)
		{
			UserSettings.Write<int>(GetTypeName(typeof(LookIn)), (int)options.LookInFlags);
			UserSettings.Write<int>(GetTypeName(typeof(SearchIn)), (int)options.SearchInFlags);
		}

		private SearchOptions GetOptions()
		{
			SearchOptions options = new SearchOptions();

			foreach (MenuItem item in configMenu.MenuItems[GetTypeName(typeof(LookIn))].MenuItems)
			{
				if (item.Checked)
				{
					options.LookInFlags |= (LookIn)item.Tag;
				}
			}

			foreach (MenuItem item in configMenu.MenuItems[GetTypeName(typeof(SearchIn))].MenuItems)
			{
				if (item.Checked)
				{
					options.SearchInFlags |= (SearchIn)item.Tag;
				}
			}

			return options;
		}

		private void SetOptions(SearchOptions options)
		{
			foreach (MenuItem item in configMenu.MenuItems[GetTypeName(typeof(LookIn))].MenuItems)
			{
				item.Checked = ((LookIn)item.Tag & options.LookInFlags) != LookIn.None;
			}

			foreach (MenuItem item in configMenu.MenuItems[GetTypeName(typeof(SearchIn))].MenuItems)
			{
				item.Checked = ((SearchIn)item.Tag & options.SearchInFlags) != SearchIn.None;
			}
		}

		protected void StopSearch()
		{
			EnableSearch(true);
			searchManager.CancelAsync();
		}

		#endregion
	}
}