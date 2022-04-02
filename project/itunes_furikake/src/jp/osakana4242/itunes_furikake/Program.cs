using jp.osakana4242.core.LogOperator;
using jp.osakana4242.itunes_furikake.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;

namespace jp.osakana4242.itunes_furikake {
	static class Program {

		static TraceSource logger = LogOperator.get();


		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main() {
			Mutex mutex = null;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			FormsTimerScheduler.Init();

			Observable.Return(0).
			ObserveOn(FormsTimerScheduler.Instance).
			Do(_ => {
				// 多重起動確認.
				mutex = new System.Threading.Mutex(true, "jp.osakana4242.itunes_furikake", out bool createdNew);
				if (createdNew == false) {
					mutex.Dispose();
					mutex = null;
					throw new AppDisplayableException(Resources.StrErrAppDuplicate);
				}
			}).
			SelectMany(_ => {
				// 起動準備.
				// 進捗を表示する.
				ProgressDialog.Config config = new ProgressDialog.Config();

				return ProgressDialog.ShowDialogAsync(null, config, 0, (_bw, _evtArgs, _prm) => {
					var progress = 0;
					var progressTotal = 2;
					ProgressDialogState.ReportWithTitle(_bw, new ProgressPair(progress, progressTotal), Resources.StrStartupFormTitle);
					ProgressDialogState.Report(_bw, new ProgressPair(progress, progressTotal), "");
					ProgressResult result = new ProgressResult();
					_evtArgs.Result = result;
					bool isEndStream = false;
					var stream = Observable.Return(0).
					Do(_2 => {
						CleanFiles();
						logger.TraceEvent(TraceEventType.Information, 0, "itunes_furikake init.");
						++progress;
						ProgressDialogState.Report(_bw, new ProgressPair(progress, progressTotal));
					}).
					ObserveOn(FormsTimerScheduler.Instance).
					Select(_2 => {
						var form = new RootForm();
						++progress;
						ProgressDialogState.Report(_bw, new ProgressPair(progress, progressTotal));
						return form;
					}).
					Delay(System.TimeSpan.FromSeconds(0.5f), FormsTimerScheduler.Instance).
					Finally(() => {
						isEndStream = true;
					});

					System.Exception ex2 = null;
					var disposer = stream.Subscribe(_result => {
						result.result = _result;
					}, _ex => {
						ex2 = _ex;
					});

					while (!isEndStream) {
						if (_bw.CancellationPending) {
							disposer.Dispose();
							throw CancelException.Instance;
						}
					}
					if (null != ex2) {
						throw ex2;
					}
				});
			}).
			Select(_result => (RootForm)_result.result).
			SelectMany(_form => {
				_form.Show();
				return _form.OnDestroyedAsObservableExt().
				Select(_ => Unit.Default).
				Take(1);
			}).Catch((System.Exception _ex) => {
				ErrorDialog.TryShowException(_ex);
				return Observable.Return(Unit.Default);
			}).
			Finally(() => {
				Application.Exit();
				if (null == mutex) return;

				logger.TraceEvent(TraceEventType.Information, 0, "itunes_furikake exit.");
				logger.Close();
				mutex.ReleaseMutex();
				mutex.Dispose();
			}).
			Subscribe();

			try {
				Application.Run();
			} catch (System.Exception ex) {
				ErrorDialog.TryShowException(ex);
			}
		}

		static void CleanFiles() {
			// テキストログの削除.
			string dirPath = "log";
			if (!Directory.Exists(dirPath)) {
				try {
					Directory.CreateDirectory(dirPath);
				} catch (Exception ex) {
					logger.TraceEvent(TraceEventType.Error, 0, dirPath + " can't create. " + ex.Message);
				}
			}

			foreach (string path in Directory.GetFiles("log", "*.log")) {
				try {
					File.Delete(path);
				} catch (Exception ex) {
					logger.TraceEvent(TraceEventType.Error, 0, path + " can't delete. " + ex.Message);
				}
			}
		}
	}
}
