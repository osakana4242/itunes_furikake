﻿
using System.Collections.Generic;
using System.Diagnostics;
using iTunesLib;

using jp.osakana4242.core.LogOperator;
using jp.osakana4242.itunes_furikake.Properties;

namespace jp.osakana4242.itunes_furikake
{
 
    /// <summary>
    /// ルビを振る機能を提供.
    /// </summary>
    public class RubyAdder : System.IDisposable
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

        public void Dispose()
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


        public static bool SetFieldIfNeeded<T>(RubyAdder rubyAdder, IITFileOrCDTrack track, TrackFieldAccessor accessor, TrackFieldPair before, TrackFieldPair after, T opt, System.Action<ProgressPair, T> reportFunc)
        {
            reportFunc(new ProgressPair(0f, 2f), opt);
            bool isNeedUpdateField = before.field != after.field;
            // フィールドが更新されると、よみがながリセットされてしまうので、変更が無くても設定し直す必要がある.
            bool isNeedUpdateSortField = isNeedUpdateField || before.sortField != after.sortField;
            if ( BuildFlag.TrackFieldForceUpdateEnabled )
            {
                isNeedUpdateField = true;
                isNeedUpdateSortField = true;
            }
            if (isNeedUpdateField)
            {
                if (ConvertHelper.IsNeedSetDummyField(rubyAdder, before.field, after.field))
                {
                    // 文字種の変更だけでは更新が無かったことにされてしまうので、ダミー文字を挟む.
                    accessor.setField(track, after.field + "_");
                }
                accessor.setField(track, after.field);
                reportFunc(new ProgressPair(1f, 2f), opt);
            }

            if (isNeedUpdateSortField)
            {
                if (ConvertHelper.IsNeedSetDummyField(rubyAdder, before.sortField, after.sortField))
                {
                    // 文字種の変更だけでは更新が無かったことにされてしまうので、ダミー文字を挟む.
                    accessor.setSortField(track, after.sortField + "_");
                }
                accessor.setSortField(track, after.sortField);
                reportFunc(new ProgressPair(2f, 2f), opt);
            }
            return isNeedUpdateField || isNeedUpdateSortField;
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
        public static bool ExecTrack<T>(RubyAdder rubyAdder, IITFileOrCDTrack track, System.Text.StringBuilder sb, T prm1, System.Action<ProgressPair, T> reportFunc)
        {
            bool hasUpdate = false;
            var items = TrackFieldAccessor.items;
            var progress = 0;
            var total = items.Length;
            reportFunc(new ProgressPair(progress, total), prm1);
            foreach (var accessor in items)
            {
                var pair = new TrackFieldPair()
                {
                    field = accessor.getField(track),
                    sortField = accessor.getSortField(track),
                };
                var pairAfter = GetProcessedPair(rubyAdder, pair);
                var prm2 = (reportFunc, prm1, progress, total);
                hasUpdate |= SetFieldIfNeeded(rubyAdder, track, accessor, pair, pairAfter, prm2, (_p, _prm2) =>
                {
                    var n = _p.Normalized();
                    _prm2.reportFunc(new ProgressPair(_prm2.progress + n, _prm2.total), _prm2.prm1);
                });
                AddDiff(rubyAdder, pair, pairAfter, sb);
                ++progress;
            }
            return hasUpdate;
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
    }
}
