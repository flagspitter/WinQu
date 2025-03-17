namespace WinQu
{
	partial class WinQu
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

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinQu));
			notifyIcon1 = new NotifyIcon(components);
			SuspendLayout();
			// 
			// notifyIcon1
			// 
			notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
			notifyIcon1.Text = "icon";
			notifyIcon1.Visible = true;
			// 
			// WinQu
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.DimGray;
			ClientSize = new Size(262, 65);
			FormBorderStyle = FormBorderStyle.None;
			KeyPreview = true;
			Margin = new Padding(4);
			Name = "WinQu";
			Text = "Qtt";
			Activated += MainWindow_Activated;
			Deactivate += MainWindow_Deactivate;
			FormClosing += MainWindow_FormClosing;
			FormClosed += MainWindow_FormClosed;
			Load += MainWindow_Load;
			Shown += MainWindow_Shown;
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.NotifyIcon notifyIcon1;
	}
}

