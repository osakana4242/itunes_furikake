
using iTunesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using jp.osakana4242.core.LogOperator;
using System.Threading.Tasks;

namespace jp.osakana4242.itunes_furikake
{

	/// <summary>ダイアログ表示して、処理してのいろんな流れ</summary>
	public static class FlowService
	{
		public static TraceSource logger = LogOperator.get();
		static readonly string BR = System.Environment.NewLine;

		public static void UpdateTrackFlow(RootForm owner)
		{
			var config = new ProgressDialog.Config();
			FlowService.ShowProgressDialog(owner, config, owner, (_bw, _evtArgs, _owner) =>
			{
				UpdateTrackWithProgress(owner.rubyAdder, _bw, _evtArgs);
			},
			(_result1) =>
			{
				if (_result1 != null && _result1.message != null)
				{
					OkCancelDialog.ShowOK(owner, _result1.title, _result1.message);
				}
			});
		}

		static string getTrackNameSafe(IITTrack trackBase)
		{
			try
			{
				return trackBase.Name;
			}
			catch (Exception ex)
			{
				logger.TraceEvent(TraceEventType.Error, 0, "トラック名が取得出来ません. ex:" + ex.Message);
				return "-";
			}
		}

		/// <summary>フリガナ処理とプログレスバーの進行</summary>
		public static void UpdateTrackWithProgress(RubyAdder rubyAdder, BackgroundWorker bgWorker, DoWorkEventArgs e)
		{
			ProgressResult result = new ProgressResult();
			List<Exception> exceptionList = new List<Exception>();

			if (e != null)
			{
				e.Result = result;
			}
			IITTrackCollection tracks = rubyAdder.iTunesApp.SelectedTracks;
			if (tracks == null)
			{
				result.title = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrAppName;
				result.message = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrErrTrack1;
				return;
			}
			int endTrackNum = 0;
			int errorTrackNum = 0;
			int targetTrackNum = tracks.Count;
			StringBuilder sb = new StringBuilder();
			// int targetFieldNum = tracks.Count * TrackFieldNames.Length;

			foreach (IITTrack trackBase in tracks)
			{
				// 中断.
				if (bgWorker.CancellationPending) return;
				// 1トラック分の処理.
				string trackNameForDisplay = getTrackNameSafe(trackBase);
				try
				{
					sb.Clear();
					if (!(trackBase is IITFileOrCDTrack track)) continue;
					ProgressDialogState.Report(bgWorker, endTrackNum, targetTrackNum, trackNameForDisplay, string.Format("{0}:{1}{2}", endTrackNum + 1, trackNameForDisplay, BR));
					RubyAdder.ExecTrack(rubyAdder, track, sb);
				}
				catch (Exception ex)
				{
					// なんかエラー
					logger.TraceEvent(TraceEventType.Error, 0, $"トラックエラー. トラック名: {trackNameForDisplay}, ex: {ex.Message}");
					exceptionList.Add(ex);
					sb.Append("トラックを編集出来ませんでした(スキップ)").Append(BR);
					errorTrackNum += 1;
				}
				finally
				{
					endTrackNum += 1;
					if (bgWorker != null)
					{
						//ProgressChangedイベントハンドラを呼び出し、
						//コントロールの表示を変更する
						ProgressDialogState.Report(bgWorker, endTrackNum, targetTrackNum, null, sb.ToString());
					}
				}
			}


			string log = BR
				+ "エラートラック数: " + errorTrackNum + BR
				+ "総トラック数: " + endTrackNum + BR
				;
			ProgressDialogState.Report(bgWorker, endTrackNum, targetTrackNum, null, log);

			System.Threading.Thread.Sleep(1000);

			if (0 < exceptionList.Count)
			{
				result.title = Properties.Resources.StrAppName;
				result.message = Properties.Resources.StrErrTrack2;
			}
		}

		/// <summary>
		/// 削除対象のリストアップ
		/// 削除の確認
		/// 削除
		/// </summary>
		public static void DeleteTrackFlow(RootForm owner, RubyAdderOpeData opeData, iTunesApp iTunesApp)
		{
			// プログレスバーを表示して、削除対象をリストアップする
			var config = new ProgressDialog.Config()
			{
				checkboxChecked = true,
				// checkboxLabel = Properties.Resources.StrDeleteProgressCheckbox,
			};
			FlowService.ShowProgressDialog(owner, config, owner, (_bw, _evtArgs, _this) =>
			{
				FlowService.ListupDeleteTrackWithProgress(opeData, _bw, _evtArgs, iTunesApp);
			},
			_result1 =>
			{
				if (_result1 != null && _result1.message != null)
				{
					OkCancelDialog.ShowOK(owner, _result1.title, _result1.message);
				}
				if (_result1.trackIDList.Length <= 0) return;

				if (_result1.isNeedConfirm)
				{
					// 確認ダイアログの表示.
					FlowService.ShowDeleteConfirmDialog(owner, _result1.trackIDList, iTunesApp.SelectedTracks, _result2 =>
					{
						if (_result2 == OkCancelDialog.Result.Cancel)
						{
							return;
						}
						DeleteTrackAsync(owner, _result1.trackIDList);
					});
				}
				else
				{
					DeleteTrackAsync(owner, _result1.trackIDList);
				}
			});
		}

