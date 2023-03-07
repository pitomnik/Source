namespace SurfedAndFound.UI
{
	partial class SearchPane
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlTop = new System.Windows.Forms.Panel();
			this.btnStop = new System.Windows.Forms.Button();
			this.lnkOptions = new System.Windows.Forms.LinkLabel();
			this.lblSearch = new System.Windows.Forms.Label();
			this.btnSearch = new System.Windows.Forms.Button();
			this.txtQuery = new System.Windows.Forms.TextBox();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.tvResult = new System.Windows.Forms.TreeView();
			this.pnlTop.SuspendLayout();
			this.pnlBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlTop
			// 
			this.pnlTop.Controls.Add(this.btnStop);
			this.pnlTop.Controls.Add(this.lnkOptions);
			this.pnlTop.Controls.Add(this.lblSearch);
			this.pnlTop.Controls.Add(this.btnSearch);
			this.pnlTop.Controls.Add(this.txtQuery);
			this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTop.Location = new System.Drawing.Point(0, 0);
			this.pnlTop.Name = "pnlTop";
			this.pnlTop.Size = new System.Drawing.Size(250, 80);
			this.pnlTop.TabIndex = 6;
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(87, 51);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 3;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// lnkOptions
			// 
			this.lnkOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkOptions.AutoSize = true;
			this.lnkOptions.Location = new System.Drawing.Point(201, 7);
			this.lnkOptions.Name = "lnkOptions";
			this.lnkOptions.Size = new System.Drawing.Size(43, 13);
			this.lnkOptions.TabIndex = 4;
			this.lnkOptions.TabStop = true;
			this.lnkOptions.Text = "Options";
			this.lnkOptions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOptions_LinkClicked);
			// 
			// lblSearch
			// 
			this.lblSearch.AutoSize = true;
			this.lblSearch.Location = new System.Drawing.Point(3, 7);
			this.lblSearch.Name = "lblSearch";
			this.lblSearch.Size = new System.Drawing.Size(59, 13);
			this.lblSearch.TabIndex = 0;
			this.lblSearch.Text = "Search for:";
			// 
			// btnSearch
			// 
			this.btnSearch.Location = new System.Drawing.Point(6, 51);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(75, 23);
			this.btnSearch.TabIndex = 2;
			this.btnSearch.Text = "Search";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// txtQuery
			// 
			this.txtQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtQuery.Location = new System.Drawing.Point(6, 25);
			this.txtQuery.Name = "txtQuery";
			this.txtQuery.Size = new System.Drawing.Size(238, 20);
			this.txtQuery.TabIndex = 1;
			this.txtQuery.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtQuery_KeyUp);
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.tvResult);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlBottom.Location = new System.Drawing.Point(0, 80);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(250, 304);
			this.pnlBottom.TabIndex = 7;
			// 
			// tvResult
			// 
			this.tvResult.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvResult.FullRowSelect = true;
			this.tvResult.HotTracking = true;
			this.tvResult.Location = new System.Drawing.Point(0, 0);
			this.tvResult.Name = "tvResult";
			this.tvResult.ShowLines = false;
			this.tvResult.ShowRootLines = false;
			this.tvResult.Size = new System.Drawing.Size(250, 304);
			this.tvResult.TabIndex = 5;
			this.tvResult.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvResult_AfterSelect);
			this.tvResult.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvResult_KeyUp);
			// 
			// SearchPane
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlBottom);
			this.Controls.Add(this.pnlTop);
			this.Name = "SearchPane";
			this.Size = new System.Drawing.Size(250, 384);
			this.pnlTop.ResumeLayout(false);
			this.pnlTop.PerformLayout();
			this.pnlBottom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlTop;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.Label lblSearch;
		private System.Windows.Forms.LinkLabel lnkOptions;
		private System.Windows.Forms.Button btnStop;
		protected System.Windows.Forms.TextBox txtQuery;
		private System.Windows.Forms.TreeView tvResult;

	}
}
