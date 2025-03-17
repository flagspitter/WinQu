namespace QuickWatch
{
	partial class QuickWatchForm
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
			components = new System.ComponentModel.Container();
			lbCounter = new Label();
			btnReset = new Button();
			btnStartStop = new Button();
			btnLap = new Button();
			panel1 = new Panel();
			btnLapView = new Button();
			tmrInterval = new System.Windows.Forms.Timer(components);
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// lbCounter
			// 
			lbCounter.Dock = DockStyle.Fill;
			lbCounter.Font = new Font("MS UI Gothic", 20F, FontStyle.Bold);
			lbCounter.ForeColor = Color.White;
			lbCounter.Location = new Point(0, 24);
			lbCounter.Margin = new Padding(4, 0, 4, 0);
			lbCounter.Name = "lbCounter";
			lbCounter.Size = new Size(207, 27);
			lbCounter.TabIndex = 10;
			lbCounter.Text = "00:00:00.00";
			lbCounter.TextAlign = ContentAlignment.MiddleCenter;
			lbCounter.DoubleClick += lbCounter_DoubleClick;
			// 
			// btnReset
			// 
			btnReset.BackColor = Color.Crimson;
			btnReset.ForeColor = Color.White;
			btnReset.Location = new Point(118, 2);
			btnReset.Margin = new Padding(4);
			btnReset.Name = "btnReset";
			btnReset.Size = new Size(46, 26);
			btnReset.TabIndex = 2;
			btnReset.Text = "CLR";
			btnReset.UseVisualStyleBackColor = false;
			btnReset.Click += btnReset_Click;
			// 
			// btnStartStop
			// 
			btnStartStop.BackColor = Color.Blue;
			btnStartStop.ForeColor = Color.White;
			btnStartStop.Location = new Point(0, 2);
			btnStartStop.Margin = new Padding(4);
			btnStartStop.Name = "btnStartStop";
			btnStartStop.Size = new Size(58, 26);
			btnStartStop.TabIndex = 1;
			btnStartStop.Text = "START";
			btnStartStop.UseVisualStyleBackColor = false;
			btnStartStop.Click += btnStartStop_Click;
			// 
			// btnLap
			// 
			btnLap.BackColor = Color.Blue;
			btnLap.ForeColor = Color.White;
			btnLap.Location = new Point(65, 2);
			btnLap.Margin = new Padding(4);
			btnLap.Name = "btnLap";
			btnLap.Size = new Size(46, 26);
			btnLap.TabIndex = 3;
			btnLap.Text = "LAP";
			btnLap.UseVisualStyleBackColor = false;
			btnLap.Click += btnLap_Click;
			// 
			// panel1
			// 
			panel1.Controls.Add(btnReset);
			panel1.Controls.Add(btnLap);
			panel1.Controls.Add(btnStartStop);
			panel1.Dock = DockStyle.Bottom;
			panel1.Location = new Point(0, 51);
			panel1.Margin = new Padding(4);
			panel1.Name = "panel1";
			panel1.Size = new Size(207, 28);
			panel1.TabIndex = 12;
			// 
			// btnLapView
			// 
			btnLapView.BackColor = SystemColors.ControlDarkDark;
			btnLapView.Dock = DockStyle.Top;
			btnLapView.ForeColor = Color.White;
			btnLapView.Location = new Point(0, 0);
			btnLapView.Margin = new Padding(4);
			btnLapView.Name = "btnLapView";
			btnLapView.Size = new Size(207, 24);
			btnLapView.TabIndex = 13;
			btnLapView.Text = "[-] LAP ----  SPL ----";
			btnLapView.TextAlign = ContentAlignment.MiddleLeft;
			btnLapView.UseVisualStyleBackColor = false;
			btnLapView.Click += btnLapView_Click;
			// 
			// tmrInterval
			// 
			tmrInterval.Interval = 1;
			// 
			// QuickWatchForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.DimGray;
			Controls.Add(lbCounter);
			Controls.Add(panel1);
			Controls.Add(btnLapView);
			Margin = new Padding(4);
			Name = "QuickWatchForm";
			Size = new Size(207, 79);
			Load += SwForm_Load;
			panel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private Label lbCounter;
		private Button btnReset;
		private Button btnStartStop;
		private Button btnLap;
		private Panel panel1;
		private Button btnLapView;
		private System.Windows.Forms.Timer tmrInterval;
	}
}
