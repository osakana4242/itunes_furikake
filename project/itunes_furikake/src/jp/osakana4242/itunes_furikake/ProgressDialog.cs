using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace jp.osakana4242.itunes_furikake {
	// 進捗状況を表示するダイアログ。
	public partial class ProgressDialog : Form {
		public static DateTimeOffset lastPaintTime;
		public static Thread mainThread_;

		object workArg;
		ProgressResult result;
		RootForm rootForm;
		System.Action<ProgressResult> onCompleted;
		string titleTextLeft = "";
		string bodyTextLeft = "";
		Timer timer_ = new Timer();


		public static IObservable<T> ShowDialogAsync<T>(RootForm root, ProgressDialog.Config config, IObservable<T> stream) {
			System.Action<BackgroundWorker, DoWorkEventArgs, int> onProgress = (_bw, _evtArgs, _prm) => {
				var isEnd = false;
				stream.Subscribe(_result => {
					var pr = new ProgressResult();
					pr.result = _result;
					_evtArgs.Result = pr;
					isEnd = true;
				});
				while (!isEnd) {
					Thread.Sleep(100);
				}
			};

			return Observable.Create<T>(_obs => {
				ShowDialog(root, config, 0, onProgress, _result => {
					_obs.OnNext((T)_result.result);
					_obs.OnCompleted();
				}, _ex => {
					_obs.OnError(_ex);
				});

				return Disposable.Empty;
			});
		}

		public static IObservable<ProgressResult> ShowDialogAsync<T>(RootForm root, ProgressDialog.Config config, T prm, System.Action<BackgroundWorker, DoWorkEventArgs, T> onProgress) {
			return Observable.Create<ProgressResult>(_obs => {
				ShowDialog(root, config, prm, onProgress, _result => {
					_obs.OnNext(_result);
					_obs.OnCompleted();
				}, _ex => {
					_obs.OnError(_ex);
				});

				return Disposable.Empty;
			});
		}

		public static void ShowDialog<T>(RootForm root, ProgressDialog.Config config, T prm, System.Action<BackgroundWorker, DoWorkEventArgs, T> onProgress, System.Action<ProgressResult> onCompleted, System.Action<System.Exception> onError) {
			ProgressDialog progressDialog = null;
			System.Exception ex = null;
			progressDialog = new ProgressDialog(root, config,
				(_sender, _e) => {
					// ここは別スレッド処理.
					try {
						onProgress((BackgroundWorker)_sender, _e, (T)_e.Argument);
					} catch (Exception ex2) {
						// メインスレッドで通知したい.
						ex = ex2;
					}
				},
				prm,
				_result => {
					if (null != ex) {
						onError(ex);
						ex = null;
					} else {
						onCompleted(_result);
					}
				}
			);
			progressDialog.ShowDialog(root);
		}


		public ProgressDialog(RootForm rootForm, Config config, DoWorkEventHandler work, object workArg, System.Action<ProgressResult> onCompleted) {
			InitializeComponent();
			this.backgroundWorker1.DoWork += work;
			this.workArg = workArg;
			this.label1.Text = "";
			this.label2.Text = "";
			this.rootForm = rootForm;
			this.onCompleted = onCompleted;
			this.checkBox1.Visible = !string.IsNullOrEmpty(config.checkboxLabel);
			this.checkBox1.Text = config.checkboxLabel;
			this.checkBox1.Checked = config.checkboxChecked;

			this.titleTextLeft = Properties.Resources.StrExecuting;
			this.Text = this.titleTextLeft;
			timer_.Enabled = true;
			timer_.Interval = 100;
			timer_.Tick += (_a, _b) => {
				if (IsNeedReflesh) {
					this.Refresh();
				}
			};

			this.Paint += (_a, _b) => {
				lastPaintTime = DateTimeOffset.Now;
			};
		}

		public void SetProgressParams(string label, int value, int minValue, int maxValue) {
			this.label1.Text = label;
			this.progressBar1.Minimum = minValue;
			this.progressBar1.Maximum = maxValue;
			this.progressBar1.Value = value;
		}

		// 中断。
		private void button1_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			this.progressBar1.Minimum = 0;

			if (!(e.UserState is ProgressDialogState state)) {
				this.progressBar1.Maximum = 100;
				this.progressBar1.Value = e.ProgressPercentage;
				return;
			}

			this.progressBar1.Maximum = (int)(state.Progress.total * 100);
			this.progressBar1.Value = (int)(state.Progress.value * 100);

			if (state.Title != null) {
				this.titleTextLeft = state.Title;
			}

			this.Text = string.Format("{0} - {1:F2}%", this.titleTextLeft, state.Progress.Percentage());

			this.label1.Text = string.Format("{0} / {1}", (int)state.Progress.value, (int)state.Progress.total);
			if (state.Text != null) {
				this.bodyTextLeft = state.Text;
				this.label2.Text = this.bodyTextLeft;
			}

			if (state.Log != null) {
				this.rootForm.AddLog(state.Log);
			}
			if (IsNeedReflesh) {
				this.Refresh();
			}
		}

		bool IsNeedReflesh => lastPaintTime + TimeSpan.FromSeconds(0.1f) <= DateTimeOffset.Now;

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			this.result = e.Result as ProgressResult;
			if (null != this.result) {
				this.result.isNeedConfirm = checkBox1.Checked;
			}
			this.Close();
		}

		private void ProgressDialog_Shown(object sender, EventArgs e) {
			mainThread_ = Thread.CurrentThread;
			this.timer_.Start();
			this.backgroundWorker1.RunWorkerAsync(this.workArg);
		}

		private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e) {
			this.backgroundWorker1.CancelAsync();
			if (this.backgroundWorker1.IsBusy) {
				// 処理中なので閉じられないよ！
				e.Cancel = true;
			}
		}

		private void ProgressDialog_FormClosed(object sender, FormClosedEventArgs e) {
			this.timer_.Stop();
			//this.Owner.Enabled = true;
			//this.onCompleted(this.result);
			FlowService.Delay(ValueTuple.Create(this.Owner, this.onCompleted, this.result), _prm => {
				if (null != _prm.Item1) {
					_prm.Item1.Enabled = true;
				}
				_prm.Item2(_prm.Item3);
			});
		}

		public struct Config {
			public string checkboxLabel;
			public bool checkboxChecked;
		}
	}
}
