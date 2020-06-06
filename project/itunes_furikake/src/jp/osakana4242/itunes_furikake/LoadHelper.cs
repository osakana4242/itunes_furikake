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
    public static class LoadHelper
    {

        public static Dictionary<char, char> ReadDictC(string filename)
        {
            var d = ReadDictS(filename);
            var dict = new Dictionary<char, char>(d.Count);
            foreach (var kv in d)
            {
                if (kv.Key.Length <= 0) continue;
                if (kv.Value.Length <= 0) continue;
                dict[kv.Key[0]] = kv.Value[0];
            }
            return new Dictionary<char, char>(dict);
        }

        /** 辞書にファイルの内容を読み込む。
        */
        public static Dictionary<string, string> ReadDictS(string filename)
        {
            var dict = new Dictionary<string, string>();
            RubyAdder.logger.TraceEvent(TraceEventType.Information, 0, "ReadDict - " + filename);
            using (StreamReader r = new StreamReader(filename, System.Text.Encoding.UTF8))
            {
                string line;
                int lineCnt = 0;
                while ((line = r.ReadLine()) != null) // 1行ずつ読み出し。
                {
                    lineCnt += 1;
                    RubyAdder.logger.TraceEvent(TraceEventType.Verbose, 0, line);
                    string[] cols = line.Split('\t');
                    if (line.Length <= 0 || line.IndexOf("#") == 0)
                    {
                        continue;
                    }
                    if (cols.Length != 2)
                    {
                        // 不正な行。
                        if (cols.Length == 1)
                        {
                            throw new AppDisplayableException(makeReadDictErrorMessage(filename, lineCnt, line, "TABが不足しています。TABで区切ってください。"));
                        }
                        continue;
                    }
                    string key = cols[0];
                    string value = cols[1];
                    if (dict.ContainsKey(key))
                    {
                        throw new AppDisplayableException(makeReadDictErrorMessage(filename, lineCnt, line, string.Format("[{0}]が重複しています。", key)));
                    }
                    else
                    {
                        dict.Add(key, value);
                    }
                }
            }
            return new Dictionary<string, string>(dict);
        }

        public static string makeReadDictErrorMessage(string fileName, int lineCnt, string line, string message)
        {
			var path = System.IO.Path.GetFullPath(fileName);
			return message + "\r\n" +
				string.Format("該当場所: {0} - {1}行目 - {2}", path, lineCnt, line);
		}
	}
}
