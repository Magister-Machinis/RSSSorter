using System;
using System.Net.Http;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;

namespace DataFormats
{
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