using SyncToy;
using System.Windows.Forms;

namespace SyncToyAid.View
{
	public partial class PairDialog : Form
	{
		public PairDialog()
		{
			InitializeComponent();
		}

		public static SyncEngineConfig Open(SyncEngineConfig config)
		{
			PairDialog form = new PairDialog();

			form.pgConfig.SelectedObject = config;

			form.ShowDialog();

			return (SyncEngineConfig)form.pgConfig.SelectedObject;
		}

		private void ConfigForm_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				Close();
			}
		}
	}
}
