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
    public class StringHelper
    {
        /// <summary>
        /// 指定文字列に含まれるのがアスキーコードのみなら true
        /// </summary>
        public static bool IsAscii(string str)
        {
            foreach (var c in str)
            {
                if (!IsAscii(c))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsAscii(char c)
        {
            return c <= 0x7f;
        }
    }
}
