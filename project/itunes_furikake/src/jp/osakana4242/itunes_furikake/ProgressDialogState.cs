using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jp.osakana4242.itunes_furikake
{
    public sealed class ProgressDialogState
    {
        public readonly ProgressPair Progress;
        public readonly string Title;
        public readonly string Text;
        public readonly string Log;
        public ProgressDialogState(ProgressPair progress, string title, string text, string log)
        {
            this.Progress = progress;
            this.Title = title;
            this.Text = text;
            this.Log = log;
        }

        public static void ReportWithTitle(BackgroundWorker bw, in ProgressPair progress, string title)
        {
            var self = new ProgressDialogState(progress, title, null, null);
            SleepForInputIfNeeded(bw, self);
        }

        public static void Report(BackgroundWorker bw, in ProgressPair progress, string text = null, string log = null)
        {
            var self = new ProgressDialogState(progress, null, text, log);
            SleepForInputIfNeeded(bw, self);
        }

        static void SleepForInputIfNeeded(BackgroundWorker bw, ProgressDialogState state)
        {
            bw.ReportProgress((int)(state.Progress.Normalized() * 100f), state);
            // フリーズ対策.
            // 再描画が長時間行われない場合は再描画を待つ.
            var now = DateTimeOffset.Now;
            var threshold = TimeSpan.FromSeconds(1f);
            var nextSleep = ProgressDialog.lastPaintTime + threshold;
            if (now < nextSleep) return;
            // 再描画待ち.
            while (ProgressDialog.lastPaintTime <= nextSleep)
            {
                if (bw.CancellationPending) throw new CancelException();
                if (Thread.CurrentThread == ProgressDialog.mainThread_) return;
                Thread.Sleep(100);
                bw.ReportProgress((int)(state.Progress.Normalized()), state);
            }
            // ボタン入力を受け付ける猶予.
            Thread.Sleep(1000);
        }
    }
}
