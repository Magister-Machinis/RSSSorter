using System;

namespace DataFormats
{
    public class CSVLINES
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Snippet { get; set; }
        public string Source { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime FirstPosted { get; set; }

        public override bool Equals(object obj)
        {
            return ((CSVLINES)obj).Url == Url;
        }

        public override int GetHashCode()
        {
            return Url.GetHashCode();
        }
    }
}