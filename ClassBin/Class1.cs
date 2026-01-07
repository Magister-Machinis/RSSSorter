using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.Swift;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;

namespace DataFormats
{
    public class RunLogLines
    {
        public DateTime TimeStamp { get; set; }
        public Verbosity Verbosity { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }

    public enum Verbosity
    {DEBUG, VERBOSE, INFO, NOTICE, WARNING, SUCCESS, ERROR, CRITICAL}
    public struct ResultStatus
    {
        public bool IsSuccess;
        public string message;
    }
    public class CSVLINES
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Snippet { get { return snippet; } set { snippet = RSSHandler.SanitizeString(value); } }
        public string Source { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime FirstPosted { get; set; }
        
        private string snippet;
    }

    /// <summary>
    /// class to manage an activity log, has the extra bits needed to save log to file automatically when program is closed
    /// </summary>
    public class RunLog : IDisposable
    {
        private string logpath;
        public List<RunLogLines> Log;

        private bool disposed = false;
        /// <summary>
        /// Init logger with path to csv formatted log, file does not need to exist yet
        /// </summary>
        /// <param name="logpath">path to save log, csv will be created at when first saved if it does not exist</param>
        public RunLog(string logpath)
        {
            this.logpath = logpath;
            
            if(File.Exists(this.logpath))
            {
                this.Log = this.GetLog().ToList<RunLogLines>();
            }
            else
            {
                this.Log = new List<RunLogLines>();
            }
        }
        ~RunLog() { Dispose(false); }
        /// <summary>
        /// retrieve current log from filepath
        /// </summary>
        /// <returns>Logs as array of objects</returns>
        public RunLogLines[] GetLog()
        {
            using (StreamReader sr = new StreamReader(this.logpath))
            {
                using (CsvReader csv = new CsvReader(sr, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound = null,
                    HeaderValidated = null
                }))
                {

                    return csv.GetRecords<RunLogLines>().ToArray<RunLogLines>();
                    
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.SaveLog();
                    disposed = true;
                }
            }
        }

        /// <summary>
        /// Add new line to log, timestamp is added automatically
        /// </summary>
        /// <param name="line">log message</param>
        /// <param name="source">source of message (interface, scraper, etc)</param>
        /// <param name="verbosity">verbosity enum</param>
        public void Writeline(string line, string source, Verbosity verbosity )
        {
            this.Log.Add(new RunLogLines()
            {
                Message = line,
                Source = source,
                Verbosity = verbosity,
                TimeStamp = DateTime.Now
            });
        }
        /// <summary>
        /// remove selected line from log
        /// </summary>
        /// <param name="linenumber">line to remove</param>
        public void RemoveLine(int linenumber)
        {
            this.Log.RemoveAt(linenumber);
        }
        public void SaveLog()
        {
            if (File.Exists(this.logpath)) { 
                File.Delete(this.logpath);
            }
            using (StreamWriter writer = new StreamWriter(this.logpath))
            {
                using (CsvWriter csvwriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csvwriter.WriteRecords(this.Log);
                }
            }
        }
        /// <summary>
        /// trim log entries older than set date
        /// </summary>
        /// <param name="cuttof">any lines older than this date will be removed from log</param>
        public void TrimLog (DateTime cuttof)
        {
            for(int i =0; i < this.Log.Count; i++)
            {
                if (this.Log[i].TimeStamp > cuttof)
                { 
                    this.RemoveLine(i);
                    i--;
                }
            }
        }

    }
    /// <summary>
    /// wrapper class to centralize rss/atom retrieval and parsing
    /// </summary>
    public class RSSHandler
    {
        /// <summary>
        /// strip out non-ascii characters, characters that could cause issues in windows cmdline, or html tags from string. Mostly used to sanitize rss snippets
        /// </summary>
        /// <param name="target">string to sanitize</param>
        /// <returns>sanitized string</returns>
        static public string SanitizeString(string target)
        {
            Regex nonascii = new Regex("<.*?>|&nbsp|&amp|[^\x00-\x7E]|[,^&|()<>]", RegexOptions.Compiled);
            return nonascii.Replace(target, "-");
        }
        public SyndicationFeed GetFeed(string url)
        {
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
            xmlReaderSettings.MaxCharactersFromEntities = 2048;
            HttpClient httpClient = new HttpClient();
            try
            {
                
                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                using (XmlReader xmlReader = XmlReader.Create(httpClient.GetStreamAsync(url).Result, xmlReaderSettings))
                {
                    return SyndicationFeed.Load(xmlReader);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                httpClient.Dispose();
            }
        }
    }
}