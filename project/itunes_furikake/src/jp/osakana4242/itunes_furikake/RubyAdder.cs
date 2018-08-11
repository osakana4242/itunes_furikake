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
        public static TraceSource logger = LogOperator.get();

        public delegate void AddLog(string str);

        public static string[] TrackFieldNames = {
            "Name",
            "Artist",
            "Album",
            "AlbumArtist",
        };
        public iTunesApp iTunesApp;
        public emanual.IME.ImeLanguage imeLanguage; // 読み仮名取得クラス。

        private static object[] tempArg1 = { null }; // InvokeMember で渡す単一の引数。

        public RubyAdderOpeData opeData = new RubyAdderOpeData();
        public Dictionary<string, string> dictHiragana2Rome = new Dictionary<string, string>();
        public Dictionary<string, string> dictHiragana2Katakana = new Dictionary<string, string>();
        public Dictionary<string, string> dictWord2Hiragana = new Dictionary<string, string>();
        public Dictionary<char, char> dictZen2Han = new Dictionary<char, char>();

        private AddLog addLog_ = null;
        public List<Exception> exceptionList = new List<Exception>();

        public RubyAdder()
        {
        }

        // ログの出力先を設定.
        public void setLogger(AddLog addLog)
        {
            this.addLog_ = addLog;
        }

        private void addLog(string str)
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

        static void Reallocate<K, V>( ref Dictionary<K, V> dict )
        {
            dict = new Dictionary<K, V>(dict);
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
                rubyAdder.exceptionList.Clear();

                string br = System.Environment.NewLine;
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
                // int targetFieldNum = tracks.Count * TrackFieldNames.Length;
                foreach (IITTrack track in tracks)
                {
                    if (bgWorker != null)
                    {
                        if (bgWorker.CancellationPending)
                        {
                            // 中断。
                            return;
                        }
                    }
                    // 1トラック分の処理.
                    StringBuilder sb = new StringBuilder();
                    try
                    {
                        string trackName = null;
                        try
                        {
                            trackName = track.Name;
                        }
                        catch (Exception ex)
                        {
                            logger.TraceEvent(TraceEventType.Error, 0, "トラック名が取得出来ません. ex:" + ex.Message);
                        }
                        if (bgWorker != null)
                        {
                            bgWorker.ReportProgress(NumberHelper.Percent(endTrackNum, targetTrackNum), new ProgressDialogState(null, string.Format("{0}:{1}{2}", endTrackNum + 1, trackName == null ? "-" : trackName, br)));
                        }
                        foreach (string fieldName in TrackFieldNames)
                        {
                            string fieldValue = RubyAdder.GetField(track, fieldName);
                            string curRuby = RubyAdder.GetSortField(track, fieldName);
                            string nextFieldValue = fieldValue;
                            bool isNeedSetRuby = false;
                            if (rubyAdder.opeData.ope == RubyAdderOpeType.ZEN2HAN)
                            {
                                nextFieldValue = ConvertHelper.ToHankaku(rubyAdder, fieldValue);
                                if (fieldValue != nextFieldValue)
                                {
                                    // 文字種の変更だけでは更新が無かったことにされてしまうので、ダミー文字を挟む.
                                    RubyAdder.SetField(track, fieldName, nextFieldValue + "_");
                                    RubyAdder.SetField(track, fieldName, nextFieldValue);
                                    // フィールドが更新されると、よみがながリセットされてしまうので、変更が無くても設定し直す必要がある.
                                    isNeedSetRuby = true;
                                }

                                string nextRuby = curRuby;
                                {
                                    nextRuby = ConvertHelper.ToHankaku(rubyAdder, curRuby);
                                    if (nextRuby == curRuby || !isNeedSetRuby)
                                    {
                                        // 変化無し。
                                    }
                                    else
                                    {
                                        // 文字種の変更だけでは更新が無かったことにされてしまうので、ダミー文字を挟む.
                                        RubyAdder.SetSortField(track, fieldName, nextRuby + "_");
                                        RubyAdder.SetSortField(track, fieldName, nextRuby);
                                    }
                                }
                                if (fieldValue != nextFieldValue)
                                {
                                    sb.Append("[ ")
                                        .Append(fieldValue)
                                        .Append(" ]")
                                        .Append(" -> ")
                                        .Append("[ ")
                                        .Append(nextFieldValue)
                                        .Append(" ]");
                                    sb.Append(br);
                                }
                                if (fieldValue != nextRuby)
                                {
                                    sb.Append("[ ")
                                        .Append(curRuby)
                                        .Append(" ]")
                                        .Append(" -> ")
                                        .Append("[ ")
                                        .Append(nextRuby)
                                        .Append(" ]");
                                }
                                sb.Append(br);
                            }
                            else
                            {
                                string nextRuby = curRuby;
                                bool isWrite = rubyAdder.opeData.isForceAdd || curRuby.Length <= 0 || rubyAdder.opeData.ope == RubyAdderOpeType.CLEAR;
                                if (isWrite)
                                {
                                    nextRuby = ConvertHelper.MakeSortField(rubyAdder, nextFieldValue);
                                    if (nextRuby == curRuby)
                                    {
                                        // 変化無し。
                                    }
                                    else
                                    {
                                        RubyAdder.SetSortField(track, fieldName, nextRuby);
                                        string afterRuby = RubyAdder.GetSortField(track, fieldName);
                                        if (nextRuby != afterRuby)
                                        {
                                            // 文字種の変更だけでは更新が無かったことにされてしまうので、ダミー文字を挟む.
                                            RubyAdder.SetSortField(track, fieldName, nextRuby + "_");
                                            RubyAdder.SetSortField(track, fieldName, nextRuby);
                                        }
                                    }
                                }
                                sb.Append("[ ")
                                    .Append(fieldValue)
                                    .Append(" ]")
                                    .Append(" -> ")
                                    .Append("[ ")
                                    .Append(nextRuby)
                                    .Append(" ]");
                                if (!isWrite)
                                {
                                    sb.Append("(スキップ)");
                                }
                                sb.Append(br);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // なんかエラー
                        logger.TraceEvent(TraceEventType.Error, 0, "トラックエラー. ex:" + ex.Message);
                        rubyAdder.exceptionList.Add(ex);
                        sb.Append("トラックを編集出来ませんでした(スキップ)").Append(br);
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

                if (bgWorker != null)
                {
                    string log = br
                        + "エラートラック数: " + errorTrackNum + br
                        + "総トラック数: " + endTrackNum + br
                        ;
                    bgWorker.ReportProgress(NumberHelper.Percent(endTrackNum, targetTrackNum), new ProgressDialogState(null, log));
                }

                System.Threading.Thread.Sleep(500);

                if (0 < rubyAdder.exceptionList.Count)
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

        public static string GetField(IITTrack track, string fieldName)
        {
            try
            {
                string ret = (string)track.GetType().InvokeMember(fieldName, BindingFlags.GetProperty, null, track, null);
                if (ret != null)
                {
                    return ret;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new TrackFieldCantGetException(ex);
            }
        }

        public static string GetSortField(IITTrack track, string fieldName)
        {
            try
            {
                string ret = (string)track.GetType().InvokeMember("Sort" + fieldName, BindingFlags.GetProperty, null, track, null);
                if (ret != null)
                {
                    return ret;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new TrackFieldCantGetException(ex);
            }
        }

        /** スレッドセーフではない。
        */
        public static void SetSortField(IITTrack track, string fieldName, string value)
        {
            SetField(track, "Sort" + fieldName, value);
        }

        public static void SetField(IITTrack track, string fieldName, string value)
        {
            RubyAdder.tempArg1[0] = value;
            try
            {
                track.GetType().InvokeMember(fieldName, BindingFlags.SetProperty, null, track, RubyAdder.tempArg1);
            }
            catch (Exception ex)
            {
                throw new TrackFieldCantSetException(ex);
            }
        }
    }
}
