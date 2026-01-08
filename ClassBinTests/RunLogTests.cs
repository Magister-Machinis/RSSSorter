using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;

namespace DataFormats.Tests
{
    [TestClass()]
    public class RunLogTests
    {
        [TestMethod()]
        public void RunLogTest()
        {
            try
            {
                RunLog runLog = new RunLog("./test.csv");
                Assert.IsNotNull(runLog);

                Assert.IsTrue(File.Exists("./test.csv"));

                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete("./test.csv");
            }
        }

        [TestMethod()]
        public void GetLogTest()
        {
            try
            {
                RunLog runLog = new RunLog("./test.csv");
                Assert.IsNotNull(runLog);

                Assert.IsTrue(File.Exists("./test.csv"));

                Assert.IsNotNull(runLog.Log);

                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete("./test.csv");
            }
        }

        [TestMethod()]
        public void DisposeTest()
        {
            try
            {
                RunLog runLog = new RunLog("./test.csv");
                Assert.IsNotNull(runLog);

                Assert.IsTrue(File.Exists("./test.csv"));

                runLog.Dispose();

                Assert.IsNull(runLog);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete("./test.csv");
            }
        }

        [TestMethod()]
        public void WritelineTest()
        {
            try
            {
                RunLog runLog = new RunLog("./test.csv");
                Assert.IsNotNull(runLog);

                Assert.IsTrue(File.Exists("./test.csv"));

                runLog.Writeline("test msg", "test source", Verbosity.DEBUG);

                Assert.IsTrue(runLog.Log.Count == 1);

                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete("./test.csv");
            }
        }

        [TestMethod()]
        public void RemoveLineTest()
        {
            try
            {
                RunLog runLog = new RunLog("./test.csv");
                Assert.IsNotNull(runLog);

                Assert.IsTrue(File.Exists("./test.csv"));

                runLog.Writeline("test msg", "test source", Verbosity.DEBUG);

                Assert.IsTrue(runLog.Log.Count == 1);

                runLog.RemoveLine(0);

                Assert.IsTrue(runLog.Log.Count == 0);

                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete("./test.csv");
            }
        }

        [TestMethod()]
        public void SaveLogTest()
        {
            try
            {
                RunLog runLog = new RunLog("./test.csv");
                Assert.IsNotNull(runLog);

                Assert.IsTrue(File.Exists("./test.csv"));
                long lengthempty = new FileInfo("./test.csv").Length;

                runLog.Writeline("test msg", "test source", Verbosity.DEBUG);

                Assert.IsTrue(runLog.Log.Count == 1);

                runLog.SaveLog();
                long lengthnotempty = new FileInfo("./test.csv").Length;

                Assert.IsTrue(lengthnotempty > lengthempty);

                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete("./test.csv");
            }
        }

        [TestMethod()]
        public void TrimLogTest()
        {
            try
            {
                RunLog runLog = new RunLog("./test.csv");
                Assert.IsNotNull(runLog);

                Assert.IsTrue(File.Exists("./test.csv"));

                runLog.Writeline("test msg", "test source", Verbosity.DEBUG);

                Assert.IsTrue(runLog.Log.Count == 1);
                runLog.TrimLog(DateTime.Now);
                Assert.IsTrue(runLog.Log.Count == 0);
                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete("./test.csv");
            }
        }
    }
}