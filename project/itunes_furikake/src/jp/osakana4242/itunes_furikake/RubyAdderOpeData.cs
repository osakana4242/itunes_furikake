
namespace jp.osakana4242.itunes_furikake {
	public class RubyAdderOpeData {
		public RubyAdderOpeType ope;
		public RubyAdderRubyType rubyType;
		/// <summary>上書きするか</summary>
		public bool isForceAdd;
		/// <summary>全角英数を半角にするか</summary>
		public bool isZenToHan;
		/// <summary>両端の空白を除去するか</summary>
		public bool isTrim;
		/// <summary>削除時に確認を挟むか</summary>
		public bool isNeedConfirmation;

		/// <summary>進捗</summary>
		public int progress;
		/// <summary>工程数</summary>
		public int total;
	}

}
