
using System.Diagnostics;
using System.Windows.Forms;

namespace jp.osakana4242.itunes_furikake {
	public static class ErrorDialog {
		public static void Show(IWin32Window owner, string title, string text) {
			OkCancelDialog.ShowOK(owner, title, text);
		}

		public static void Show(IWin32Window owner, string message) {
			Show(owner, Properties.Resources.StrErrFormTitle, message);
		}

		public static void ShowUnknown(IWin32Window owner, System.Exception ex) {
			RubyAdder.logger.TraceEvent(TraceEventType.Error, 0, string.Format("不明なエラー. ex: {0}", ex.Message));
			Show(owner, Properties.Resources.StrErrFormTitle, string.Format(Properties.Resources.StrErrFormUnknown, ex));
		}
	}
}
