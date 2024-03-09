
using System.Collections.Generic;
using System.Diagnostics;
using iTunesLib;

using jp.osakana4242.core.LogOperator;
using jp.osakana4242.itunes_furikake.Properties;

namespace jp.osakana4242.itunes_furikake {

	/// <summary>
	/// ルビを振る機能を提供.
	/// </summary>
	public class RubyAdder : System.IDisposable {
		private static readonly string BR = System.Environment.NewLine;
		public static TraceSource logger = LogOperator.get();

		public delegate void AddLogDelegate(string str);

		public iTunesApp iTunesApp;
		public ImeLanguage imeLanguage; // 読み仮名取得クラス。

		public RubyAdderOpeData opeData = new RubyAdderOpeData();
		public Dictionary<string, string> dictHiragana2Rome = new Dictionary<string, string>();
		public Dictionary<string, string> dictHiragana2Katakana = new Dictionary<string, string>();
		public Dictionary<string, string> dictWord2Hiragana = new Dictionary<string, string>();
		public Dictionary<char, char> dictZen2Han = new Dictionary<char, char>();

		private AddLogDelegate addLog_ = null;

		public RubyAdder() {
		}

		// ログの出力先を設定.
		public void SetLogger(AddLogDelegate addLog) {
			this.addLog_ = addLog;
		}

		private void AddLog(string str) {
			if (this.addLog_ != null) {
				this.addLog_(str);
			}
		}

		public void Init() {
			dictHiragana2Rome = LoadHelper.ReadDictS("dict/dict_h2r.txt");
			dictHiragana2Katakana = LoadHelper.ReadDictS("dict/dict_h2k.txt");
			dictWord2Hiragana = LoadHelper.ReadDictS("dict/dict_word2h.txt");
			dictZen2Han = LoadHelper.ReadDictC("dict/dict_zen2han.txt");

			this.imeLanguage = new ImeLanguage();
			this.iTunesApp = createiTunesApp();
		}

		public void Dispose() {
			if (this.iTunesApp != null) {
				System.Runtime.InteropServices.Marshal.ReleaseComObject(this.iTunesApp);
				this.iTunesApp = null;
			}
			if (this.imeLanguage != null) {
				this.imeLanguage.Dispose();
				this.imeLanguage = null;
			}
		}

