using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;

using SurfedAndFound.UI;
using SurfedAndFound.Logic.Managers;
using SurfedAndFound.Shared.Types;
using SurfedAndFound.Shared.Tools;

namespace SurfedAndFound.Tool
{
	public partial class MainForm : Form
	{
		#region Private Members

		private delegate void SearchProgressDelegate(SearchArgs e);

		#endregion

		#region Constructors

		public MainForm()
		{
			InitializeComponent();

			Text = ProgramInfo.Instance.Name;

			spMain.SearchProgress += new EventHandler<SearchArgs>(spMain_SearchProgress);
			spMain.ResultClick += new EventHandler<ResultArgs>(spMain_ResultClick);
		}

		#endregion

		#region Private Events

		private void spMain_SearchProgress(object sender, SearchArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new SearchProgressDelegate(OnSearchProgress), new object[] { e });
			}
			else
			{
				OnSearchProgress(e);
			}
		}

		private void spMain_ResultClick(object sender, ResultArgs e)
		{
			OnResultClick(e);
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			spMain.SaveState();
		}

		#endregion

		#region Private Methods

		private void OnSearchProgress(SearchArgs e)
		{
			pbProgress.Value = e.CurrentCount;
			pbProgress.Maximum = e.TotalCount;

			lblProgress.Text = e.Info.Url;
		}

		private void OnResultClick(ResultArgs e)
		{
			wbMain.Navigate(e.Info.Url);
		}

		#endregion
	}
}