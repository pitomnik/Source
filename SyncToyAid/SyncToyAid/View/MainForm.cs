using SyncToy;
using SyncToyAid.Logic;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SyncToyAid.View
{
	public partial class MainForm : Form
	{
		#region Constants

		private const string binFilesFilter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
		private const string xmlFilesFilter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

		#endregion

		#region Private Members

		private ContextMenu menu;
		private SyncToyPair manager;

		#endregion

		#region Constructor(s)

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		public MainForm()
		{
			InitializeComponent();
	
			manager = new SyncToyPair();
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets a value indicating whether this instance is item selected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is item selected; otherwise, <c>false</c>.
		/// </value>
		public bool IsItemSelected
		{
			[DebuggerNonUserCode]
			get { return lvMain.SelectedItems.Count > 0; }
		}

		/// <summary>
		/// Gets the selected item.
		/// </summary>
		/// <value>The selected item.</value>
		public ListViewItem SelectedItem
		{
			[DebuggerNonUserCode]
			get { return IsItemSelected ? lvMain.SelectedItems[0] : null; }
		}

		/// <summary>
		/// Gets the selected config.
		/// </summary>
		/// <value>The selected config.</value>
		public SyncEngineConfig SelectedConfig
		{
			[DebuggerNonUserCode]
			get { return SelectedItem == null ? null : SelectedItem.Tag as SyncEngineConfig; }
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Loads the engine config.
		/// </summary>
		/// <param name="forceRead">if set to <c>true</c> force read.</param>
		private void LoadEngineConfig(bool forceRead)
		{
			lvMain.BeginUpdate();

			try
			{
				if (forceRead)
				{
					manager.Read(SyncToyInfo.Instance.ConfigPath);
				}

				lvMain.Items.Clear();

				foreach (SyncEngineConfig config in manager.List)
				{
					ListViewItem item = new ListViewItem(config.Name);

					item.Tag = config;

					item.SubItems.Add(config.LeftDir);
					item.SubItems.Add(config.RightDir);

					lvMain.Items.Add(item);
				}
			}
			finally
			{
				lvMain.EndUpdate();
				lvMain.Refresh();
			}
		}

		/// <summary>
		/// Shows the configuration.
		/// </summary>
		private void ShowConfiguration()
		{
			if (SelectedConfig != null)
			{
				PairDialog.Open(SelectedConfig);
			}
		}

		#endregion

		#region Private Events

		/// <summary>
		/// Handles the Load event of the MainForm control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void MainForm_Load(object sender, EventArgs e)
		{
			try
			{
				LoadEngineConfig(true);
			}
			catch (Exception ex)
			{
				//LogMaster.Instance.Error(String.Format("Failed reading '{0}'.", SyncToyInfo.Instance.ConfigPath), ex);
			}
		}

        /// <summary>
        /// Handles the Resize event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MainForm_Resize(object sender, EventArgs e)
        {
            lvMain.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

		/// <summary>
		/// Handles the DoubleClick event of the List control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void List_DoubleClick(object sender, EventArgs e)
		{
			ShowConfiguration();

			LoadEngineConfig(false);
		}

		#endregion
	}
}
