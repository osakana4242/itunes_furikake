using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.IO;
using System.Security;

namespace jp.osakana4242.core.LogOperator
{
    public class LogOperator
    {
        private static TraceSource traceSource = null;

        public static TraceSource get()
        {
            if (traceSource != null)
            {
                return traceSource;
            }
            traceSource = new TraceSource("itunes_furikake");
            return traceSource;
        }
    }



}
