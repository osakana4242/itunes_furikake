using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using iTunesLib;

using jp.osakana4242.core.LogOperator;

namespace jp.osakana4242.itunes_furikake
{
    public enum RubyAdderOpeType
    {
        HIRAGANA,
        KATAKANA,
        ALPHABET,
        CLEAR,
        ZEN2HAN,
    };

    public class RubyAdderOpeData
    {
        public RubyAdderOpeType ope;
        public bool isForceAdd;
    }

    /// <summary>
    /// ルビを振る機能を提供.
    /// </summary>
    public class RubyAdder
    {
        private static readonly string BR = System.Environment.NewLine;
        public static TraceSource logger = LogOperator.get();

        public delegate void AddLogDelegate(string str);

        public iTunesApp iTunesApp;
        public emanual.IME.ImeLanguage imeLanguage; // 読み仮名取得クラス。

        public RubyAdderOpeData opeData = new RubyAdderOpeData();
        public Dictionary<string, string> dictHiragana2Rome = new Dictionary<string, string>();
        public Dictionary<string, string> dictHiragana2Katakana = new Dictionary<string, string>();
        public Dictionary<string, string> dictWord2Hiragana = new Dictionary<string, string>();
        public Dictionary<char, char> dictZen2Han = new Dictionary<char, char>();

        private AddLogDelegate addLog_ = null;

        public RubyAdder()
        {
        }

        // ログの出力先を設定.
        public void SetLogger(AddLogDelegate addLog)
        {
            this.addLog_ = addLog;
        }

        private void AddLog(string str)
        {
            if (this.addLog_ != null)
            {
                this.addLog_(str);
            }
        }

        public void Init()
        {
            dictHiragana2Rome = LoadHelper.ReadDictS("dict/dict_h2r.txt");
            dictHiragana2Katakana = LoadHelper.ReadDictS("dict/dict_h2k.txt");
            dictWord2Hiragana = LoadHelper.ReadDictS("dict/dict_word2h.txt");
            dictZen2Han = LoadHelper.ReadDictC("dict/dict_zen2han.txt");

            this.imeLanguage = new emanual.IME.ImeLanguage();
            this.iTunesApp = new iTunesApp();
        }

        public void Exit()
        {
            if (this.iTunesApp != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(this.iTunesApp);
                this.iTunesApp = null;
            }
            if (this.imeLanguage != null)
            {
                this.imeLanguage.Dispose();
                this.imeLanguage = null;
            }
        }


        //BackgroundWorker1のDoWorkイベントハンドラ
        //ここで時間のかかる処理を行う
        public static readonly DoWorkEventHandler doWorkEventHandler = (object sender, DoWorkEventArgs e) =>
        {
            Exec((BackgroundWorker)sender, e);
        };


        public static void Exec(BackgroundWorker bgWorker, DoWorkEventArgs e)
        {
            RubyAdder rubyAdder = (RubyAdder)e.Argument;
            ProgressResult result = new ProgressResult();
            try
            {
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
                        IITFileOrCDTrack track = trackBase as IITFileOrCDTrack;
                        if (track == null) continue;
                        bgWorker.ReportProgress(NumberHelper.Percent(endTrackNum, targetTrackNum), new ProgressDialogState(null, string.Format("{0}:{1}{2}", endTrackNum + 1, trackName == null ? "-" : trackName, BR)));
                        ExecTrack(rubyAdder, track, sb);
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
                    result.title = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrAppName;
                    result.message = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrErrTrack2;
                }
            }
            catch (Exception ex)
            {
                logger.TraceEvent(TraceEventType.Error, 0, "不昧なエラー. ex:" + ex.Message);
                result.title = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrAppName;
                result.message = global::jp.osakana4242.itunes_furikake.Properties.Resources.StrErrUnknown;
            }
        }

        /** 処理を適用したペアを得る */
        public static TrackFieldPair GetProcessedPair(RubyAdder rubyAdder, TrackFieldPair pair)
        {
            switch (rubyAdder.opeData.ope)
            {
                case RubyAdderOpeType.ZEN2HAN:
                    return UpdateZenToHan(rubyAdder, pair);
                case RubyAdderOpeType.CLEAR:
                    return ClearSortField(rubyAdder, pair);
                default:
                    return UpdateSortField(rubyAdder, pair);
            }
        }


        public static void SetFieldIfNeeded(RubyAdder rubyAdder, IITFileOrCDTrack track, TrackFieldAccessor accessor, TrackFieldPair before, TrackFieldPair after)
        {
            bool isNeedUpdateField = before.field != after.field;
            // フィールドが更新されると、よみがながリセットされてしまうので、変更が無くても設定し直す必要がある.
            bool isNeedUpdateSortField = isNeedUpdateField || before.sortField != after.sortField;

            if (isNeedUpdateField)
            {
                if (ConvertHelper.IsNeedSetDummyField(rubyAdder, before.field, after.field))
                {
                    // 文字種の変更だけでは更新が無かったことにされてしまうので、ダミー文字を挟む.
                    accessor.setField(track, after.field + "_");
                }
                accessor.setField(track, after.field);
            }

            if (isNeedUpdateSortField)
            {
                if (ConvertHelper.IsNeedSetDummyField(rubyAdder, before.sortField, after.sortField))
                {
                    // 文字種の変更だけでは更新が無かったことにされてしまうので、ダミー文字を挟む.
                    accessor.setSortField(track, after.sortField + "_");
                }
                accessor.setSortField(track, after.sortField);
            }
        }

        /** 変化分を sb に追記する.  */
        public static void AddDiff(RubyAdder rubyAdder, TrackFieldPair before, TrackFieldPair after, System.Text.StringBuilder sb)
        {
            if (before.field != after.field)
            {
                sb.AppendFormat("[{0}] -> [{1}]", before.field, after.field);
                sb.Append(BR);
            }
            if (before.sortField != after.sortField)
            {
                sb.AppendFormat("[{0}]: [{1}] -> [{2}]", after.field, before.sortField, after.sortField);
                sb.Append(BR);
            }
        }

        /** トラックに対する処理の実行. */
        public static void ExecTrack(RubyAdder rubyAdder, IITFileOrCDTrack track, System.Text.StringBuilder sb)
        {
            foreach (var accessor in TrackFieldAccessor.items)
            {
                var pair = new TrackFieldPair()
                {
                    field = accessor.getField(track),
                    sortField = accessor.getSortField(track),
                };
                var pairAfter = GetProcessedPair(rubyAdder, pair);
                SetFieldIfNeeded(rubyAdder, track, accessor, pair, pairAfter);
                AddDiff(rubyAdder, pair, pairAfter, sb);
            }
        }

        public static TrackFieldPair ClearSortField(RubyAdder rubyAdder, TrackFieldPair pair)
        {
            pair.sortField = "";
            return pair;
        }

        public static TrackFieldPair UpdateSortField(RubyAdder rubyAdder, TrackFieldPair pair)
        {
            bool isWrite = rubyAdder.opeData.isForceAdd || pair.sortField.Length <= 0;
            if (!isWrite) return pair;

            pair.sortField = ConvertHelper.MakeSortField(rubyAdder, pair.field);
            return pair;
        }

        public static TrackFieldPair UpdateZenToHan(RubyAdder rubyAdder, TrackFieldPair pair)
        {
            pair.field = ConvertHelper.ToHankaku(rubyAdder, pair.field);
            pair.sortField = ConvertHelper.ToHankaku(rubyAdder, pair.sortField);
            return pair;
        }

        public struct TrackFieldPair
        {
            public string field;
            public string sortField;
        }

    }
}
