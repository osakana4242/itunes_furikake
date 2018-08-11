﻿using System;
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
}
