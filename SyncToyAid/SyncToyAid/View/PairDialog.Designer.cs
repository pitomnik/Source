namespace SyncToyAid.View
{
	partial class PairDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pgConfig = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// pgConfig
			// 
			this.pgConfig.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgConfig.Location = new System.Drawing.Point(0, 0);
			this.pgConfig.Name = "pgConfig";
			this.pgConfig.Size = new System.Drawing.Size(384, 466);
			this.pgConfig.TabIndex = 0;
			// 
			// ConfigForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 466);
			this.Controls.Add(this.pgConfig);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.KeyPreview = true;
			this.Name = "ConfigForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ConfigForm";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ConfigForm_KeyUp);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid pgConfig;

	}
}