		/// <summary>プログレスバーを表示して削除処理</summary>
		public static void DeleteTrackAsync(RootForm owner, TrackID[] trackIDList)
		{
			var config = new ProgressDialog.Config();
			FlowService.ShowProgressDialog(owner, config, owner, (_bw, _evtArgs, _owner) =>
			{
				FlowService.DeleteTrackWithProgress(_owner.rubyAdder.opeData, _bw, _evtArgs, trackIDList, _owner.rubyAdder.iTunesApp.SelectedTracks);
			}, _r =>
			{
				// 完了.
			});
		}

		public static void ShowDeleteConfirmDialog(IWin32Window owner, TrackID[] trackIDList, IITTrackCollection tracks, System.Action<OkCancelDialog.Result> onCompleted)
		{
			string title = Properties.Resources.StrDeleteConfirmForm1;
			string body;
			{
				var sb = new System.Text.StringBuilder();
				sb.Append(string.Format(Properties.Resources.StrDeleteConfirmForm2, trackIDList.Length)).
					Append(BR).
					Append(BR);

				for (int i = 0, iCount = trackIDList.Length; i < iCount; ++i)
				{
					var trackID = trackIDList[i];
					var track = tracks.GetItemByTrackID_ext(trackID);
					sb.Append(string.Format(Properties.Resources.StrDeleteConfirmForm3, i + 1, track.Name) + BR);
				}

				body = sb.ToString();

			}
			FlowService.ShowOkCancelDialog(owner, title, body, onCompleted);
		}

		/// <summary>削除対象のトラックをリストアップ, プログレスバーの進行</summary>
		public static void ListupDeleteTrackWithProgress(RubyAdderOpeData opeData, BackgroundWorker bgWorker, DoWorkEventArgs evtArgs, iTunesApp iTunesApp)
		{
			var result = new ProgressResult();
			evtArgs.Result = result;
			var exceptionList = new List<Exception>();
			IITTrackCollection tracks = iTunesApp.SelectedTracks;
			if (tracks == null)
			{
				result.title = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrAppName;
				result.message = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrErrTrack1;
				return;
			}

			// 工程数を求める.
			// リストアップと削除の2工程.
			opeData.total += tracks.Count * 2;
			int errorTrackNum = 0;
			ProgressDialogState.ReportWithTitle(bgWorker, opeData.progress, opeData.total, Properties.Resources.StrDeleteCheckProgress1);

			/// 素直に foreach で Delete すると、
			/// Delete を実行した数だけ要素がすっとばされてしまう.
			/// なので、IDリスト経由してイテレートする.
			/// 
			/// 例:
			/// トラック 01
			/// トラック 02
			/// トラック 03
			/// トラック 04
			/// 
			/// に対して、以下の foreach を回すと
			/// 
			/// トラック 02
			/// トラック 04
			/// 
			/// がスキップされる.
			/// 
			///  foreach (IITTrack track in tracks)
			///  {
			///      track.Delete();
			///  }

			// IDリストを作成する.
			var trackIDList = new List<TrackID>();
			foreach (IITTrack trackBase in tracks)
			{
				string trackNameForDisplay = getTrackNameSafe(trackBase);
				try
				{
					if (bgWorker.CancellationPending) return;


					if (!(trackBase is IITFileOrCDTrack track))
					{
						++opeData.progress;
						continue;
					}

					Console.WriteLine($"{ track.Enabled }");
					var path = track.Location;
					var exists = System.IO.File.Exists(path);

					if (exists)
					{
						++opeData.progress;
						continue;
					}

					TrackID trackID = iTunesApp.GetTrackID_ext(trackBase);
					trackIDList.Add(trackID);
				}
				catch (Exception ex)
				{
					logger.TraceEvent(TraceEventType.Error, 0, $"トラックエラー. トラック名: {trackNameForDisplay}, ex: {ex.Message}");
					exceptionList.Add(ex);
					errorTrackNum += 1;
				}
				finally
				{
					++opeData.progress;
					ProgressDialogState.Report(bgWorker, opeData.progress, opeData.total, trackNameForDisplay, null);
				}
			}

			result.trackIDList = trackIDList.ToArray();

			if (trackIDList.Count <= 0)
			{
				ProgressDialogState.Report(bgWorker, opeData.progress, opeData.total,
					null,
					string.Format(Properties.Resources.StrDeleteCheckLog1, tracks.Count) + BR
				);
			}

			System.Threading.Thread.Sleep(1000);

			if (0 < exceptionList.Count)
			{
				result.title = Properties.Resources.StrAppName;
				result.message = Properties.Resources.StrErrTrack2;
			}
		}

