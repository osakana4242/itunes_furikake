using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using iTunesLib;

namespace App
{
    public class AppException : Exception
    {
        public AppException(string comment) : base(comment)
        {
        }
    }

    static class Program
    {
        


        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RubyAdder rubyAdder = new RubyAdder();
            try
            {
                rubyAdder.Init();
                RootForm form = new RootForm(rubyAdder);
                Application.Run(form);
            }
            catch (AppException ex)
            {
                ErrorDialog.Show("エラー",
                    ex.Message
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
            }
        }
    }
    public class RubyAdder
    {
        public delegate void AddLog(string str);

        public enum OPE
        {
            HIRAGANA,
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
        private Dictionary<string, string> dictH2R = new Dictionary<string, string>();
        private Dictionary<string, string> dictWord2H = new Dictionary<string, string>();
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
            RubyAdder.ReadDict(this.dictH2R, "dict_h2r.txt");
            RubyAdder.ReadDict(this.dictWord2H, "dict_word2h.txt");
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
            this.exceptionList.Clear();

            string br = System.Environment.NewLine;
            ProgressResult result = new ProgressResult();
            if (e != null)
            {
                e.Result = result;
            }

            ProgressDialogState state = new ProgressDialogState();
            this.Ope = ope;
            IITTrackCollection tracks = this.iTunesApp.SelectedTracks;
            if (tracks == null)
            {
                result.title = global::App.Properties.Resources.StrAppName;
                result.message = global::App.Properties.Resources.StrErrTrack1;
                return;
            }
            int endTrackNum = 0;
            int targetTrackNum = tracks.Count;
            int endFieldNum = 0;
            // int targetFieldNum = tracks.Count * TrackFieldNames.Length;
            foreach (IITFileOrCDTrack track in tracks)
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
                sb.Append(endTrackNum + 1)
                    .Append(":")
                    .Append(track.Name)
                    .Append(br);
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
                            try
                            {
                                RubyAdder.SetSortField(track, fieldName, nextRuby);
                            }
                            catch (Exception ex)
                            {
                                // トラックが編集できない.
                                this.exceptionList.Add(ex);
                                sb.Append(ex.Message).Append(br);
                                goto TRACK_END;
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
                    endFieldNum += 1;
                }
TRACK_END:

                endTrackNum += 1;
                if (bgWorker != null)
                {
                    //ProgressChangedイベントハンドラを呼び出し、
                    //コントロールの表示を変更する
                    state.Text = "" + endTrackNum + "/" + targetTrackNum;
                    state.Log = sb.ToString();
                    bgWorker.ReportProgress(endTrackNum * 100 / targetTrackNum, state);
                }
            }

            System.Threading.Thread.Sleep(500);

            if (0 < this.exceptionList.Count)
            {
                result.title = global::App.Properties.Resources.StrAppName;
                result.message = global::App.Properties.Resources.StrErrTrack2;
                return;
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
                string ruby;
                if (this.dictWord2H.ContainsKey(baseField))
                {
                    // 単語リストから変換。
                    ruby = this.dictWord2H[baseField];
                }
                else
                {
                    ruby = baseField;
                }
                switch (this.Ope)
                {
                    case OPE.HIRAGANA:
                        ruby = this.toHiragana(ruby);
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

        /** アルファベット化。
        */
        private string toAlphabet(string src)
        {
            string hiragana = this.toHiragana(src);
            StringBuilder sb = new StringBuilder();
            foreach (char c in hiragana)
            {
                string s = c.ToString();
                if (this.dictH2R.ContainsKey(s))
                {
                    sb.Append(this.dictH2R[s]);
                }
                else
                {
                    sb.Append(s);
                }
            }
            return sb.ToString();
        }


        public static string GetField(IITFileOrCDTrack track, string fieldName)
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

        public static string GetSortField(IITFileOrCDTrack track, string fieldName)
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

        /** スレッドセーフではない。
        */
        public static void SetSortField(IITFileOrCDTrack track, string fieldName, string value)
        {
            RubyAdder.tempArg1[0] = value;
            try
            {
                track.GetType().InvokeMember("Sort" + fieldName, BindingFlags.SetProperty, null, track, RubyAdder.tempArg1);
            }
            catch (Exception ex)
            {
                string br = System.Environment.NewLine;
                throw new AppException("トラックの編集が出来ませんでした。[" + track.Name + "]" + br + "詳細情報:" + ex);
            }
        }

        /** 辞書にファイルの内容を読み込む。
        */
        public static void ReadDict(Dictionary<string, string> dict, string filename)
        {
            using (StreamReader r = new StreamReader(filename, System.Text.Encoding.UTF8))
            {
                string line;
                int lineCnt = 0;
                while ((line = r.ReadLine()) != null) // 1行ずつ読み出し。
                {
                    lineCnt += 1;
                    Console.WriteLine(line);
                    string[] cols = line.Split('\t');
                    if (line.Length <= 0 || line.IndexOf("#") == 0)
                    {
                        continue;
                    }
                    if (cols.Length != 2)
                    {
                        // 不正な行。
                        continue;
                    }
                    string key = cols[0];
                    string value = cols[1];
                    if (dict.ContainsKey(key))
                    {
                        throw new AppException("[ " + filename + " ]内で[ " + key + " ]が重複しています。(" + lineCnt + "行目)");
                    }
                    else
                    {
                        dict.Add(key, value);
                    }
                }
            }
        }

    }
}
