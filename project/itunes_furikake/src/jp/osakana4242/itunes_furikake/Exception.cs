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

namespace jp.osakana4242.itunes_furikake {
	/// <summary>
	/// ユーザに表示可能なコメントを持つ例外クラス.
	/// </summary>
	public class AppDisplayableException : Exception {
		public string displayMessage {
			get;
			private set;
		}

		public AppDisplayableException(string displayMessage)
			: base(displayMessage) {
			this.displayMessage = displayMessage;
		}

		public AppDisplayableException(string displayMessage, Exception ex)
			: base(displayMessage, ex) {
			if (BuildFlag.IsDebug) {
				Console.WriteLine($"ex: {ex}");
			}
			string BR = System.Environment.NewLine;
			this.displayMessage =
				displayMessage + BR +
				BR +
				string.Format(global::jp.osakana4242.itunes_furikake.Properties.Resources.StrErrDetailBlock, ex);
		}

	}
}
