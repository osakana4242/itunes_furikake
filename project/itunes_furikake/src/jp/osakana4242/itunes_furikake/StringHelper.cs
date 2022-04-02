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
	public class StringHelper {
		/// <summary>指定文字列に含まれるのがアスキーコードのみなら true</summary>
		public static bool IsAscii(string str) {
			foreach (var c in str) {
				if (!IsAscii(c)) {
					return false;
				}
			}
			return true;
		}

		public static bool IsAscii(char c) {
			return c <= 0x7f;
		}

		// 文字がひらがなか、カタカナか、漢字か、英数字か調べる.
		// from: https://dobon.net/vb/dotnet/string/ishiragana.html

		/// <summary>指定した文字列の指定位置にある文字が漢字かどうかを示します。</summary>
		/// <returns>s の index の位置にある文字が漢字の場合は true。
		/// それ以外の場合は false。</returns>
		public static bool IsKanji(string s, int index) {
			if (s == null) {
				throw new ArgumentNullException("s");
			}
			if (s.Length <= index) {
				throw new ArgumentException("index が s 内にない位置です。");
			}

			char c1 = s[index];
			if (char.IsHighSurrogate(c1)) {
				if (s.Length - 1 <= index) {
					return false;
				}

				char c2 = s[index + 1];
				//CJK統合漢字拡張Bの範囲にあるか調べる
				return (('\uD840' <= c1 && c1 < '\uD869') && char.IsLowSurrogate(c2)) ||
						(c1 == '\uD869' && ('\uDC00' <= c2 && c2 <= '\uDEDF'));
			} else {
				//CJK統合漢字、CJK互換漢字、CJK統合漢字拡張Aの範囲にあるか調べる
				return ('\u4E00' <= c1 && c1 <= '\u9FCF') ||
					   ('\uF900' <= c1 && c1 <= '\uFAFF') ||
					   ('\u3400' <= c1 && c1 <= '\u4DBF');
			}
		}

		/// <summary>指定した Unicode 文字が、ひらがなかどうかを示します.</summary>
		/// <returns>c がひらがなである場合は true。それ以外の場合は false。</returns>
		public static bool IsHiragana(char c) {
			//「ぁ」～「より」までをひらがなとする
			return ('\u3041' <= c && c <= '\u309F');
		}

		/// <summary>指定した Unicode 文字が、カタカナかどうかを示します.</summary>
		/// <returns>c がカタカナである場合は true。それ以外の場合は false。</returns>
		public static bool IsKatakana(char c) {
			//「ァ」から「コト」までと、カタカナフリガナ拡張と、
			//濁点と半濁点と、半角カタカナをカタカナとする
			return
				'\u30A0' != c && // ダブルハイフン「゠」を除外.
				'\u30FB' != c && // 中点「・」を除外.
				'\u30FC' != c && // 長音記号「ー」を除外.
				(
					('\u30A1' <= c && c <= '\u30FF') ||
					('\u31F0' <= c && c <= '\u31FF') ||
					('\u3099' <= c && c <= '\u309C') ||
					('\uFF65' <= c && c <= '\uFF9F')
				);
		}
	}
}
