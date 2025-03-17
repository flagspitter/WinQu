namespace QuickNote
{
	partial class QuickNoteForm
	{
		/// <summary> 
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region コンポーネント デザイナーで生成されたコード

		/// <summary> 
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			NoteListView = new SortableListView();
			KeyColumn = new ColumnHeader();
			TextColumn = new ColumnHeader();
			SuspendLayout();
			// 
			// NoteListView
			// 
			NoteListView.BackColor = SystemColors.ControlLight;
			NoteListView.BorderStyle = BorderStyle.None;
			NoteListView.Columns.AddRange(new ColumnHeader[] { KeyColumn, TextColumn });
			NoteListView.Dock = DockStyle.Fill;
			NoteListView.ForeColor = SystemColors.WindowText;
			NoteListView.Location = new Point(0, 0);
			NoteListView.Name = "NoteListView";
			NoteListView.OwnerDraw = true;
			NoteListView.Size = new Size(272, 137);
			NoteListView.SortTarget = -1;
			NoteListView.TabIndex = 3;
			NoteListView.UseCompatibleStateImageBehavior = false;
			NoteListView.View = View.Details;
			NoteListView.KeyDown += NoteListView_KeyDown;
			// 
			// KeyColumn
			// 
			KeyColumn.Text = "Key";
			// 
			// TextColumn
			// 
			TextColumn.Text = "Text";
			TextColumn.Width = 360;
			// 
			// QuickNoteForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(NoteListView);
			Name = "QuickNoteForm";
			Size = new Size(272, 137);
			KeyDown += QuickNoteForm_KeyDown;
			ResumeLayout(false);
		}

		#endregion

		private SortableListView NoteListView;
		private ColumnHeader KeyColumn;
		private ColumnHeader TextColumn;
	}
}
