using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Net.Http;

namespace RSSSorter
{
    class CSVLINES
    {
        public string title { get; set; }
        public string url { get; set; }
        public string snippet { get; set; }
        public string source { get; set; }
        public DateTime age { get; set; }
    }
    public class Program
    {
        static void helpmenu()
        {
            Console.WriteLine("params are as follows and must be entered in order:");
            Console.WriteLine("folderpath to lists of rss feed lists, each line separated list will create a normal and high value list for output. IE rssalerts folder has rssnews.txt and rssalerts.txt with rss feed urls in it to retrieve, this will create a normal and high value list for both source lists");
            Console.WriteLine("path to high value sources list. This is a line delimited txt file with the domains to be placed in the high value list rather than the normal list");
            Console.WriteLine("path to ignored sources list. This is a line delimited txt file with the domains to be excluded");
            Console.WriteLine("path to folder to store output");
            Console.WriteLine("number of days to keep entry on list, optional parameter, defaults to 30 days");
            Console.WriteLine("-h, -help, or /? will display this message");
        }

        /// <summary>
        /// read in and validate cmdline arguments before moving onto core functionality
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            string listfolder;
            string highvaluelist;
            string discardlist;
            string outputfolder;
            int agelimit = 30;
            //check for help menu
            if(args[0] == "-help" || args[0] == "/?" || args[0] == "-h")
            {
                helpmenu();
                return;
            }

            //validate folderpath for rss feed lists
            if(Directory.Exists(args[0]))
            {
                listfolder = args[0];
            }
            else
            {
                Console.WriteLine("Invalid folderpath for first parameter");
                helpmenu();
                return;
            }

            //validate highvalue list
            if (File.Exists(args[1]))
            {
                highvaluelist = args[1];
            }
            else
            {
                Console.WriteLine("Invalid filepath for second parameter");
                helpmenu();
                return;
            }
            
            //validate discard list
            if (File.Exists(args[2]))
            {
                discardlist = args[2];
            }
            else
            {
                Console.WriteLine("Invalid filepath for third parameter");
                helpmenu();
                return;
            }
            //validate folderpath for output
            if (Directory.Exists(args[3]))
            {
                outputfolder = args[3];
            }
            else
            {
                Console.WriteLine("Invalid folderpath for output folder");
                helpmenu();
                return;
            }
            //check for and validate agelimit
            if (args.Length==4)
            {
                if(int.TryParse(args[4], out agelimit))
                {}
                else
                {
                    Console.WriteLine("agelimit value not valid integer");
                    helpmenu();
                    return;
                }
            }

            Task<bool>[] tasks = Directory.GetFiles(listfolder, "*.txt").Select(async rssfile => await UpdateRSSlists(rssfile, highvaluelist, discardlist,outputfolder, agelimit)).ToArray();

            Task.WaitAll(tasks);
            if(tasks.All(i => i.Result==true))
            {
                Console.WriteLine("Processing completed successfully");
            }
            else
            {
                Console.WriteLine("Processing of one or more feed collections has failed.");
            }

        }

