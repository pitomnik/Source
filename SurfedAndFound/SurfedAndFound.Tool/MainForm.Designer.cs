namespace SurfedAndFound.Tool
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.ssMain = new System.Windows.Forms.StatusStrip();
			this.pbProgress = new System.Windows.Forms.ToolStripProgressBar();
			this.lblProgress = new System.Windows.Forms.ToolStripStatusLabel();
			this.scMain = new System.Windows.Forms.SplitContainer();
			this.spMain = new SurfedAndFound.UI.SearchPane();
			this.wbMain = new System.Windows.Forms.WebBrowser();
			this.ssMain.SuspendLayout();
			this.scMain.Panel1.SuspendLayout();
			this.scMain.Panel2.SuspendLayout();
			this.scMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// ssMain
			// 
			this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pbProgress,
            this.lblProgress});
			this.ssMain.Location = new System.Drawing.Point(0, 418);
			this.ssMain.Name = "ssMain";
			this.ssMain.Size = new System.Drawing.Size(625, 22);
			this.ssMain.TabIndex = 1;
			this.ssMain.Text = "statusStrip1";
			// 
			// pbProgress
			// 
			this.pbProgress.Name = "pbProgress";
			this.pbProgress.Size = new System.Drawing.Size(100, 16);
			// 
			// lblProgress
			// 
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(508, 17);
			this.lblProgress.Spring = true;
			this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// scMain
			// 
			this.scMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scMain.Location = new System.Drawing.Point(0, 0);
			this.scMain.Name = "scMain";
			// 
			// scMain.Panel1
			// 
			this.scMain.Panel1.Controls.Add(this.spMain);
			// 
			// scMain.Panel2
			// 
			this.scMain.Panel2.Controls.Add(this.wbMain);
			this.scMain.Size = new System.Drawing.Size(625, 418);
			this.scMain.SplitterDistance = 208;
			this.scMain.TabIndex = 2;
			// 
			// spMain
			// 
			this.spMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.spMain.Location = new System.Drawing.Point(0, 0);
			this.spMain.Name = "spMain";
			this.spMain.Size = new System.Drawing.Size(208, 418);
			this.spMain.TabIndex = 0;
			// 
			// wbMain
			// 
			this.wbMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wbMain.Location = new System.Drawing.Point(0, 0);
			this.wbMain.MinimumSize = new System.Drawing.Size(20, 20);
			this.wbMain.Name = "wbMain";
			this.wbMain.ScriptErrorsSuppressed = true;
			this.wbMain.Size = new System.Drawing.Size(413, 418);
			this.wbMain.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(625, 440);
			this.Controls.Add(this.scMain);
			this.Controls.Add(this.ssMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.ssMain.ResumeLayout(false);
			this.ssMain.PerformLayout();
			this.scMain.Panel1.ResumeLayout(false);
			this.scMain.Panel2.ResumeLayout(false);
			this.scMain.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip ssMain;
		private System.Windows.Forms.ToolStripProgressBar pbProgress;
		private System.Windows.Forms.ToolStripStatusLabel lblProgress;
		private System.Windows.Forms.SplitContainer scMain;
		private SurfedAndFound.UI.SearchPane spMain;
		private System.Windows.Forms.WebBrowser wbMain;

	}
}