namespace SyncToyAid.View
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
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLeftFolder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRightFolder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvMain = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 150;
            // 
            // colLeftFolder
            // 
            this.colLeftFolder.Text = "Left Folder";
            this.colLeftFolder.Width = 225;
            // 
            // colRightFolder
            // 
            this.colRightFolder.Text = "Right Folder";
            this.colRightFolder.Width = 225;
            // 
            // lvMain
            // 
            this.lvMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colLeftFolder,
            this.colRightFolder});
            this.lvMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMain.FullRowSelect = true;
            this.lvMain.LabelEdit = true;
            this.lvMain.Location = new System.Drawing.Point(0, 0);
            this.lvMain.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.lvMain.MultiSelect = false;
            this.lvMain.Name = "lvMain";
            this.lvMain.Scrollable = false;
            this.lvMain.ShowGroups = false;
            this.lvMain.ShowItemToolTips = true;
            this.lvMain.Size = new System.Drawing.Size(1611, 911);
            this.lvMain.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvMain.TabIndex = 3;
            this.lvMain.UseCompatibleStateImageBehavior = false;
            this.lvMain.View = System.Windows.Forms.View.Details;
            this.lvMain.DoubleClick += new System.EventHandler(this.List_DoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1611, 911);
            this.Controls.Add(this.lvMain);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "MainForm";
            this.Text = "MianForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.ColumnHeader colName;
		private System.Windows.Forms.ColumnHeader colLeftFolder;
		private System.Windows.Forms.ColumnHeader colRightFolder;
		private System.Windows.Forms.ListView lvMain;


	}
}

