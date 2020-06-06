
namespace jp.osakana4242.itunes_furikake
{
    public class RubyAdderOpeData
    {
        public RubyAdderOpeType ope;
        /// <summary>上書きするか</summary>
        public bool isForceAdd;
        /// <summary>削除時に確認を挟むか</summary>
        public bool isNeedConfirmation;

        /// <summary>進捗</summary>
        public int progress;
        /// <summary>工程数</summary>
        public int total;
    }

}
