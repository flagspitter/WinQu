namespace QuickBoard
{
	partial class QuickBoardForm
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
			ClipListView = new SortableListView();
			KeyColumn = new ColumnHeader();
			TypeColumn = new ColumnHeader();
			TextColumn = new ColumnHeader();
			Title = new Label();
			SuspendLayout();
			// 
			// ClipListView
			// 
			ClipListView.BackColor = SystemColors.ControlLight;
			ClipListView.BorderStyle = BorderStyle.None;
			ClipListView.Columns.AddRange(new ColumnHeader[] { KeyColumn, TypeColumn, TextColumn });
			ClipListView.Dock = DockStyle.Fill;
			ClipListView.ForeColor = SystemColors.WindowText;
			ClipListView.FullRowSelect = true;
			ClipListView.GridLines = true;
			ClipListView.Location = new Point(0, 15);
			ClipListView.Name = "ClipListView";
			ClipListView.OwnerDraw = true;
			ClipListView.Size = new Size(272, 122);
			ClipListView.SortTarget = -1;
			ClipListView.TabIndex = 3;
			ClipListView.UseCompatibleStateImageBehavior = false;
			ClipListView.View = View.Details;
			ClipListView.KeyDown += BoardListView_KeyDown;
			// 
			// KeyColumn
			// 
			KeyColumn.Text = "Key";
			KeyColumn.Width = 40;
			// 
			// TypeColumn
			// 
			TypeColumn.Text = "Type";
			TypeColumn.Width = 50;
			// 
			// TextColumn
			// 
			TextColumn.Text = "Text";
			TextColumn.Width = 300;
			// 
			// Title
			// 
			Title.Dock = DockStyle.Top;
			Title.Location = new Point(0, 0);
			Title.Name = "Title";
			Title.Size = new Size(272, 15);
			Title.TabIndex = 4;
			Title.Text = "Clickboard";
			// 
			// QuickBoardForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(ClipListView);
			Controls.Add(Title);
			Name = "QuickBoardForm";
			Size = new Size(272, 137);
			KeyDown += QuickBoardForm_KeyDown;
			ResumeLayout(false);
		}

		#endregion

		private SortableListView ClipListView;
		private ColumnHeader KeyColumn;
		private ColumnHeader TextColumn;
		private ColumnHeader TypeColumn;
		private Label Title;
	}
}
