using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jp.osakana4242.itunes_furikake
{
    public sealed class ProgressResult
    {
        public object result;
        public Exception ex;
        public string errorMessage;
        public bool isNeedConfirm;
        public TrackID[] trackIDList = { };
    }
}
