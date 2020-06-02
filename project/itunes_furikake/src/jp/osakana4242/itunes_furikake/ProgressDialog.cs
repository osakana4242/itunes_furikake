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
        string titleTextLeft = "";
        string bodyTextLeft = "";

        public ProgressDialog(RootForm rootForm, Config config, DoWorkEventHandler work, object workArg, System.Action<ProgressResult> onCompleted)
        {
            InitializeComponent();
            this.backgroundWorker1.DoWork += work;
            this.workArg = workArg;
            this.label1.Text = "";
            this.rootForm = rootForm;
            this.onCompleted = onCompleted;
            this.checkBox1.Visible = !string.IsNullOrEmpty( config.checkboxLabel );
            this.checkBox1.Text = config.checkboxLabel;
            this.checkBox1.Checked = config.checkboxChecked;

            this.titleTextLeft = Properties.Resources.StrExecuting;
            this.Text = this.titleTextLeft;
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

            if (!(e.UserState is ProgressDialogState state))
            {
                this.progressBar1.Maximum = 100;
                this.progressBar1.Value = e.ProgressPercentage;
                return;
            }

            this.progressBar1.Maximum = state.total;
            this.progressBar1.Value = state.progress;

            if (state.Title != null)
            {
                this.titleTextLeft = state.Title;
            }

			float percentage = (state.progress * 100f / state.total);
			this.Text = string.Format("{0} - {1:F2}%",this.titleTextLeft, percentage);

            this.label1.Text = string.Format("{0} / {1}", state.progress, state.total);
			if (state.Text != null)
            {
                this.bodyTextLeft = state.Text;
                this.label2.Text = this.bodyTextLeft;
            }

            if (state.Log != null)
            {
                this.rootForm.AddLog(state.Log);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.result = (ProgressResult)e.Result;
            this.result.isNeedConfirm = checkBox1.Checked;
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
            //this.Owner.Enabled = true;
            //this.onCompleted(this.result);
            FlowService.Delay(Tuple.Create(this.Owner, this.onCompleted, this.result), _prm => {
                _prm.Item1.Enabled = true;
                _prm.Item2(_prm.Item3);
            });
        }

        private void ProgressDialog_Load(object sender, EventArgs e)
        {

        }

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{

		}

        public struct Config
        {
            public string checkboxLabel;
            public bool checkboxChecked;
        }
    }

    public sealed class ProgressDialogState
    {
        public readonly int progress;
        public readonly int total;
        public readonly string Title = null;
        public readonly string Text = null;
        public readonly string Log = null;
        public ProgressDialogState(int progress, int total, string title, string text, string log)
        {
            this.progress = progress;
            this.total = total;
            this.Title = title;
            this.Text = text;
            this.Log = log;
        }

        public static void ReportWithTitle(BackgroundWorker bw, int progress, int total, string title = null)
        {
            var self = new ProgressDialogState(progress, total, title, null, null);
            bw.ReportProgress(progress * 100 / total, self);
        }

        public static void Report(BackgroundWorker bw, int progress, int total, string text = null, string log = null)
        {
            var self = new ProgressDialogState(progress, total, null, text, log);
            bw.ReportProgress( progress * 100 / total, self);
        }
    }

    public sealed class ProgressResult
    {
        public Exception ex;
        public string title;
        public string message;
        public bool isNeedConfirm;
        public TrackID[] trackIDList = { };
    }
}
