
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
			var BR = System.Environment.NewLine;
			var message = Properties.Resources.StrErrFormUnknown + BR +
				BR +
				string.Format(Properties.Resources.StrErrDetailBlock, ex);
			RubyAdder.logger.TraceEvent(TraceEventType.Error, 0, message);
			Show(owner, Properties.Resources.StrErrFormTitle, message);
		}

		public static bool TryShowException(IWin32Window owner, System.Exception ex) {
			if (ex is CancelException) return false;

			if (ex is AppDisplayableException ex1) {
				ErrorDialog.Show(owner, ex1.displayMessage);
			} else {
				ErrorDialog.ShowUnknown(owner, ex);
			}
			return true;
		}

		public static bool TryShowException(System.Exception ex) {
			return TryShowException(null, ex);
		}

	}
}
