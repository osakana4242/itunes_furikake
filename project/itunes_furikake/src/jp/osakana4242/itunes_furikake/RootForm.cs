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
		public readonly RubyAdder rubyAdder;
		readonly Dictionary<RubyAdderRubyType, RadioButton> radioButtonsForRubyAddType_;

		public RootForm()
		{
			InitializeComponent();
			radioButtonsForRubyAddType_ = new Dictionary<RubyAdderRubyType, RadioButton> {
				{ RubyAdderRubyType.Hiragana, radioButtonRubyAddTypeHiragana },
				{ RubyAdderRubyType.Alphabet, radioButtonRubyAddTypeAlphabet },
			};
			this.rubyAdder = new RubyAdder();
			this.rubyAdder.Init();
			this.rubyAdder.SetLogger(this.AddLog);

			this.OnDestroyedAsObservableExt().Subscribe(_ =>
			{
				this.rubyAdder.Dispose();
			});
		}

		private void Form1_Load(object sender, EventArgs e)
		{
		}

		// 強制書き換えチェックボックス。
		private void checkBoxRubyAddOverwrite_CheckedChanged(object sender, EventArgs e)
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

		private void 存在しないトラックを削除するToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StartOperation(RubyAdderOpeType.DELETE_UNEXISTS);
		}

		private void StartOperation(RubyAdderOpeType ope)
		{
			console.Clear();
			this.Enabled = false;
			rubyAdder.opeData.ope = ope;
			rubyAdder.opeData.isForceAdd = checkBoxRubyAddOverwrite.Checked;
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
			int selectedTracksCount;
            if (RubyAdder.TryGetSelectedTracks(rubyAdder, out var tracks))
            {
				selectedTracksCount = tracks.Count;
            }
            else
            {
                selectedTracksCount = 0;
            }

			foreach ( var kv in radioButtonsForRubyAddType_ ) {
				kv.Value.Enabled = kv.Key == rubyAdder.opeData.rubyType;
			}

			var hasSelectedTrack = 0 < selectedTracksCount;
			this.toolStripStatusLabel1.Text = hasSelectedTrack ?
				string.Format(Properties.Resources.StrRootFormStatusBar1, selectedTracksCount) :
				Properties.Resources.StrRootFormStatusBar2;

			this.groupBoxRubyAdd.Enabled = this.checkBoxRubyAdd.Checked;
			this.buttonApply.Enabled = hasSelectedTrack;
			this.存在しないトラックを削除するToolStripMenuItem.Enabled = hasSelectedTrack;
		}

		private void RootForm_Activated(object sender, EventArgs e)
		{
		}

		private void RootForm_Shown(object sender, EventArgs e)
		{
			this.buttonApply.Focus();
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

		private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e) {

		}

		private void buttonApply_Click(object sender, EventArgs e) {

		}

		private void checkBoxRubyAddEnabled_CheckedChanged(object sender, EventArgs e) {
			UpdateComponentStatus();
		}
	}
}
