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
        string testfile = @".\test.csv";
        [TestMethod()]
        public void RunLogTest()
        {
            try
            {
                RunLog runLog = new RunLog(testfile);
                Assert.IsNotNull(runLog);

                

                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete(testfile);
            }
        }

        [TestMethod()]
        public void GetLogTest()
        {
            try
            {
                RunLog runLog = new RunLog(testfile);
                Assert.IsNotNull(runLog);

                

                Assert.IsNotNull(RunLog.Log);

                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete(testfile);
            }
        }

        [TestMethod()]
        public void DisposeTest()
        {
            try
            {
                RunLog runLog = new RunLog(testfile);
                Assert.IsTrue(!runLog.IsDisposed);

               

                runLog.Dispose();

                Assert.IsTrue(runLog.IsDisposed);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete(testfile);
            }
        }

        [TestMethod()]
        public void WritelineTest()
        {
            try
            {
                RunLog runLog = new RunLog(testfile);
                Assert.IsNotNull(runLog);

                

                runLog.Writeline("test msg", "test source", Verbosity.DEBUG);

                Assert.IsTrue(RunLog.Log.Count == 1);

                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete(testfile);
            }
        }

        [TestMethod()]
        public void RemoveLineTest()
        {
            try
            {
                RunLog runLog = new RunLog(testfile);
                Assert.IsNotNull(runLog);

                

                runLog.Writeline("test msg", "test source", Verbosity.DEBUG);

                Assert.IsTrue(RunLog.Log.Count == 1);

                runLog.RemoveLine(0);

                Assert.IsTrue(RunLog.Log.Count == 0);

                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete(testfile);
            }
        }

        [TestMethod()]
        public void SaveLogTest()
        {
            try
            {
                RunLog runLog = new RunLog(testfile);
                Assert.IsNotNull(runLog);

                runLog.SaveLog();
                long lengthempty = new FileInfo(testfile).Length;

                runLog.Writeline("test msg", "test source", Verbosity.DEBUG);

                Assert.IsTrue(RunLog.Log.Count == 1);

                runLog.SaveLog();
                long lengthnotempty = new FileInfo(testfile).Length;

                Assert.IsTrue(lengthnotempty > lengthempty);

                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete(testfile);
            }
        }

        [TestMethod()]
        public void TrimLogTest()
        {
            try
            {
                RunLog runLog = new RunLog(testfile);
                Assert.IsNotNull(runLog);

                

                runLog.Writeline("test msg", "test source", Verbosity.DEBUG);

                Assert.IsTrue(RunLog.Log.Count == 1);
                Thread.Sleep(6000);
                runLog.TrimLog(DateTime.Now);
                Assert.IsTrue(RunLog.Log.Count == 0);
                runLog.Dispose();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                File.Delete(testfile);
            }
        }
    }
}