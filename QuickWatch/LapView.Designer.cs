namespace QuickWatch
{
	partial class LapView
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
			lstLap = new SortableListView();
			clmNo = new ColumnHeader();
			clmLap = new ColumnHeader();
			clmSpl = new ColumnHeader();
			btnCopy = new Button();
			btnClose = new Button();
			SuspendLayout();
			// 
			// lstLap
			// 
			lstLap.BorderStyle = BorderStyle.None;
			lstLap.Columns.AddRange(new ColumnHeader[] { clmNo, clmLap, clmSpl });
			lstLap.Dock = DockStyle.Top;
			lstLap.GridLines = true;
			lstLap.Location = new Point(0, 0);
			lstLap.Margin = new Padding(4, 4, 4, 4);
			lstLap.Name = "lstLap";
			lstLap.OwnerDraw = true;
			lstLap.Size = new Size(335, 281);
			lstLap.SortTarget = -1;
			lstLap.TabIndex = 0;
			lstLap.UseCompatibleStateImageBehavior = false;
			lstLap.View = View.Details;
			// 
			// clmNo
			// 
			clmNo.Text = "No";
			// 
			// clmLap
			// 
			clmLap.Text = "LAP";
			clmLap.Width = 100;
			// 
			// clmSpl
			// 
			clmSpl.Text = "SPLIT";
			clmSpl.Width = 100;
			// 
			// btnCopy
			// 
			btnCopy.Location = new Point(0, 289);
			btnCopy.Margin = new Padding(4, 4, 4, 4);
			btnCopy.Name = "btnCopy";
			btnCopy.Size = new Size(169, 45);
			btnCopy.TabIndex = 97;
			btnCopy.Text = "クリップボードにコピー(&C)";
			btnCopy.UseVisualStyleBackColor = true;
			btnCopy.Click += btnCopy_Click;
			// 
			// btnClose
			// 
			btnClose.DialogResult = DialogResult.Cancel;
			btnClose.Location = new Point(198, 289);
			btnClose.Margin = new Padding(4, 4, 4, 4);
			btnClose.Name = "btnClose";
			btnClose.Size = new Size(136, 45);
			btnClose.TabIndex = 98;
			btnClose.Text = "閉じる(&X)";
			btnClose.UseVisualStyleBackColor = true;
			btnClose.Click += btnClose_Click;
			// 
			// LapView
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(335, 338);
			Controls.Add(btnClose);
			Controls.Add(btnCopy);
			Controls.Add(lstLap);
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			Margin = new Padding(4, 4, 4, 4);
			Name = "LapView";
			Text = "LAP - QuickWatch";
			FormClosing += LapView_FormClosing;
			ResumeLayout(false);
		}

		#endregion

		private SortableListView lstLap;
		private System.Windows.Forms.ColumnHeader clmNo;
		private System.Windows.Forms.ColumnHeader clmLap;
		private System.Windows.Forms.ColumnHeader clmSpl;
		private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.Button btnClose;
	}
}
