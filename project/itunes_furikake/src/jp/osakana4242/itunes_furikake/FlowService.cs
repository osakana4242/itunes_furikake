
using iTunesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using jp.osakana4242.core.LogOperator;

namespace jp.osakana4242.itunes_furikake.src.jp.osakana4242.itunes_furikake
{

	/// <summary>ダイアログ表示して、処理してのいろんな流れ</summary>
	public static class FlowService
	{
		public static TraceSource logger = LogOperator.get();
		static readonly string BR = System.Environment.NewLine;

		public static void UpdateTrackFlow(RootForm owner)
		{

			FlowService.ShowProgressDialog(owner, owner, (_bw, _evtArgs, _owner) =>
			{
				UpdateTrackWithProgress(owner.rubyAdder, _bw, _evtArgs);
			},
			(_result1) =>
			{
				if (_result1 != null && _result1.message != null)
				{
					MessageBox.Show(owner, _result1.message, _result1.title);
				}
			});
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
				try
				{
					// 1トラック分の処理.
					string trackName = null;
					sb.Clear();
					try
					{
						trackName = trackBase.Name;
					}
					catch (Exception ex)
					{
						logger.TraceEvent(TraceEventType.Error, 0, "トラック名が取得出来ません. ex:" + ex.Message);
					}
					if (!(trackBase is IITFileOrCDTrack track)) continue;

					bgWorker.ReportProgress(NumberHelper.Percent(endTrackNum, targetTrackNum), new ProgressDialogState(null, string.Format("{0}:{1}{2}", endTrackNum + 1, trackName == null ? "-" : trackName, BR)));
					RubyAdder.ExecTrack(rubyAdder, track, sb);
				}
				catch (Exception ex)
				{
					// なんかエラー
					logger.TraceEvent(TraceEventType.Error, 0, "トラックエラー. ex:" + ex.Message);
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
						bgWorker.ReportProgress(NumberHelper.Percent(endTrackNum, targetTrackNum), new ProgressDialogState(String.Format("{0}/{1}", endTrackNum, targetTrackNum), sb.ToString()));
					}
				}
			}


			string log = BR
				+ "エラートラック数: " + errorTrackNum + BR
				+ "総トラック数: " + endTrackNum + BR
				;
			bgWorker.ReportProgress(NumberHelper.Percent(endTrackNum, targetTrackNum), new ProgressDialogState(null, log));

			System.Threading.Thread.Sleep(250);

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
			FlowService.ShowProgressDialog(owner, owner, (_bw, _evtArgs, _this) =>
			{
				FlowService.ListupDeleteTrackWithProgress(opeData, _bw, _evtArgs, iTunesApp);
			},
			_result1 =>
			{
				if (_result1 != null && _result1.message != null)
				{
					MessageBox.Show(owner, _result1.message, _result1.title);
				}
				if (_result1.trackIDList.Length <= 0) return;


				if (opeData.isNeedConfirmation)
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
			FlowService.ShowProgressDialog(owner, owner, (_bw, _evtArgs, _owner) =>
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
				string trackName = "?";
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
					{
						string n = track.Name;
						if (string.IsNullOrEmpty(n))
						{
							trackName = n;
						}
					}

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
					logger.TraceEvent(TraceEventType.Error, 0, $"トラックエラー. トラック名: {trackName}, ex: {ex.Message}");
					exceptionList.Add(ex);
					errorTrackNum += 1;
				}
				finally
				{
					++opeData.progress;
					bgWorker.ReportProgress(
						NumberHelper.Percent(opeData.progress, opeData.total),
						new ProgressDialogState(string.Format(Properties.Resources.StrDeleteCheckProgress1, opeData.progress, opeData.total), null)
						);
				}
			}

			result.trackIDList = trackIDList.ToArray();

			if (trackIDList.Count <= 0)
			{
				bgWorker.ReportProgress(
					NumberHelper.Percent(opeData.progress, opeData.total),
					new ProgressDialogState(
						null,
						string.Format(Properties.Resources.StrDeleteCheckLog1, tracks.Count) + BR
					)
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
			var result = new ProgressResult();
			evtArgs.Result = result;
			var exceptionList = new List<Exception>();
			int errorTrackNum = 0;
			int endTrackNum = 0;
			for (int i = 0, iCount = trackIDList.Length; i < iCount; ++i)
			{
				if (bw.CancellationPending) return;
				TrackID trackID = trackIDList[i];
				string trackName = $"{i + 1}";
				try
				{
					IITTrack track = tracks.GetItemByTrackID_ext(trackID);
					trackName = $"{i + 1}: {track.Name}";
					track.Delete();
					bw.ReportProgress(
						NumberHelper.Percent(opeData.progress, opeData.total),
						new ProgressDialogState(null, string.Format(Properties.Resources.StrDeleteOK, i + 1, trackName) + BR)
					);
					++endTrackNum;
				}
				catch (System.Exception ex)
				{
					exceptionList.Add(ex);
					bw.ReportProgress(
						NumberHelper.Percent(opeData.progress, opeData.total),
						new ProgressDialogState(null, string.Format(Properties.Resources.StrDeleteNG, i + 1, trackName) + BR)
					);
					++errorTrackNum;
				}
				finally
				{
					++opeData.progress;
					bw.ReportProgress(
						NumberHelper.Percent(opeData.progress, opeData.total),
						new ProgressDialogState(string.Format(Properties.Resources.StrDeleteProgress1, opeData.progress, opeData.total), null)
					);
				}
			}

			string log = BR
				+ "エラートラック数: " + errorTrackNum + BR
				+ "削除したトラック数: " + endTrackNum + BR
				;
			bw.ReportProgress(
				NumberHelper.Percent(opeData.progress, opeData.total),
				new ProgressDialogState(null, log)
			);

			System.Threading.Thread.Sleep(1000);

			if (0 < exceptionList.Count)
			{
				result.title = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrAppName;
				result.message = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrErrTrack2;
			}
		}

		public static void ShowOkCancelDialog(IWin32Window owner, string title, string body, System.Action<OkCancelDialog.Result> onCompleted)
		{
			using (var dialog = new OkCancelDialog(title, body, onCompleted))
			{
				dialog.ShowDialog(owner);
			}
		}

		public static void ShowProgressDialog<T>(RootForm root, T prm, System.Action<BackgroundWorker, DoWorkEventArgs, T> onProgress, System.Action<ProgressResult> onCompleted)
		{
			ProgressDialog progressDialog = new ProgressDialog(root, (_sender, _e) =>
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

			}, prm, onCompleted);
			progressDialog.Text = Properties.Resources.StrExecuting;
			progressDialog.ShowDialog(root);
		}
	}
}
