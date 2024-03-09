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
		int antiUpdateComponentStatusCount_;

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
			SettingToUI(rubyAdder.opeData.setting);

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
			StartOperation(RubyAdderOpeType.DeleteUnexistsTrack);
		}

		private void StartOperation(RubyAdderOpeType ope)
		{
			console.Clear();
			this.Enabled = false;
			rubyAdder.opeData.ope = ope;
			rubyAdder.opeData.progress = 0;
			rubyAdder.opeData.total = 0;

			switch (ope) {
				case RubyAdderOpeType.DeleteUnexistsTrack:
					FlowService.DeleteTrackFlow(this, rubyAdder.opeData, rubyAdder.iTunesApp);
					break;
				case RubyAdderOpeType.UpdateTrack:
					FlowService.UpdateTrackFlow(this);
					break;
				default:
					throw new System.NotSupportedException($"ope: {ope}");
			}
		}

		void SettingToUI(RubyAdderOpeData.Setting setting) {
			using (CreateAntiUpdateComponentStatusScope()) {
				checkBoxRubyAdd.Checked = setting.rubyAdd.enabled;
				groupBoxRubyAdd.Enabled = setting.rubyAdd.enabled;
				checkBoxRubyAddOverwrite.Checked = setting.rubyAdd.isForceAdd;
				foreach ( var kv in radioButtonsForRubyAddType_ ) {
					var rubyType = kv.Key;
					var radioButton = kv.Value;
					radioButton.Checked = rubyType == setting.rubyAdd.rubyType;
				}

				//
				checkBoxTrim.Checked = setting.isTrim;
				checkBoxZenToHan.Checked = setting.isZenToHan;
			}
		}

		RubyAdderOpeData.Setting GetSettingFromUI() {
			var radioButtonsChecked = radioButtonsForRubyAddType_.Where(_v => _v.Value.Checked).ToArray();
			var rubyType = (radioButtonsChecked.Length == 1) ?
				radioButtonsChecked[0].Key :
				RubyAdderOpeData.RubyAdd.Default.rubyType;

			var setting = new RubyAdderOpeData.Setting(
				new RubyAdderOpeData.RubyAdd(
					enabled: checkBoxRubyAdd.Checked,
					isForceAdd: checkBoxRubyAddOverwrite.Checked,
					rubyType: rubyType
				),
				isRubyRemove: checkBoxRubyRemove.Checked,
				isZenToHan: checkBoxZenToHan.Checked,
				isTrim: checkBoxTrim.Checked
			);
			return setting;
		}

        /// <summary>トラックの選択状態に応じて、各種コンポーネントの表示を切り替える.</summary>
        void UpdateComponentStatus()
		{
			var antiUpdateComponentStatusEnabled = 0 < antiUpdateComponentStatusCount_;
			if (antiUpdateComponentStatusEnabled) {
				return;
			}
			int selectedTracksCount;
            if (RubyAdder.TryGetSelectedTracks(rubyAdder, out var tracks))
            {
				selectedTracksCount = tracks.Count;
            }
            else
            {
                selectedTracksCount = 0;
            }
			var setting = GetSettingFromUI();

			var hasSelectedTrack = 0 < selectedTracksCount;
			this.toolStripStatusLabel1.Text = hasSelectedTrack ?
				string.Format(Properties.Resources.StrRootFormStatusBar1, selectedTracksCount) :
				Properties.Resources.StrRootFormStatusBar2;
			this.groupBoxRubyAdd.Enabled = checkBoxRubyAdd.Checked;
			this.buttonApply.Enabled = hasSelectedTrack && setting.HasTask();
			this.存在しないトラックを削除するToolStripMenuItem.Enabled = hasSelectedTrack;

			rubyAdder.opeData.setting = setting;
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
			UpdateComponentStatus();
			StartOperation(RubyAdderOpeType.UpdateTrack);
		}

		private void checkBoxRubyAdd_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxRubyAdd.Checked) {
				using (CreateAntiUpdateComponentStatusScope()) {
					checkBoxRubyRemove.Checked = false;
				}
			}
			UpdateComponentStatus();
		}

		private void checkBoxRubyRemove_CheckedChanged(object sender, EventArgs e) {
			if (checkBoxRubyRemove.Checked) {
				using (CreateAntiUpdateComponentStatusScope()) {
					checkBoxRubyAdd.Checked = false;
				}
			}
			UpdateComponentStatus();
		}

		AntiUpdateComponentStatusScope CreateAntiUpdateComponentStatusScope() {
			return new AntiUpdateComponentStatusScope(this);
		}

		readonly struct AntiUpdateComponentStatusScope : System.IDisposable {
			readonly RootForm parent_;
			public AntiUpdateComponentStatusScope(RootForm parent) {
				parent_ = parent;
				++parent_.antiUpdateComponentStatusCount_;
			}
			public void Dispose() {
				--parent_.antiUpdateComponentStatusCount_;
			}
		}
	}
}
