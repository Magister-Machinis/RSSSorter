using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSSSorter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSSorter.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void MainTest()
        {
            string[] rssurls = { "https://www.google.com/alerts/feeds/05999718615320080555/4574872169044351658", "https://krebsonsecurity.com/feed/", "https://www.google.com/alerts/feeds/05999718615320080555/11815055537862344919", "https://hachyderm.io/@shortridge.rss" };
            try
            {
                Directory.CreateDirectory(@".\input");
                File.WriteAllLines(@".\input\rssurls.txt", rssurls);                
                Directory.CreateDirectory(@".\output");
                File.WriteAllText(@".\discard.txt", "game");
                File.WriteAllText(@".\highval.txt", "hacked");
                Console.WriteLine(Path.GetFullPath("./"));
                RSSSorter.Program.Main(new string[] { @".\input", @".\highval.txt", @".\discard.txt", @".\output", "30" });
                RSSSorter.Program.Main(new string[] { @".\input", @".\highval.txt", @".\discard.txt", @".\output", "30" });
                Assert.AreEqual(true, true);
            }
            finally
            {
                Directory.Delete(@".\input", true);
                Directory.Delete(@".\output", true);
                File.Delete(@".\discard.txt");
                File.Delete(@".\highval.txt");
            }
        }
    }
}
