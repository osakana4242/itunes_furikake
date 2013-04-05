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

    static class NumberUtil
    {
        public static int Percent(int a, int b) {
            return a * 100 / b;
        }
    }

    /// <summary>
    /// ユーザに表示可能なコメントを持つ例外クラス.
    /// </summary>
    public class AppDisplayableException : Exception
    {
        public string displayMessage
        {
            get;
            private set;
        }

        public AppDisplayableException(string displayMessage)
            : base(displayMessage)
        {
            this.displayMessage = displayMessage;
        }

        public AppDisplayableException(string displayMessage, Exception ex)
            : base(displayMessage, ex)
        {
            this.displayMessage = displayMessage;
        }

    }

    public class TrackFieldCantGetException : Exception
    {
        public TrackFieldCantGetException(Exception ex)
            : base("TrackFieldCantGetException", ex)
        {
        }
    }

    public class TrackFieldCantSetException : Exception
    {
        public TrackFieldCantSetException(Exception ex)
            : base("TrackFieldCantSetException", ex)
        {
        }
    }

    static class Program
    {

        private static TraceSource logger = LogOperator.get();


        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {

            bool createdNew;
            //Mutexクラスの作成
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, "jp.osakana4242.itunes_furikake", out createdNew);
            if (createdNew == false)
            {
                //ミューテックスの初期所有権が付与されなかったときは
                //すでに起動していると判断して終了
                ErrorDialog.Show("エラー", "多重起動は出来ません。");
                return;
            }
            try
            {
                cleanFiles();

                logger.TraceEvent(TraceEventType.Information, 0, "itunes_furikake init.");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                RubyAdder rubyAdder = new RubyAdder();
                try
                {
                    rubyAdder.Init();
                    RootForm form = new RootForm(rubyAdder);
                    Application.Run(form);
                }
                catch (AppDisplayableException ex)
                {
                    ErrorDialog.Show("エラー",
                        ex.displayMessage
                    );
                }
                catch (FileNotFoundException ex)
                {
                    string BR = System.Environment.NewLine;
                    ErrorDialog.Show("エラー",
                        ex.FileName + " が見つかりません。"
                    );
                }
                catch (Exception ex)
                {
                    string BR = System.Environment.NewLine;
                    ErrorDialog.Show("エラー",
                        ex.ToString()
                    );
                }
                finally
                {
                    rubyAdder.Exit();
                    logger.TraceEvent(TraceEventType.Information, 0, "itunes_furikake exit.");
                    logger.Close();
                }
            }
            finally
            {
                //ミューテックスを解放する
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 掃除.
        /// </summary>
        private static void cleanFiles()
        {
            // テキストログの削除.
            string dirPath = "log";
            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (Exception ex)
                {
                    logger.TraceEvent(TraceEventType.Error, 0, dirPath + " can't create. " + ex.Message);
                }
            }

            foreach (string path in Directory.GetFiles("log", "*.log"))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    logger.TraceEvent(TraceEventType.Error, 0, path + " can't delete. " + ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// ルビを振る機能を提供.
    /// </summary>
    public class RubyAdder
    {
        private static TraceSource logger = LogOperator.get();

        public delegate void AddLog(string str);

        public enum OPE
        {
            HIRAGANA,
            KATAKANA,
            ALPHABET,
            CLEAR,
        };
        public static string[] TrackFieldNames = {
            "Name",
            "Artist",
            "Album",
            "AlbumArtist",
        };
        private iTunesApp iTunesApp;
        private emanual.IME.ImeLanguage imeLanguage; // 読み仮名取得クラス。

        public OPE Ope = OPE.CLEAR;
        public bool IsForceAdd = false; // ルビを強制的に振るか。
        private static object[] tempArg1 = { null }; // InvokeMember で渡す単一の引数。
        private Dictionary<string, string> dictHiragana2Rome = new Dictionary<string, string>();
        private Dictionary<string, string> dictHiragana2Katakana = new Dictionary<string, string>();
        private Dictionary<string, string> dictWord2Hiragana = new Dictionary<string, string>();
        private AddLog _addLog = null;
        public List<Exception> exceptionList = new List<Exception>();

        public RubyAdder()
        {
        }

        // ログの出力先を設定.
        public void setLogger(AddLog addLog)
        {
            this._addLog = addLog;
        }

        private void addLog(string str)
        {
            if (this._addLog != null)
            {
                this._addLog(str);
            }
        }

        public void Init()
        {
            RubyAdder.ReadDict(this.dictHiragana2Rome, "dict/dict_h2r.txt");
            RubyAdder.ReadDict(this.dictHiragana2Katakana, "dict/dict_h2k.txt");
            RubyAdder.ReadDict(this.dictWord2Hiragana, "dict/dict_word2h.txt");
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


        public DoWorkEventHandler makeDoWorkEventHandler()
        {
            return new DoWorkEventHandler(this.BackgroundWorker1_DoWork);
        }

        //BackgroundWorker1のDoWorkイベントハンドラ
        //ここで時間のかかる処理を行う
        public void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Exec((OPE)e.Argument, (BackgroundWorker)sender, e);
        }

        public void Exec(OPE ope)
        {
            this.Exec(ope, null, null);
        }

        public void Exec(OPE ope, BackgroundWorker bgWorker, DoWorkEventArgs e)
        {
            ProgressResult result = new ProgressResult();
            try
            {
                this.exceptionList.Clear();

                string br = System.Environment.NewLine;
                if (e != null)
                {
                    e.Result = result;
                }

                this.Ope = ope;
                IITTrackCollection tracks = this.iTunesApp.SelectedTracks;
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
                            bgWorker.ReportProgress(NumberUtil.Percent(endTrackNum, targetTrackNum), new ProgressDialogState(null, string.Format("{0}:{1}{2}", endTrackNum + 1, trackName == null ? "-" : trackName, br)));
                        }
                        foreach (string fieldName in TrackFieldNames)
                        {
                            string fieldValue = RubyAdder.GetField(track, fieldName);
                            string curRuby = RubyAdder.GetSortField(track, fieldName);
                            string nextRuby = curRuby;
                            bool isWrite = this.IsForceAdd || curRuby.Length <= 0 || this.Ope == OPE.CLEAR;
                            if (isWrite)
                            {
                                nextRuby = this.MakeSortField(fieldValue);
                                if (nextRuby == curRuby)
                                {
                                    // 変化無し。
                                }
                                else
                                {
                                    RubyAdder.SetSortField(track, fieldName, nextRuby);
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
                    catch (Exception ex)
                    {
                        // なんかエラー
                        logger.TraceEvent(TraceEventType.Error, 0, "トラックエラー. ex:" + ex.Message);
                        this.exceptionList.Add(ex);
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
                            bgWorker.ReportProgress(NumberUtil.Percent(endTrackNum, targetTrackNum), new ProgressDialogState(String.Format("{0}/{1}", endTrackNum, targetTrackNum), sb.ToString()));
                        }
                    }
                }

                if (bgWorker != null)
                {
                    string log = br
                        + "エラートラック数: " + errorTrackNum + br
                        + "総トラック数: " + endTrackNum + br
                        ;
                    bgWorker.ReportProgress(NumberUtil.Percent(endTrackNum, targetTrackNum), new ProgressDialogState(null, log));
                }

                System.Threading.Thread.Sleep(500);

                if (0 < this.exceptionList.Count)
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

        public string MakeSortField(string baseField)
        {
            if (this.Ope == OPE.CLEAR)
            {
                return "";
            }
            if (0 < baseField.Length)
            {
                string ruby = "";
                // 単語リストから変換。
                if (!this.dictWord2Hiragana.TryGetValue(baseField, out ruby))
                {
                    ruby = baseField;
                }

                switch (this.Ope)
                {
                    case OPE.HIRAGANA:
                        ruby = this.toHiragana(ruby);
                        break;
                    case OPE.KATAKANA:
                        ruby = this.toKatakana(ruby);
                        break;
                    case OPE.ALPHABET:
                        ruby = this.toAlphabet(ruby);
                        break;
                }
                System.Console.WriteLine(ruby);
                return ruby;
            }
            else
            {
                return "";
            }
        }

        /** ひらがな化。
        */
        private string toHiragana(string src)
        {
            string dest = this.imeLanguage.GetYomi(src);
            return dest;
        }

        /** カタカナ化。
        */
        private string toKatakana(string src)
        {
            string hiragana = this.toHiragana(src);
            return this.toHoge(hiragana, this.dictHiragana2Katakana);
        }

        /** アルファベット化。
        */
        private string toAlphabet(string src)
        {
            string hiragana = this.toHiragana(src);
            return this.toHoge(hiragana, this.dictHiragana2Rome);
        }


        /** 指定文字列を指定の辞書で置換.
       */
        private string toHoge(string src, Dictionary<string, string> hToHoge)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in src)
            {
                string s = c.ToString();
                string outS = s;
                if (!hToHoge.TryGetValue(s, out outS))
                {
                    outS = s;
                }
                sb.Append(outS);
            }
            return sb.ToString();
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
            RubyAdder.tempArg1[0] = value;
            try
            {
                track.GetType().InvokeMember("Sort" + fieldName, BindingFlags.SetProperty, null, track, RubyAdder.tempArg1);
            }
            catch (Exception ex)
            {
                throw new TrackFieldCantSetException(ex);
            }
        }

        /** 辞書にファイルの内容を読み込む。
        */
        public static void ReadDict(Dictionary<string, string> dict, string filename)
        {
            logger.TraceEvent(TraceEventType.Information, 0, "ReadDict - " + filename);
            using (StreamReader r = new StreamReader(filename, System.Text.Encoding.UTF8))
            {
                string line;
                int lineCnt = 0;
                while ((line = r.ReadLine()) != null) // 1行ずつ読み出し。
                {
                    lineCnt += 1;
                    logger.TraceEvent(TraceEventType.Verbose, 0, line);
                    string[] cols = line.Split('\t');
                    if (line.Length <= 0 || line.IndexOf("#") == 0)
                    {
                        continue;
                    }
                    if (cols.Length != 2)
                    {
                        // 不正な行。
                        if (cols.Length == 1)
                        {
                            throw new AppDisplayableException(makeReadDictErrorMessage(filename, lineCnt, line, "タブが不足しています。"));
                        }
                        continue;
                    }
                    string key = cols[0];
                    string value = cols[1];
                    if (dict.ContainsKey(key))
                    {
                        throw new AppDisplayableException(makeReadDictErrorMessage(filename, lineCnt, line, "[ " + key + " ]が重複しています。"));
                    }
                    else
                    {
                        dict.Add(key, value);
                    }
                }
            }
        }
        public static string makeReadDictErrorMessage(string fileName, int lineCnt, string line, string message)
        {
            return (message + "場所: " + fileName + " - " + lineCnt + "行目 - " + line);
        }

    }
}
