using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace WinQu
{
	public partial class About : Form
	{
		public About()
		{
			InitializeComponent();
		}

		public string ModulesVersion { get; set; } = "";

		private void About_Load(object sender, EventArgs e)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			// アセンブリ情報を取得
			var appName = GetAssemblyAttribute<AssemblyTitleAttribute>(assembly)?.Title ?? "(AppName ???)";
			var author = GetAssemblyAttribute<AssemblyCompanyAttribute>(assembly)?.Company ?? "(Author ???)";
			var version = assembly?.GetName()?.Version?.ToString() ?? "(Version ???)";

			VersionInfo.Text = $"""
				------------------------------------------------
				 {appName} {version}
				------------------------------------------------
				 Copyright 2025 {author}. All rights reserved.
				 This software is licensed under the MIT License.
				 For details, see "LICENSE.txt"
				------------------------------------------------
				 WinQu is a set of lightweight utilities can be activated with hot keys.
				------------------------------------------------
				 Modules:
				{ModulesVersion}
				""";
		}

		private void Accept_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private static T? GetAssemblyAttribute<T>(Assembly assembly) where T : Attribute
		{
			return (T?)Attribute.GetCustomAttribute(assembly, typeof(T));
		}
	}
}
