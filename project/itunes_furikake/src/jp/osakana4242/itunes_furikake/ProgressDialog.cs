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
    // 進捗状況を表示するダイアログ。
    public partial class ProgressDialog : Form
    {
        private object workArg;
        private ProgressResult result;
        private RootForm rootForm;
        System.Action<ProgressResult> onCompleted;

        public ProgressDialog(RootForm rootForm, DoWorkEventHandler work, object workArg, System.Action<ProgressResult> onCompleted)
        {
            InitializeComponent();
            this.backgroundWorker1.DoWork += work;
            this.workArg = workArg;
            this.label1.Text = "";
            this.rootForm = rootForm;
            this.onCompleted = onCompleted;

        }

        public void SetProgressParams(string label, int value, int minValue, int maxValue)
        {
            this.label1.Text = label;
            this.progressBar1.Minimum = minValue;
            this.progressBar1.Maximum = maxValue;
            this.progressBar1.Value = value;
        }

        // 中断。
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Value = e.ProgressPercentage;

            if (e.UserState == null) return;
            if (!(e.UserState is ProgressDialogState state)) return;

            if (state.Text != null)
            {
                this.label1.Text = state.Text;
            }

            if (state.Log != null)
            {
                this.rootForm.AddLog(state.Log);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.result = (ProgressResult)e.Result;
            this.Close();
        }

        private void ProgressDialog_Shown(object sender, EventArgs e)
        {
            this.backgroundWorker1.RunWorkerAsync(this.workArg);
        }

        private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.backgroundWorker1.CancelAsync();
            if (this.backgroundWorker1.IsBusy)
            {
                // 処理中なので閉じられないよ！
                e.Cancel = true;
            }
        }

        private void ProgressDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            onCompleted(this.result);
            this.Owner.Enabled = true;
        }

        private void ProgressDialog_Load(object sender, EventArgs e)
        {

        }

    }

    public sealed class ProgressDialogState
    {
        public readonly string Text = "";
        public readonly string Log = "";
        public ProgressDialogState(string text, string log)
        {
            this.Text = text;
            this.Log = log;
        }
    }

    public sealed class ProgressResult
    {
        public Exception ex;
        public string title;
        public string message;
        public TrackID[] trackIDList = { };
    }
}
