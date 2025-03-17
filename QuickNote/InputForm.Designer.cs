namespace QuickNote
{
	partial class InputForm
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
			NoteText = new TextBox();
			panel1 = new Panel();
			Cancel = new Button();
			Accept = new Button();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// NoteText
			// 
			NoteText.BackColor = Color.LightGray;
			NoteText.Dock = DockStyle.Fill;
			NoteText.Location = new Point(0, 0);
			NoteText.Multiline = true;
			NoteText.Name = "NoteText";
			NoteText.Size = new Size(446, 199);
			NoteText.TabIndex = 1;
			NoteText.Text = "Text";
			// 
			// panel1
			// 
			panel1.Controls.Add(Cancel);
			panel1.Controls.Add(Accept);
			panel1.Dock = DockStyle.Bottom;
			panel1.Location = new Point(0, 168);
			panel1.Name = "panel1";
			panel1.Size = new Size(446, 31);
			panel1.TabIndex = 2;
			// 
			// Cancel
			// 
			Cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			Cancel.Location = new Point(287, 5);
			Cancel.Name = "Cancel";
			Cancel.Size = new Size(75, 23);
			Cancel.TabIndex = 0;
			Cancel.Text = "Cancel(&C)";
			Cancel.UseVisualStyleBackColor = true;
			Cancel.Click += Cancel_Click;
			// 
			// Accept
			// 
			Accept.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			Accept.Location = new Point(368, 5);
			Accept.Name = "Accept";
			Accept.Size = new Size(75, 23);
			Accept.TabIndex = 0;
			Accept.Text = "Accept(&A)";
			Accept.UseVisualStyleBackColor = true;
			Accept.Click += Accept_Click;
			// 
			// InputForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = SystemColors.ControlDarkDark;
			ClientSize = new Size(446, 199);
			Controls.Add(panel1);
			Controls.Add(NoteText);
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			KeyPreview = true;
			Name = "InputForm";
			Text = "QuickNoteForm";
			panel1.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private TextBox NoteText;
		private Panel panel1;
		private Button Accept;
		private Button Cancel;
	}
}
