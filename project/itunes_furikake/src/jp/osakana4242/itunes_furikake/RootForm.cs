using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using iTunesLib;

using jp.osakana4242.itunes_furikake;
using System.Reactive.Linq;
using jp.osakana4242.core.LogOperator;

namespace jp.osakana4242.itunes_furikake
{
	public partial class RootForm : Form
	{
		public RubyAdder rubyAdder;

		public RootForm(RubyAdder rubyAdder)
		{
			InitializeComponent();
			this.rubyAdder = rubyAdder;
			this.rubyAdder.SetLogger(this.AddLog);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
		}

		// ボタン：ひらがな.
		private void button1_Click(object sender, EventArgs e)
		{
			this.StartOperation(RubyAdderOpeType.HIRAGANA);
		}

		// ボタン：カタカナ.
		private void button5_Click(object sender, EventArgs e)
		{
			this.StartOperation(RubyAdderOpeType.KATAKANA);
		}

		// ボタン：アルファベット.
		private void button2_Click(object sender, EventArgs e)
		{
			this.StartOperation(RubyAdderOpeType.ALPHABET);
		}

		// ボタン：クリア.
		private void button3_Click(object sender, EventArgs e)
		{
			this.StartOperation(RubyAdderOpeType.CLEAR);
		}

		// ボタン：バージョン情報の表示.
		private void button4_Click(object sender, EventArgs e)
		{
			VersionForm versionForm = new VersionForm();
			versionForm.ShowDialog(this);
			versionForm.Dispose();
		}

		// 強制書き換えチェックボックス。
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
		}

		// ユーザに見せるログを追加.
		public void AddLog(string str)
		{
			this.console.AppendText(str);
		}

		private void バージョン情報ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var versionForm = new VersionForm())
			{
				versionForm.ShowDialog(this);
			}
		}

		private void 全角英数を半角にするToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StartOperation(RubyAdderOpeType.ZEN2HAN);
		}

		private void 存在しないトラックを削除するToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StartOperation(RubyAdderOpeType.DELETE_UNEXISTS);
		}

		private void StartOperation(RubyAdderOpeType ope)
		{
			console.Clear();
			this.Enabled = false;
			rubyAdder.opeData.ope = ope;
			rubyAdder.opeData.isForceAdd = checkBox1.Checked;
			rubyAdder.opeData.progress = 0;
			rubyAdder.opeData.total = 0;

			switch (ope)
			{
				case RubyAdderOpeType.DELETE_UNEXISTS:
					FlowService.DeleteTrackFlow(this, rubyAdder.opeData, rubyAdder.iTunesApp);
					break;
				default:
					FlowService.UpdateTrackFlow(this);
					break;
			}
		}

		/// <summary>トラックの選択状態に応じて、各種コンポーネントの表示を切り替える.</summary>
		void UpdateComponentStatus()
		{
			int selectedTracksCount = 0;
			try
			{
				var iTunesApp = rubyAdder.iTunesApp;
				var tracks = iTunesApp.SelectedTracks;
				selectedTracksCount = tracks?.Count ?? 0;
			}
			catch (System.Exception ex)
			{
				Console.WriteLine($"ex: {ex}");
			}

			var hasSelectedTrack = 0 < selectedTracksCount;
			this.toolStripStatusLabel1.Text = hasSelectedTrack ?
				string.Format(Properties.Resources.StrRootFormStatusBar1, selectedTracksCount) :
				Properties.Resources.StrRootFormStatusBar2;

			this.button1.Enabled = hasSelectedTrack;
			this.button2.Enabled = hasSelectedTrack;
			this.button3.Enabled = hasSelectedTrack;
			this.button5.Enabled = hasSelectedTrack;
			this.全角英数を半角にするToolStripMenuItem.Enabled = hasSelectedTrack;
			this.存在しないトラックを削除するToolStripMenuItem.Enabled = hasSelectedTrack;
		}

		private void button4_Click_1(object sender, EventArgs e)
		{

		}

		private void RootForm_Activated(object sender, EventArgs e)
		{
		}

		private void RootForm_Shown(object sender, EventArgs e)
		{
			this.button1.Focus();
			UpdateComponentStatus();
			timer1.Enabled = this.WindowState != FormWindowState.Minimized;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			UpdateComponentStatus();
		}

		private void RootForm_SizeChanged(object sender, EventArgs e)
		{
			UpdateComponentStatus();
			timer1.Enabled = this.WindowState != FormWindowState.Minimized;
		}
	}
}
