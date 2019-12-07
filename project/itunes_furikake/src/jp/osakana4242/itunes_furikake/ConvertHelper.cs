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
    public static class ConvertHelper
    {
        public static string MakeSortField(RubyAdder rubyAdder, string baseField)
        {
            if (baseField.Length <= 0) return "";

            string ruby = "";
            // 単語リストから変換。
            if (!rubyAdder.dictWord2Hiragana.TryGetValue(baseField, out ruby))
            {
                ruby = baseField;
            }

            switch (rubyAdder.opeData.ope)
            {
                case RubyAdderOpeType.HIRAGANA:
                    ruby = ToHiragana(rubyAdder, ruby);
                    break;
                case RubyAdderOpeType.KATAKANA:
                    ruby = ToKatakana(rubyAdder, ruby);
                    break;
                case RubyAdderOpeType.ALPHABET:
                    ruby = ToAlphabet(rubyAdder, ruby);
                    break;
            }
            return ruby;
        }

        /** ひらがな化。
        */
        public static string ToHiragana(RubyAdder rubyAdder, string src)
        {
            string dest;
            if (StringHelper.IsAscii(src))
            {
                // 読みがな不要.
                return "";
            }
            else
            {
                dest = rubyAdder.imeLanguage.GetYomi(src);
                dest = ToHankaku(rubyAdder, dest);
            }
            return dest;
        }

        /** カタカナ化。
        */
        public static string ToKatakana(RubyAdder rubyAdder, string src)
        {
            string hiragana = ToHiragana(rubyAdder, src);
            return ToHoge(hiragana, rubyAdder.dictHiragana2Katakana);
        }

        /** アルファベット化。
        */
        public static string ToAlphabet(RubyAdder rubyAdder, string src)
        {
            string hiragana = ToHiragana(rubyAdder, src);
            return ToHoge(hiragana, rubyAdder.dictHiragana2Rome);
        }

        public static string ToHankaku(RubyAdder rubyAdder, string src)
        {
            return ToHankaku(rubyAdder.dictZen2Han, src);
        }

        /** 全角半角
        */
        public static string ToHankaku(Dictionary<char, char> dictZen2Han, string src)
        {
            var sb = new System.Text.StringBuilder(src.Length);
            foreach (char c in src)
            {
                char nextC;
                if (!dictZen2Han.TryGetValue(c, out nextC))
                {
                    nextC = c;
                }
                sb.Append(nextC);
            }
            return sb.ToString();
        }

        /** 指定文字列を指定の辞書で置換.
       */
        public static string ToHoge(string src, Dictionary<string, string> hToHoge)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in src)
            {
                string s = c.ToString();
                string outS = s;
                if (!hToHoge.TryGetValue(s, out outS))
                {
                    outS = s;
                }
                sb.Append(outS);
            }
            return sb.ToString();
        }

        /** ダミー文字を混じえた変更が必要か. */
        public static bool IsNeedSetDummyField(RubyAdder rubyAdder, string a, string b)
        {
            if (ToHankaku(rubyAdder, a) != ToHankaku(rubyAdder, b)) return false;
            return true;
        }
    }
}
