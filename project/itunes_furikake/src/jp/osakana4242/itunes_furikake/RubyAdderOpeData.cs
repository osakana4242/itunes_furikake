
namespace jp.osakana4242.itunes_furikake {
	public class RubyAdderOpeData {
		public RubyAdderOpeType ope;
		public Setting setting = Setting.Default;

		/// <summary>進捗</summary>
		public int progress;
		/// <summary>工程数</summary>
		public int total;

		public readonly struct Setting {
			public static readonly Setting Default = new Setting(
				rubyAdd: RubyAdd.Default,
				isRubyRemove: false,
				isZenToHan: true,
				isTrim: true
			);

			public readonly RubyAdd rubyAdd;
			public readonly bool isRubyRemove;
			public readonly bool isZenToHan;
			public readonly bool isTrim;

			public Setting(
				RubyAdd rubyAdd,
				bool isRubyRemove,
				bool isZenToHan,
				bool isTrim
			) {
				this.rubyAdd      = rubyAdd     ;
				this.isRubyRemove = isRubyRemove;
				this.isZenToHan   = isZenToHan  ;
				this.isTrim       = isTrim      ;
				if (rubyAdd.enabled && isRubyRemove) {
					throw new System.ArgumentException("読みの振り、読みを消すは両立出来ない");
				}
			}

			public bool HasTask() {
				if ( rubyAdd.enabled ) return true;
				if ( isRubyRemove ) return true;
				if ( isZenToHan ) return true;
				if ( isTrim ) return true;
				return false;
			}
		}

		public readonly struct RubyAdd {
			public static readonly RubyAdd Default = new RubyAdd(
				true,
				false,
				RubyAdderRubyType.Hiragana
			);

			public readonly bool enabled;
			public readonly bool isForceAdd;
			public readonly RubyAdderRubyType rubyType;

			public RubyAdd(
				bool enabled,
				bool isForceAdd,
				RubyAdderRubyType rubyType
			)
			{
				this.enabled    = enabled   ;
				this.isForceAdd = isForceAdd;
				this.rubyType   = rubyType  ;
			}
		}
	}

}