		static iTunesApp createiTunesApp() {
			try {
				return new iTunesApp();
			} catch (System.Exception ex) {
				if (ex.Message.Contains("CO_E_SERVER_EXEC_FAILURE")) {
					// iTunes 初回起動時の規約確認で拒否して、 iTunes を終了させたとき:
					// System.Runtime.InteropServices.COMException: CLSID {DC0C2640-1415-4644-875C-6F4D769839BA} を含むコンポーネントの COM クラス ファクトリを取得中に、次のエラーが発生しました: 80080005 サーバーの実行に失敗しました (HRESULT からの例外:0x80080005 (CO_E_SERVER_EXEC_FAILURE))。
					//    場所 System.RuntimeTypeHandle.CreateInstance(RuntimeType type, Boolean publicOnly, Boolean noCheck, Boolean& canBeCached, RuntimeMethodHandleInternal& ctor, Boolean& bNeedSecurityCheck)
					//    場所 System.RuntimeType.CreateInstanceSlow(Boolean publicOnly, Boolean skipCheckThis, Boolean fillCache, StackCrawlMark& stackMark)
					//    場所 System.RuntimeType.CreateInstanceDefaultCtor(Boolean publicOnly, Boolean skipCheckThis, Boolean fillCache, StackCrawlMark& stackMark)
					//    場所 System.Activator.CreateInstance(Type type, Boolean nonPublic)
					//    場所 System.Activator.CreateInstance(Type type) 
					//    場所 jp.osakana4242.itunes_furikake.RubyAdder.Init() 場所 C:\Users\osakana4242\prj\itunes_furikake\project\itunes_furikake\src\jp\osakana4242\itunes_furikake\RubyAdder.cs:行 53
					throw new AppDisplayableException(Resources.StrErriTunesExecFailure, ex);
				} else {
					// iTunes をアンインストールした状態のとき:
					//  System.Runtime.InteropServices.COMException (0x80040154): CLSID {DC0C2640-1415-4644-875C-6F4D769839BA} を含むコンポーネントの COM クラス ファクトリを取得中に、次のエラーが発生しました: 80040154 クラスが登録されていません (HRESULT からの例外:0x80040154 (REGDB_E_CLASSNOTREG))。
					//     場所 System.RuntimeTypeHandle.CreateInstance(RuntimeType type, Boolean publicOnly, Boolean noCheck, Boolean& canBeCached, RuntimeMethodHandleInternal& ctor, Boolean& bNeedSecurityCheck)
					//     場所 System.RuntimeType.CreateInstanceSlow(Boolean publicOnly, Boolean skipCheckThis, Boolean fillCache, StackCrawlMark& stackMark)
					//     場所 System.RuntimeType.CreateInstanceDefaultCtor(Boolean publicOnly, Boolean skipCheckThis, Boolean fillCache, StackCrawlMark& stackMark)
					//     場所 System.Activator.CreateInstance(Type type, Boolean nonPublic)
					//     場所 System.Activator.CreateInstance(Type type)
					//     場所 jp.osakana4242.itunes_furikake.RubyAdder.Init()
					//     場所 jp.osakana4242.itunes_furikake.RootForm..ctor()
					//     場所 jp.osakana4242.itunes_furikake.Program.<>c__DisplayClass1_1.<Main>b__8(Int32 _2)
					//     場所 System.Reactive.Linq.ObservableImpl.Select`2.Selector._.OnNext(TSource value)
					throw new AppDisplayableException(Resources.StrErriTunesNotFound, ex);
				}
			}
		}

		/// <summary>処理を適用したペアを得る</summary>
		public static TrackFieldPair GetProcessedPair(RubyAdder rubyAdder, TrackFieldPair pair) {
			var setting = rubyAdder.opeData.setting;
			if (setting.isRubyRemove) {
				pair = ClearSortField(rubyAdder, pair);
			}
			if (setting.rubyAdd.enabled) {
				pair = UpdateSortField(rubyAdder, pair);
			}
			if (setting.isZenToHan) {
				pair = UpdateZenToHan(rubyAdder, pair);
			}
			if (setting.isTrim) {
				pair = UpdateTrim(rubyAdder, pair);
			}
 			return pair;
		}


		public static bool SetFieldIfNeeded<T>(RubyAdder rubyAdder, IITFileOrCDTrack track, TrackFieldAccessor accessor, TrackFieldPair before, TrackFieldPair after, T opt, System.Action<ProgressPair, T> reportFunc) {
			reportFunc(new ProgressPair(0f, 2f), opt);
			bool isNeedUpdateField = before.field != after.field;
			// フィールドが更新されると、よみがながリセットされてしまうので、変更が無くても設定し直す必要がある.
			bool isNeedUpdateSortField = isNeedUpdateField || before.sortField != after.sortField;
			if (BuildFlag.TrackFieldForceUpdateEnabled) {
				isNeedUpdateField = true;
				isNeedUpdateSortField = true;
			}
			if (isNeedUpdateField) {
				if (ConvertHelper.IsNeedSetDummyField(rubyAdder, before.field, after.field)) {
					// 文字種の変更だけでは更新が無かったことにされてしまうので、ダミー文字を挟む.
					accessor.setField(track, after.field + "_");
				}
				accessor.setField(track, after.field);
				reportFunc(new ProgressPair(1f, 2f), opt);
			}

			if (isNeedUpdateSortField) {
				if (ConvertHelper.IsNeedSetDummyField(rubyAdder, before.sortField, after.sortField)) {
					// 文字種の変更だけでは更新が無かったことにされてしまうので、ダミー文字を挟む.
					accessor.setSortField(track, after.sortField + "_");
				}
				accessor.setSortField(track, after.sortField);
				reportFunc(new ProgressPair(2f, 2f), opt);
			}
			return isNeedUpdateField || isNeedUpdateSortField;
		}

