
using System.Collections.Generic;

namespace jp.osakana4242.itunes_furikake {
	public struct YomiWord {
		public string word;
		public bool isNeedYomi;
	}

	public static class YomiWordUtil {

		/// <summary>読みをふる対象の文字(ひらがな、カタカナ、漢字)か</summary>
		public static bool IsYomiTarget(string str, int index, out int length) {
			var c = str[index];
			var hasNextC = index + 1 < str.Length;
			length = (char.IsHighSurrogate(c) && hasNextC) ? 2 : 1;

			if (StringHelper.IsHiragana(c)) return true;
			if (StringHelper.IsKatakana(c)) return true;
			if (StringHelper.IsKanji(str, index)) return true;
			return false;
		}

		/// <summary>読みが必要なもの(ひらがな、カタカナ、漢字)とそうでないものに分割する</summary>
		public static void GetYomiWordList(string line, List<YomiWord> outList) {
			outList.Clear();
			int charLength = 1;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			bool isYomi1 = false;

			for (int i = 0, iCount = line.Length; i < iCount; i += charLength) {
				var isYomi2 = IsYomiTarget(line, i, out charLength);
				if (isYomi1 != isYomi2) {
					addIfNeeded(sb, isYomi1, outList);
					isYomi1 = isYomi2;
				}

				for (int ci = 0; ci < charLength; ++ci) {
					sb.Append(line[i + ci]);
				}
			}
			addIfNeeded(sb, isYomi1, outList);

			void addIfNeeded(System.Text.StringBuilder _sb, bool _isYomi, List<YomiWord> _outList) {
				if (_sb.Length <= 0) return;
				var item = new YomiWord() {
					isNeedYomi = _isYomi,
					word = _sb.ToString(),
				};
				_outList.Add(item);
				_sb.Clear();
			}
		}
	}
}
