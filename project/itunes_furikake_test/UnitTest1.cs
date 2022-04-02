
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using jp.osakana4242.itunes_furikake;

namespace jp.osakana4242.itunes_furikake_test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetYomiTest()
        {
            using (var ime = new emanual.IME.ImeLanguage())
            {
                // ～ が「から」にされてしまう問題の確認.
                // ほかの記号はどうだろう...
                System.Console.Write($"~ to {ime.GetYomi("~")}\n");
                System.Console.Write($"～ to {ime.GetYomi("～")}\n");
                System.Console.Write($"▲ to {ime.GetYomi("▲")}\n");
                System.Console.Write($"↑ to {ime.GetYomi("↑")}\n");
                System.Console.Write($"！ to {ime.GetYomi("！")}\n");
                System.Console.Write($"？ to {ime.GetYomi("？")}\n");
                System.Console.Write($"￥ to {ime.GetYomi("￥")}\n");
                System.Console.Write($"₌ to {ime.GetYomi("₌")}\n");
                System.Console.Write($"― to {ime.GetYomi("―")}\n");

                // ~ to から
                // ～ to ～
                // ▲ to さんかく
                // ↑ to やじるし
                // ！ to ！
                // ？ to ？
                // ￥ to ￥
                // ₌ to ₌
                // ― to ー
            }

        }

        [TestMethod]
        public void GetYomiListTest()
        {
            var list = new List<YomiWord>();
            YomiWordUtil.GetYomiWordList("漢字だよ~漢字だよ~", list);
            Assert.AreEqual(list[0].word, "漢字だよ");
            Assert.AreEqual(list[0].isNeedYomi, true);
            Assert.AreEqual(list[1].word, "~");
            Assert.AreEqual(list[1].isNeedYomi, false);
            Assert.AreEqual(list[2].word, "漢字だよ");
            Assert.AreEqual(list[2].isNeedYomi, true);
            Assert.AreEqual(list[3].word, "~");
            Assert.AreEqual(list[3].isNeedYomi, false);

            YomiWordUtil.GetYomiWordList("漢字だよ～漢字だよ～", list);
            Assert.AreEqual(list[0].word, "漢字だよ");
            Assert.AreEqual(list[0].isNeedYomi, true);
            Assert.AreEqual(list[1].word, "～");
            Assert.AreEqual(list[1].isNeedYomi, false);
            Assert.AreEqual(list[2].word, "漢字だよ");
            Assert.AreEqual(list[2].isNeedYomi, true);
            Assert.AreEqual(list[3].word, "～");
            Assert.AreEqual(list[3].isNeedYomi, false);

            YomiWordUtil.GetYomiWordList("サロゲートペアの漢字𠀋𡈽𡌛だよ", list);
            Assert.AreEqual(list[0].word, @"サロゲ");
            Assert.AreEqual(list[0].isNeedYomi, true);
            Assert.AreEqual(list[1].word, @"ー");
            Assert.AreEqual(list[1].isNeedYomi, false);
            Assert.AreEqual(list[2].word, "トペアの漢字𠀋𡈽𡌛だよ");
            Assert.AreEqual(list[2].isNeedYomi, true);
        }
    }
}
