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
                ErrorDialog.Show(null, "多重起動は出来ません。");
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
                    ErrorDialog.Show(null, ex.displayMessage);
                }
                catch (FileNotFoundException ex)
                {
                    ErrorDialog.Show(null, string.Format(Properties.Resources.StrErrFile, ex.FileName));
                }
                catch (Exception ex)
                {
                    ErrorDialog.ShowUnknown(null, ex);
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
}
