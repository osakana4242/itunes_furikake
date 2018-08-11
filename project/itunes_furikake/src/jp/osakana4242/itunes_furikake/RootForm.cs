using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTunesLib;

namespace jp.osakana4242.itunes_furikake
{
    public partial class RootForm : Form
    {
        private RubyAdder rubyAdder;
        public RootForm(RubyAdder rubyAdder)
        {
            InitializeComponent();
            this.rubyAdder = rubyAdder;
            this.rubyAdder.setLogger(this.addLog);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // ボタン：ひらがな.
        private void button1_Click(object sender, EventArgs e)
        {
            this.exec(RubyAdder.OPE.HIRAGANA);
        }

        // ボタン：カタカナ.
        private void button5_Click(object sender, EventArgs e)
        {
            this.exec(RubyAdder.OPE.KATAKANA);
        }

        // ボタン：アルファベット.
        private void button2_Click(object sender, EventArgs e)
        {
            this.exec(RubyAdder.OPE.ALPHABET);
        }

        // ボタン：クリア.
        private void button3_Click(object sender, EventArgs e)
        {
            this.exec(RubyAdder.OPE.CLEAR);
        }

        // ボタン：バージョン情報の表示.
        private void button4_Click(object sender, EventArgs e)
        {
            VersionForm versionForm = new VersionForm();
            versionForm.ShowDialog(this);
            versionForm.Dispose();
        }

        private void exec(RubyAdder.OPE ope)
        {
            console.Clear();
            this.Enabled = false;
            ProgressDialog progressDialog = new ProgressDialog(this, this.rubyAdder.getDoWorkEventHandler(), ope);
            progressDialog.Text = jp.osakana4242.itunes_furikake.Properties.Resources.StrExecuting;
            progressDialog.Show(this);
//            this.rubyAdder.Exec(ope);
        }

        // 強制書き換えチェックボックス。
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.rubyAdder.IsForceAdd = this.checkBox1.Checked;
        }

        // ユーザに見せるログを追加.
        public void addLog(string str)
        {
            this.console.AppendText(str);
        }

        private void バージョン情報ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VersionForm versionForm = new VersionForm();
            versionForm.ShowDialog(this);
            versionForm.Dispose();
        }

        private void 全角英数を半角にするToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exec(RubyAdder.OPE.ZEN2HAN);
        }
    }


}
