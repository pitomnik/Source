namespace SurfedAndFound.Demo
{
	partial class DemoForm
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
			this.demoBrowser = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			// 
			// demoBrowser
			// 
			this.demoBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.demoBrowser.Location = new System.Drawing.Point(0, 0);
			this.demoBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.demoBrowser.Name = "demoBrowser";
			this.demoBrowser.ScrollBarsEnabled = false;
			this.demoBrowser.Size = new System.Drawing.Size(724, 558);
			this.demoBrowser.TabIndex = 0;
			this.demoBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.demoBrowser_PreviewKeyDown);
			// 
			// DemoForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(724, 558);
			this.Controls.Add(this.demoBrowser);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DemoForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DemoForm_KeyUp);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.WebBrowser demoBrowser;
	}
}