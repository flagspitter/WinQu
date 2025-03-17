using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickNote
{
	public partial class InputForm : Form
	{
		public InputForm()
		{
			InitializeComponent();
		}

		public void SetNote(string key, string note)
		{
			this.Text = $"QuickNote [ {key} ]";
			NoteText.Text = note;
		}

		private void Accept_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult= DialogResult.Cancel;
			this.Close();
		}

		public string Note => NoteText.Text;
	}
}