        /// <summary>
        /// either update an existing set of rss feeds lists, or create a new one, returns true if complete without error, else false and prints error message
        /// </summary>
        /// <param name="rssfile"></param>
        /// <param name="highvaluelist"></param>
        /// <param name="discardlist"></param>
        /// <param name="outputfolder"></param>
        /// <param name="agelimit"></param>
        /// <returns></returns>
        static async Task<bool> UpdateRSSlists(string rssfile, string highvaluelist, string discardlist,string outputfolder, int agelimit)
        {
            //determine if we are dealing with a new rss feed list or not
            try
            {
                if (File.Exists(Path.Combine(outputfolder, rssfile.Replace(".txt", ".csv"))))
                {
                    updatelist(rssfile, highvaluelist, discardlist, outputfolder, agelimit, new List<CSVLINES>(), new List<CSVLINES>());
                }
                else
                {
                    oldlist(rssfile, highvaluelist, discardlist, outputfolder, agelimit);
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// short detour for when there is a preexisting list to ingest first
        /// </summary>
        /// <param name="rssfile"></param>
        /// <param name="highvaluelist"></param>
        /// <param name="discardlist"></param>
        /// <param name="outputfolder"></param>
        /// <param name="agelimit"></param>
        static void oldlist(string rssfile, string highvaluelist, string discardlist, string outputfolder, int agelimit)
        {
            List<CSVLINES> csv;
            List<CSVLINES> csvhighval;
            using (StreamReader file = new StreamReader(Path.Combine(outputfolder, rssfile.Replace(".txt", ".csv"))))
            {
                using(CsvReader reader = new CsvReader(file,CultureInfo.InvariantCulture))
                {
                   csv= reader.GetRecords<CSVLINES>().ToList();
                }
            }
            using (StreamReader file = new StreamReader(Path.Combine(outputfolder, "HighValue-"+rssfile.Replace(".txt", ".csv"))))
            {
                using(CsvReader reader = new CsvReader(file,CultureInfo.InvariantCulture))
                {
                    csvhighval = reader.GetRecords<CSVLINES>().ToList();
                }
            }
            updatelist(rssfile, highvaluelist, discardlist, outputfolder, agelimit,csv, csvhighval);
        }

        /// <summary>
        /// core function to retrieve rss feeds, then update and print lists to files
        /// </summary>
        /// <param name="rssfile"></param>
        /// <param name="highvaluelist"></param>
        /// <param name="discardlist"></param>
        /// <param name="outputfolder"></param>
        /// <param name="agelimit"></param>
        /// <param name="csv"></param>
        /// <param name="csvhighval"></param>
        static void updatelist(string rssfile, string highvaluelist, string discardlist, string outputfolder, int agelimit, List<CSVLINES> csv, List<CSVLINES> csvhighval)
        {
            List<Task<CSVLINES[]>> newalerts = File.ReadAllLines(rssfile).Select(async rssurl => await GetRssUpdate(rssurl)).ToList();

            string[] highval = File.ReadAllLines(highvaluelist);
            string[] discard = File.ReadAllLines(discardlist);

            csv = AgeTrim(csv, agelimit);
            csvhighval = AgeTrim(csvhighval, agelimit);

            foreach(Task<CSVLINES[]> alerts in newalerts)
            {
                SortAlerts(alerts.Result, ref csv, ref csvhighval, ref highval, ref discard);
            }

            //write highval csv
            using (StreamWriter writer = new StreamWriter(Path.Combine(outputfolder, "HighValue-" + rssfile.Replace(".txt", ".csv"))))
            {
                using (CsvWriter csvwriter = new CsvWriter(writer,CultureInfo.InvariantCulture))
                {
                    csvwriter.WriteRecords(csvhighval.OrderBy(item => item.age));
                }
            }
            //write normal csv
            using (StreamWriter writer = new StreamWriter(Path.Combine(outputfolder, rssfile.Replace(".txt", ".csv"))))
            {
                using (CsvWriter csvwriter = new CsvWriter(writer,CultureInfo.InvariantCulture))
                {
                    csvwriter.WriteRecords(csv.OrderBy(item => item.age));
                }
            }
        }

        /// <summary>
        /// takes new list of alerts, discards any matches in discard list, then sort into either normal or highval lists
        /// </summary>
        /// <param name="alerts"></param>
        /// <param name="csv"></param>
        /// <param name="csvhighval"></param>
        /// <param name="highvaluelist"></param>
        /// <param name="discardlist"></param>
        static void SortAlerts(CSVLINES[] alerts, ref List<CSVLINES> csv, ref List<CSVLINES> csvhighval, ref string[] highval, ref string[] discard)
        {
            foreach(CSVLINES alert in alerts)
            {
                if(discard.All(i => !checkcontent(i, alert)))
                {
                    if(highval.Any(i => checkcontent(i, alert)))
                    {
                        if(csvhighval.Any(i => alert.url == i.url))
                        {
                            csvhighval[csvhighval.FindIndex(i => i.url == alert.url)].age = alert.age;
                        }
                        else
                        {
                            csvhighval.Add(alert);
                        }
                    }
                    else
                    {
                        if (csv.Any(i => alert.url == i.url))
                        {
                            csv[csv.FindIndex(i => i.url == alert.url)].age = alert.age;
                        }
                        else
                        {
                            csv.Add(alert);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// checks if the noted content is present in the url, description, or title
        /// </summary>
        /// <param name="particle">string to look for</param>
        /// <param name="line">csvline line item to check</param>
        /// <returns></returns>
        static bool checkcontent(string particle, CSVLINES line)
        {
            return (line.title.Contains(particle) || line.url.Contains(particle) || line.snippet.Contains(particle));
        }

        /// <summary>
        /// retrieve rss feed and parse into a list of CSVLINE objects. XML retrieval and parsing is pretty much copied from https://stackoverflow.com/questions/14904171/read-xml-from-url/14904529
        /// </summary>
        /// <param name="rssurl"></param>
        /// <returns></returns>
        static async Task<CSVLINES[]> GetRssUpdate(string rssurl)
        {
            List<CSVLINES> rssCsv = new List<CSVLINES>();
            using (XmlReader xmlReader = XmlReader.Create(new HttpClient().GetStreamAsync(rssurl).Result))
            {
                SyndicationFeed syndicationFeed = SyndicationFeed.Load(xmlReader);
                
                foreach(SyndicationItem item in syndicationFeed.Items)
                {
                    rssCsv.Add(new CSVLINES
                    {
                        title = item.Title.Text,
                        url = item.Links.ToString(),
                        snippet = item.Summary.Text,
                        source = syndicationFeed.Title.Text,
                        age = item.LastUpdatedTime.DateTime
                    });
                }

            }
            return rssCsv.ToArray();
        }

        

        /// <summary>
        /// small function to remove entries beyond the agelimit
        /// </summary>
        /// <param name="csv"></param>
        /// <param name="agelimit"></param>
        /// <returns></returns>
        static List<CSVLINES> AgeTrim(List<CSVLINES> csv, int agelimit)
        {
            return csv.Where(i => i.age < DateTime.Now.AddDays(agelimit*-1)).ToList();
        }
    }
}

    