		/// <summary>変化分を sb に追記する.</summary>
		public static void AddDiff(RubyAdder rubyAdder, TrackFieldPair before, TrackFieldPair after, System.Text.StringBuilder sb) {
			if (before.field != after.field) {
				sb.AppendFormat("[{0}] -> [{1}]", before.field, after.field);
				sb.Append(BR);
			}
			if (before.sortField != after.sortField) {
				sb.AppendFormat("[{0}]: [{1}] -> [{2}]", after.field, before.sortField, after.sortField);
				sb.Append(BR);
			}
		}

		/// <summary>トラックに対する処理の実行.</summary>
		public static bool ExecTrack<T>(RubyAdder rubyAdder, IITFileOrCDTrack track, System.Text.StringBuilder sb, T prm1, System.Action<ProgressPair, T> reportFunc) {
			bool hasUpdate = false;
			var items = TrackFieldAccessor.items;
			var progress = 0;
			var total = items.Length;
			reportFunc(new ProgressPair(progress, total), prm1);
			foreach (var accessor in items) {
				var pair = new TrackFieldPair(
					field: accessor.getField(track),
					sortField: accessor.getSortField(track)
				);
				var pairAfter = GetProcessedPair(rubyAdder, pair);
				var prm2 = (reportFunc, prm1, progress, total);
				hasUpdate |= SetFieldIfNeeded(rubyAdder, track, accessor, pair, pairAfter, prm2, (_p, _prm2) => {
					var n = _p.Normalized();
					_prm2.reportFunc(new ProgressPair(_prm2.progress + n, _prm2.total), _prm2.prm1);
				});
				AddDiff(rubyAdder, pair, pairAfter, sb);
				++progress;
			}
			return hasUpdate;
		}

		public static TrackFieldPair ClearSortField(RubyAdder rubyAdder, TrackFieldPair pair) {
			return pair.SetSortField("");
		}

		public static TrackFieldPair UpdateSortField(RubyAdder rubyAdder, TrackFieldPair pair) {
			bool isWrite = rubyAdder.opeData.setting.rubyAdd.isForceAdd || pair.sortField.Length <= 0;
			if (!isWrite) return pair;

			return pair.SetSortField( ConvertHelper.MakeSortField(rubyAdder, pair.field) );
		}

		public static TrackFieldPair UpdateZenToHan(RubyAdder rubyAdder, TrackFieldPair pair) {
			return new TrackFieldPair(
				field: ConvertHelper.ToHankaku(rubyAdder, pair.field),
				sortField: ConvertHelper.ToHankaku(rubyAdder, pair.sortField)
			);
		}

		public static TrackFieldPair UpdateTrim(RubyAdder rubyAdder, TrackFieldPair pair) {
			return new TrackFieldPair(
				field: ConvertHelper.Trim(rubyAdder, pair.field),
				sortField: ConvertHelper.Trim(rubyAdder, pair.sortField)
			);
		}

		public static bool TryGetSelectedTracks(RubyAdder rubyAdder, out IITTrackCollection tracks) {
			try {
				var iTunesApp = rubyAdder.iTunesApp;
				tracks = iTunesApp.SelectedTracks;
				if (null == tracks) return false;
				if (tracks.Count <= 0) return false;
				return true;
			} catch (System.Exception ex) {
				if (BuildFlag.IsDebug) {
					// ex: 'System.Runtime.InteropServices.COMException (0x80004005): エラー HRESULT E_FAIL が COM コンポーネントの呼び出しから返されました。
					// 場所 iTunesLib.IiTunes.get_SelectedTracks()
					// 場所 jp.osakana4242.itunes_furikake.RootForm.UpdateComponentStatus() 場所 C:\Users\osakana4242\prj\itunes_furikake\project\itunes_furikake\src\jp\osakana4242\itunes_furikake\RootForm.cs:行 139'
					System.Console.WriteLine($"ex: '{ex}'");
				}
				tracks = null;
				return false;
			}
		}
	}
}