		/// <summary>削除処理とプログレスバーの進行</summary>
		public static void DeleteTrackWithProgress(RubyAdderOpeData opeData, BackgroundWorker bw, DoWorkEventArgs evtArgs, TrackID[] trackIDList, IITTrackCollection tracks)
		{
			ProgressDialogState.ReportWithTitle(bw, opeData.progress, opeData.total, Properties.Resources.StrDeleteProgress1);
			var result = new ProgressResult();
			evtArgs.Result = result;
			var exceptionList = new List<Exception>();
			int errorTrackNum = 0;
			int endTrackNum = 0;
			for (int i = 0, iCount = trackIDList.Length; i < iCount; ++i)
			{
				if (bw.CancellationPending) return;
				TrackID trackID = trackIDList[i];
				string trackNameForDisplay = "-";
				try
				{
					IITTrack track = tracks.GetItemByTrackID_ext(trackID);
					trackNameForDisplay = getTrackNameSafe(track);
					track.Delete();
					ProgressDialogState.Report(bw, opeData.progress, opeData.total,
						trackNameForDisplay,
						 string.Format(Properties.Resources.StrDeleteOK, i + 1, trackNameForDisplay) + BR
					);
					++endTrackNum;
				}
				catch (System.Exception ex)
				{
					exceptionList.Add(ex);
					ProgressDialogState.Report(bw, opeData.progress, opeData.total,
						trackNameForDisplay,
						string.Format(Properties.Resources.StrDeleteNG, i + 1, trackNameForDisplay) + BR
					);
					++errorTrackNum;
				}
				finally
				{
					++opeData.progress;
					ProgressDialogState.Report(bw, opeData.progress, opeData.total,
						trackNameForDisplay,
						null
					);
				}
			}

			string log = BR
				+ "エラートラック数: " + errorTrackNum + BR
				+ "削除したトラック数: " + endTrackNum + BR
				;
			ProgressDialogState.Report(bw, opeData.progress, opeData.total, null, log);

			System.Threading.Thread.Sleep(1000);

			if (0 < exceptionList.Count)
			{
				result.title = Properties.Resources.StrAppName;
				result.message = Properties.Resources.StrErrTrack2;
			}
		}

		public static void ShowOkCancelDialog(IWin32Window owner, string title, string body, System.Action<OkCancelDialog.Result> onCompleted)
		{
			using (var dialog = new OkCancelDialog(OkCancelDialog.Type.OKCancel, title, body, onCompleted))
			{
				dialog.ShowDialog(owner);
			}
		}

		public static void ShowProgressDialog<T>(RootForm root, ProgressDialog.Config config, T prm, System.Action<BackgroundWorker, DoWorkEventArgs, T> onProgress, System.Action<ProgressResult> onCompleted)
		{
			ProgressDialog progressDialog = null;
			progressDialog = new ProgressDialog(root, config, (_sender, _e) =>
			{
				try
				{
					onProgress((BackgroundWorker)_sender, _e, (T)_e.Argument);
				}
				catch (Exception ex)
				{
					RubyAdder.logger.TraceEvent(TraceEventType.Error, 0, "不昧なエラー. ex:" + ex.Message);
					var result = new ProgressResult();
					result.title = Properties.Resources.StrAppName;
					result.message = Properties.Resources.StrErrUnknown;
					_e.Result = result;
				}

			}, prm, _r => {
//				progressDialog.Dispose();
				onCompleted(_r);
			});
			progressDialog.ShowDialog(root);
		}

		public static void Delay<T>(T prm, System.Action<T> func)
		{
			var t = new System.Windows.Forms.Timer();
			t.Interval = 200;
			t.Tick += (_a, _b) =>
			{
				t.Dispose();
				func(prm);
			};
			t.Enabled = true;
			t.Start();
		}
	}
}
