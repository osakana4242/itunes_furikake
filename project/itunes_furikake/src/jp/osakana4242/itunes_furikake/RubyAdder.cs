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
using System.Diagnostics.Eventing.Reader;

namespace jp.osakana4242.itunes_furikake
{
    public struct TrackID
    {
        public readonly int highID;
        public readonly int lowID;
        public TrackID(int high, int low)
        {
            this.highID = high;
            this.lowID = low;
        }
    }

    public static class iTunesAppExt
    {
        public static TrackID GetTrackID_ext(this iTunesApp self, object obj)
        {
            int highID;
            int lowID;
            self.GetITObjectPersistentIDs(ref obj, out highID, out lowID);
            return new TrackID(highID, lowID);
        }
    }

    public static class IITTrackCollectionExt
    {

        public static IITTrack GetItemByTrackID_ext(this IITTrackCollection self, in TrackID id)
        {
            return self.ItemByPersistentID[id.highID, id.lowID];
        }
    }

    public enum RubyAdderOpeType
    {
        HIRAGANA,
        KATAKANA,
        ALPHABET,
        CLEAR,
        ZEN2HAN,
        DELETE_UNEXISTS,
    };

    public class RubyAdderOpeData
    {
        public RubyAdderOpeType ope;
        /// <summary>上書きするか</summary>
        public bool isForceAdd;
        /// <summary>削除時に確認を挟むか</summary>
        public bool isNeedConfirmation;

        /// <summary>進捗</summary>
        public int progress;
        /// <summary>工程数</summary>
        public int total;
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
