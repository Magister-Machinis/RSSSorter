using System;
using System.Net.Http;
using System.Xml;
using System.ServiceModel.Syndication;

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
        //public string Snippet { get; set; }
        public string Source { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime FirstPosted { get; set; }



    }

    /// <summary>
    /// wrapper class to centralize rss/atom retrieval and parsing
    /// </summary>
    public class RSSHandler
    {
        public SyndicationFeed GetFeed(string url)
        {
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
            xmlReaderSettings.MaxCharactersFromEntities = 2048;
            using (XmlReader xmlReader = XmlReader.Create(new HttpClient().GetStreamAsync(url).Result, xmlReaderSettings))
            {

                return SyndicationFeed.Load(xmlReader);

            }
        }
    }
}