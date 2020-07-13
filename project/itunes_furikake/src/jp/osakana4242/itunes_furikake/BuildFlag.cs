
using System.Collections.Generic;
using System.Diagnostics;
using iTunesLib;

using jp.osakana4242.core.LogOperator;

namespace jp.osakana4242.itunes_furikake
{
    public static class BuildFlag
    {
        public const bool IsDebug =
        #if DEBUG
            true;
#else
            false;
#endif
        /// <summary>1トラックのフィールドをすべて書き換えたときの画面更新の遅れ具合の確認用.</summary>
        public const bool TrackFieldForceUpdateEnabled = IsDebug && true;
    }
}
