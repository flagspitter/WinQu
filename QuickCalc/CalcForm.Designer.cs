namespace QuickCalc
{
	partial class CalcForm
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
			lbResult = new Label();
			tmrInterval = new System.Windows.Forms.Timer(components);
			txtExpression = new TextBox();
			lbResultHex = new Label();
			SuspendLayout();
			// 
			// lbResult
			// 
			lbResult.BackColor = Color.DimGray;
			lbResult.BorderStyle = BorderStyle.FixedSingle;
			lbResult.Dock = DockStyle.Top;
			lbResult.Font = new Font("MS UI Gothic", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 128);
			lbResult.ForeColor = Color.White;
			lbResult.Location = new Point(0, 0);
			lbResult.Margin = new Padding(4, 0, 4, 0);
			lbResult.Name = "lbResult";
			lbResult.Size = new Size(257, 40);
			lbResult.TabIndex = 4;
			lbResult.Text = "---";
			lbResult.TextAlign = ContentAlignment.MiddleRight;
			lbResult.Paint += lbResult_Paint;
			// 
			// tmrInterval
			// 
			tmrInterval.Interval = 50;
			tmrInterval.Tick += tmrInterval_Tick;
			// 
			// txtExpression
			// 
			txtExpression.BackColor = Color.Black;
			txtExpression.Dock = DockStyle.Fill;
			txtExpression.Font = new Font("MS UI Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 128);
			txtExpression.ForeColor = Color.White;
			txtExpression.ImeMode = ImeMode.Disable;
			txtExpression.Location = new Point(0, 67);
			txtExpression.Margin = new Padding(4);
			txtExpression.Multiline = true;
			txtExpression.Name = "txtExpression";
			txtExpression.Size = new Size(257, 52);
			txtExpression.TabIndex = 5;
			txtExpression.TextChanged += txtExpression_TextChanged;
			txtExpression.KeyDown += txtExpression_KeyDown;
			txtExpression.KeyPress += txtExpression_KeyPress;
			// 
			// lbResultHex
			// 
			lbResultHex.BackColor = Color.DimGray;
			lbResultHex.BorderStyle = BorderStyle.FixedSingle;
			lbResultHex.Dock = DockStyle.Top;
			lbResultHex.Font = new Font("MS UI Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 128);
			lbResultHex.ForeColor = Color.White;
			lbResultHex.Location = new Point(0, 40);
			lbResultHex.Margin = new Padding(4, 0, 4, 0);
			lbResultHex.Name = "lbResultHex";
			lbResultHex.Size = new Size(257, 27);
			lbResultHex.TabIndex = 6;
			lbResultHex.Text = "---";
			lbResultHex.TextAlign = ContentAlignment.MiddleRight;
			// 
			// CalcForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(txtExpression);
			Controls.Add(lbResultHex);
			Controls.Add(lbResult);
			Margin = new Padding(4);
			Name = "CalcForm";
			Size = new Size(257, 119);
			Load += CalcForm_Load;
			Enter += CalcForm_Enter;
			KeyDown += CalcForm_KeyDown;
			KeyPress += CalcForm_KeyPress;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label lbResult;
		private System.Windows.Forms.Timer tmrInterval;
		private System.Windows.Forms.TextBox txtExpression;
		private System.Windows.Forms.Label lbResultHex;
	}
}
