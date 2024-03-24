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
	public static class ConvertHelper {
		static readonly char[] extraWhiteSpaces = { '　' };

		public static string MakeSortField(RubyAdder rubyAdder, string baseField) {
			if (baseField.Length <= 0) return "";

			string ruby = "";
			// 単語リストから変換。
			if (!rubyAdder.dictWord2Hiragana.TryGetValue(baseField, out ruby)) {
				ruby = baseField;
			}

			var rubyType = rubyAdder.opeData.setting.rubyAdd.rubyType;
			switch (rubyType) {
				case RubyAdderRubyType.Hiragana:
					ruby = ToHiragana(rubyAdder, ruby);
					break;
				case RubyAdderRubyType.Alphabet:
					ruby = ToAlphabet(rubyAdder, ruby);
					break;
				default:
					throw new System.NotSupportedException($"rubyType: {rubyType}");
			}
			return ruby;
		}

		/// <summary>ひらがな化.</summary>
		public static string ToHiragana(RubyAdder rubyAdder, string src) {
			string dest;
			if (StringHelper.IsAscii(src)) {
				// 読みがな不要.
				return "";
			} else {
				// 読みをふる対象を漢字、ひらがな、カタカナに限定する.
				using (TemporaryListPool<YomiWord>.instance_g.Alloc(out var wordList)) {
					YomiWordUtil.GetYomiWordList(src, wordList);
					var sb = new System.Text.StringBuilder();
					for (int i = 0, iCount = wordList.Count; i < iCount; ++i) {
						YomiWord item = wordList[i];
						if (item.isNeedYomi) {
							var word2 = rubyAdder.imeLanguage.GetYomi(item.word);
							sb.Append(word2);
						} else {
							sb.Append(item.word);
						}
					}
					dest = sb.ToString();
					dest = ToHankaku(rubyAdder, dest);
				}
			}
			return dest;
		}

		/// <summary>アルファベット化.</summary>
		public static string ToAlphabet(RubyAdder rubyAdder, string src) {
			string hiragana = ToHiragana(rubyAdder, src);
			return ToHoge(hiragana, rubyAdder.dictHiragana2Rome);
		}

		public static string ToHankaku(RubyAdder rubyAdder, string src) {
			return ToHankaku(rubyAdder.dictZen2Han, src);
		}

		/// <summary>全角半角.</summary>
		public static string ToHankaku(Dictionary<char, char> dictZen2Han, string src) {
			var sb = new System.Text.StringBuilder(src.Length);
			foreach (char c in src) {
				char nextC;
				if (!dictZen2Han.TryGetValue(c, out nextC)) {
					nextC = c;
				}
				sb.Append(nextC);
			}
			return sb.ToString();
		}

		/// <summary>両端の空白を除去する.</summary>
		public static string Trim(RubyAdder rubyAdder, string src) {
			var trimedAsciiWhiteSpace = src.Trim();
			var trimedExtraWhiteSpace = trimedAsciiWhiteSpace.Trim(extraWhiteSpaces);
			return trimedExtraWhiteSpace;
		}

		/// <summary>指定文字列を指定の辞書で置換.</summary>
		public static string ToHoge(string src, Dictionary<string, string> hToHoge) {
			StringBuilder sb = new StringBuilder();
			foreach (char c in src) {
				string s = c.ToString();
				string outS = s;
				if (!hToHoge.TryGetValue(s, out outS)) {
					outS = s;
				}
				sb.Append(outS);
			}
			return sb.ToString();
		}

		/// <summary>
		/// ダミー文字を混じえた変更が必要か.
		/// iTunes で文字種の変更だけでは更新が無かったことにされてしまう問題の対策用.
		/// </summary>
		public static bool IsNeedSetDummyField(RubyAdder rubyAdder, string a, string b) {
			if (ToHankaku(rubyAdder, a) != ToHankaku(rubyAdder, b)) return false;
			return true;
		}
	}
}
