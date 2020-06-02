using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace jp.osakana4242.itunes_furikake
{
	public partial class OkCancelDialog : Form
	{
		System.Action<Result> onCompleted;
		Result result = Result.Cancel;

		public static void ShowOK(IWin32Window owner, string title, string body, System.Action<Result> onCompleted = null)
		{
			Show(owner, Type.OK, title, body, onCompleted);
		}

		public static void ShowOKCancel(IWin32Window owner, string title, string body, System.Action<Result> onCompleted = null)
		{
			Show(owner, Type.OKCancel, title, body, onCompleted);
		}

		public static void Show(IWin32Window owner, Type type, string title, string body, System.Action<Result> onCompleted = null)
		{
			using (var self = new OkCancelDialog(type, title, body, onCompleted))
			{
				self.ShowDialog(owner);
			}

		}

		public OkCancelDialog(Type type, string title, string body, System.Action<Result> onCompleted)
		{
			InitializeComponent();
			this.onCompleted = onCompleted ?? (_  => { });
			this.Text = title;
			this.textBox.Text = body;
			this.cancelButton.Visible = type == Type.OKCancel;
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.result = Result.OK;
			this.Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.result = Result.Cancel;
			this.Close();
		}


		private void OkCancelDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			FlowService.Delay(Tuple.Create(this.onCompleted, this.result), _prm =>
			{
				_prm.Item1(_prm.Item2);
			});
		}

		private void OkCancelDialog_Shown(object sender, EventArgs e)
		{
			this.Focus();
			this.okButton.Focus();
		}

		public enum Type
		{
			OK,
			OKCancel,
		}

		public enum Result
		{
			Cancel,
			OK,
		}

	}
}
