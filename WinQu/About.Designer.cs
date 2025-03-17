namespace WinQu
{
	partial class About
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
			panel1 = new Panel();
			Accept = new Button();
			VersionInfo = new RichTextBox();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// panel1
			// 
			panel1.Controls.Add(Accept);
			panel1.Dock = DockStyle.Bottom;
			panel1.Location = new Point(0, 244);
			panel1.Name = "panel1";
			panel1.Size = new Size(288, 36);
			panel1.TabIndex = 1;
			// 
			// Accept
			// 
			Accept.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			Accept.Location = new Point(201, 6);
			Accept.Name = "Accept";
			Accept.Size = new Size(75, 23);
			Accept.TabIndex = 0;
			Accept.Text = "OK";
			Accept.UseVisualStyleBackColor = true;
			Accept.Click += Accept_Click;
			// 
			// VersionInfo
			// 
			VersionInfo.BackColor = SystemColors.Control;
			VersionInfo.BorderStyle = BorderStyle.None;
			VersionInfo.Dock = DockStyle.Fill;
			VersionInfo.Location = new Point(0, 0);
			VersionInfo.Name = "VersionInfo";
			VersionInfo.ReadOnly = true;
			VersionInfo.Size = new Size(288, 244);
			VersionInfo.TabIndex = 2;
			VersionInfo.Text = "";
			// 
			// About
			// 
			AcceptButton = Accept;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = Accept;
			ClientSize = new Size(288, 280);
			Controls.Add(VersionInfo);
			Controls.Add(panel1);
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			Name = "About";
			Text = "About";
			Load += About_Load;
			panel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private Panel panel1;
		private Button Accept;
		private RichTextBox VersionInfo;
	}
}