
using iTunesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using jp.osakana4242.core.LogOperator;
using jp.osakana4242.itunes_furikake.Properties;
using System.Threading;

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
			ProgressDialog.ShowDialogAsync(owner, config, owner, (_bw, _evtArgs, _owner) =>
				{
					UpdateTrackWithProgress(owner.rubyAdder, _bw, _evtArgs);
				}).
				Subscribe((_result1) =>
				{
					if (_result1 != null && _result1.errorMessage != null)
					{
						OkCancelDialog.ShowOK(owner, "", _result1.errorMessage);
					}
				}, _ex =>
				{
					ErrorDialog.ShowUnknown(owner, _ex);
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
				result.errorMessage = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrErrTrack1;
				return;
			}
			int endTrackNum = 0;
			int errorTrackNum = 0;
			int targetTrackNum = tracks.Count;
			StringBuilder sb = new StringBuilder();
			// int targetFieldNum = tracks.Count * TrackFieldNames.Length;
			DateTimeOffset nextSleep = DateTimeOffset.Now + TimeSpan.FromSeconds(1f);
			string trackNameForLog = "";

			string trackNameForDisplay = "";

			foreach (IITTrack trackBase in tracks)
			{
				// 中断.
				if (bgWorker.CancellationPending) throw new CancelException();
				// 1トラック分の処理.
				trackNameForDisplay = getTrackNameSafe(trackBase);
				try
				{
					sb.Clear();
					if (!(trackBase is IITFileOrCDTrack track)) continue;
					trackNameForLog = string.Format("{0}:{1}{2}", endTrackNum + 1, trackNameForDisplay, BR);
					ProgressDialogState.Report(bgWorker, new ProgressPair(endTrackNum, targetTrackNum), trackNameForDisplay);
					var prm = (bgWorker, endTrackNum, targetTrackNum, trackNameForDisplay);
					var hasUpdate = RubyAdder.ExecTrack(rubyAdder, track, sb, prm, (_p, _prm) =>
					{
						ProgressDialogState.Report(_prm.bgWorker, new ProgressPair(_prm.endTrackNum + _p.Normalized(), _prm.targetTrackNum), _prm.trackNameForDisplay);
					});
				}
				catch (CancelException ex)
				{
					throw ex;
				}
				catch (Exception ex)
				{
					// なんかエラー
					logger.TraceEvent(TraceEventType.Error, 0, $"トラックエラー. トラック名: {trackNameForDisplay}, ex: {ex.Message}");
					exceptionList.Add(ex);
					sb.Append("トラックを編集出来ませんでした(スキップ)").Append(BR);
					errorTrackNum += 1;
				}

				endTrackNum += 1;
				if (0 < sb.Length)
				{
					sb.Insert(0, trackNameForLog);
				}
				ProgressDialogState.Report(bgWorker, new ProgressPair(endTrackNum, targetTrackNum), null, sb.ToString());
			}

			string log = BR
				+ "エラートラック数: " + errorTrackNum + BR
				+ "総トラック数: " + endTrackNum + BR
				;
			ProgressDialogState.Report(bgWorker, new ProgressPair(endTrackNum, targetTrackNum), null, log);

			System.Threading.Thread.Sleep(1000);

			if (0 < exceptionList.Count)
			{
				result.errorMessage = Properties.Resources.StrErrTrack2;
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

			ProgressDialog.ShowDialogAsync(owner, config, owner, (_bw, _evtArgs, _this) =>
				{
					FlowService.ListupDeleteTrackWithProgress(opeData, _bw, _evtArgs, iTunesApp);
				}).
				SelectMany(_result1 =>
				{
					if (_result1.trackIDList.Length <= 0) throw CancelException.Instance;

					if (_result1.isNeedConfirm)
					{
						// 確認ダイアログあり.
						return FlowService.ShowDeleteConfirmDialogAsync(owner, _result1.trackIDList, iTunesApp.SelectedTracks).
						Select(_result2 => Tuple.Create(_result1, _result2));
					}
					else
					{
						// 確認ダイアログなし.
						return Observable.Return(Tuple.Create(_result1, OkCancelDialog.Result.OK));
					}
				}).
				SelectMany(_result =>
				{
					// キャンセル.
					if (_result.Item2 == OkCancelDialog.Result.Cancel) throw CancelException.Instance;
					// 実削除処理.
					return DeleteTrackAsync(owner, _result.Item1.trackIDList);
				}).
				Subscribe(_ =>
				{
					// 完了.
				}, _ex =>
				{
					if (_ex is CancelException) return; // キャンセルの場合はお咎めなし.

					if (_ex is AppDisplayableException ex)
					{
						ErrorDialog.Show(owner, ex.displayMessage);
					}
					else
					{
						ErrorDialog.ShowUnknown(owner, _ex);

					}
				});
		}

		/// <summary>プログレスバーを表示して削除処理</summary>
		public static IObservable<ProgressResult> DeleteTrackAsync(RootForm owner, TrackID[] trackIDList)
		{
			var config = new ProgressDialog.Config();
			return ProgressDialog.ShowDialogAsync(owner, config, owner, (_bw, _evtArgs, _owner) =>
			{
				FlowService.DeleteTrackWithProgress(_owner.rubyAdder.opeData, _bw, _evtArgs, _owner.rubyAdder.iTunesApp, trackIDList);
			});
		}

		public static IObservable<OkCancelDialog.Result> ShowDeleteConfirmDialogAsync(IWin32Window owner, TrackID[] trackIDList, IITTrackCollection tracks)
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
			return OkCancelDialog.ShowOKCancelAsync(owner, title, body);
		}

		/// <summary>削除対象のトラックをリストアップ, プログレスバーの進行</summary>
		public static void ListupDeleteTrackWithProgress(RubyAdderOpeData opeData, BackgroundWorker bgWorker, DoWorkEventArgs evtArgs, iTunesApp iTunesApp)
		{
			var result = new ProgressResult();
			evtArgs.Result = result;
			var exceptionList = new List<Exception>();
			IITTrackCollection tracks = iTunesApp.SelectedTracks;
			if (tracks == null) throw new AppDisplayableException(Properties.Resources.StrErrTrack1);

			// 工程数を求める.
			// リストアップと削除の2工程.
			opeData.total += tracks.Count * 2;
			int errorTrackNum = 0;
			ProgressDialogState.ReportWithTitle(bgWorker, new ProgressPair(opeData.progress, opeData.total), Properties.Resources.StrDeleteCheckProgress1);

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
					ProgressDialogState.Report(bgWorker, new ProgressPair(opeData.progress, opeData.total), trackNameForDisplay, null);
				}
			}

			result.trackIDList = trackIDList.ToArray();

			if (trackIDList.Count <= 0)
			{
				ProgressDialogState.Report(bgWorker, new ProgressPair(opeData.progress, opeData.total),
					null,
					string.Format(Properties.Resources.StrDeleteCheckLog1, tracks.Count) + BR
				);
			}

			System.Threading.Thread.Sleep(1000);

			if (0 < exceptionList.Count)
			{
				result.errorMessage = Properties.Resources.StrErrTrack2;
			}
		}

		/// <summary>削除処理とプログレスバーの進行</summary>
		public static void DeleteTrackWithProgress(RubyAdderOpeData opeData, BackgroundWorker bw, DoWorkEventArgs evtArgs, iTunesApp iTunesApp, TrackID[] trackIDList)
		{
			ProgressDialogState.ReportWithTitle(bw, new ProgressPair(opeData.progress, opeData.total), Properties.Resources.StrDeleteProgress1);
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
					// 確実にライブラリから削除するために
					// iTunesApp.SelectedTracks ではなく、
					// iTunesApp.LibraryPlaylist.Tracks
					// から削除する.
					//
					// iTunesApp.SelectedTracks から削除すると、
					// プレイリストのトラックを選択してた場合は
					// プレイリストからは削除されるが、ライブラリには残ってしまう.
					//
					//
					// ・LibraryPlaylist.Tracks は都度 iTunesApp から取得する.
					// Delete 後にアクセスすると下記の例外が吐かれるため.
					//
					// System.Runtime.InteropServices.COMException
					// The playlist has been deleted.
					{
						IITTrack track = iTunesApp.LibraryPlaylist.Tracks.GetItemByTrackID_ext(trackID);
						trackNameForDisplay = getTrackNameSafe(track);
						track.Delete();
					}
					ProgressDialogState.Report(bw, new ProgressPair(opeData.progress, opeData.total),
						trackNameForDisplay,
						 string.Format(Properties.Resources.StrDeleteOK, i + 1, trackNameForDisplay) + BR
					);
					++endTrackNum;
				}
				catch (System.Exception ex)
				{
					logger.TraceEvent(TraceEventType.Error, 0, "トラックの削除に失敗しました. ex: " + ex.Message);
					exceptionList.Add(ex);
					ProgressDialogState.Report(bw, new ProgressPair(opeData.progress, opeData.total),
						trackNameForDisplay,
						string.Format(Properties.Resources.StrDeleteNG, i + 1, trackNameForDisplay) + BR
					);
					++errorTrackNum;
				}
				finally
				{
					++opeData.progress;
					ProgressDialogState.Report(bw, new ProgressPair(opeData.progress, opeData.total),
						trackNameForDisplay,
						null
					);
				}
			}

			string log = BR
				+ "エラートラック数: " + errorTrackNum + BR
				+ "削除したトラック数: " + endTrackNum + BR
				;
			ProgressDialogState.Report(bw, new ProgressPair(opeData.progress, opeData.total), null, log);

			System.Threading.Thread.Sleep(1000);

			if (0 < exceptionList.Count)
			{
				result.errorMessage = Properties.Resources.StrErrTrack2;
			}
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

	public static class ObservableHelper
	{
		public static IObservable<Unit> ReturnUnit<T>()
		{
			return Observable.Return(Unit.Default);
		}
	}

	public class CancelException : System.Exception
	{
		public static readonly CancelException Instance = new CancelException();
	}
}
