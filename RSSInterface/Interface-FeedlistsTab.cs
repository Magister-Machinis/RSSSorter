using CsvHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml;

namespace RSSInterface
{
    public partial class MainWindow : Window
    {
        public class RssData
        {
            public string URL { get; set; }
            public string Title { get; set; }
            public bool Selected { get; set; }
        }
        public List<RssData> RssEntries { get; set; }

        private string FeedlistPath;
        

        private void InitializeFeedTab()
        {

            RssEntries.Add(new RssData()
            {
                URL = "example.com/rss",
                Title = "Create or load rss list to populate this table.",
                Selected = true,
            });

            RSSEntriesDisplay.ItemsSource = null;
            RSSEntriesDisplay.ItemsSource = RssEntries;


            //confirm or initiatilize feed list folder
            if (!Directory.Exists(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "RssLists")))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "RssLists"));
            }

        }

        private void Create_Feedlist_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "RssLists")),
                Filter = "Text Files (*.txt)|*.txt",
                CheckFileExists = false,
                CheckPathExists = true,
                Title = "Select Feed list to create."
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }
            using (new CursorWait())
            {
                Save_Feed_List.IsEnabled = true;
                Add_Item_Feed_List.IsEnabled = true;
                Remove_Item_Feed_List.IsEnabled = true;
                Retrieve_Item_Feed_Info.IsEnabled = true;

                File.Create(saveFileDialog.FileName);
                FeedlistPath = saveFileDialog.FileName;
            }
        }

        private void Load_Feedlist_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "RssLists")),
                Filter = "Text Files (*.txt)|*.txt",
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Select Feed list to view.",
                Multiselect = false
            };

            if (ofd.ShowDialog() != true)
            {
                return;
            }
            using (new CursorWait())
            {
                Save_Feed_List.IsEnabled = true;
                Add_Item_Feed_List.IsEnabled = true;
                Remove_Item_Feed_List.IsEnabled = true;
                Retrieve_Item_Feed_Info.IsEnabled = true;

                LoadFeedlistFromfile(ofd.FileName);
                RSSEntriesDisplay.ItemsSource = null;
                RSSEntriesDisplay.ItemsSource = RssEntries;

                FeedlistPath = ofd.FileName;
            }
        }

        private void LoadFeedlistFromfile(string fileName)
        {
            RssEntries.Clear();
            foreach (string line in File.ReadLines(fileName))
            {
                RssEntries.Add(new RssData()
                {
                    Selected = false,
                    Title = "",
                    URL = line
                });
            }
        }

        private void Save_Feedlist_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllLines(FeedlistPath, RssEntries.Select(x => x.URL));
        }

        private void Add_Item_Feedlist_Click(object sender, RoutedEventArgs e)
        {
            string input = (Microsoft.VisualBasic.Interaction.InputBox("Input new rss/atom url.", "New Feed List", "Place url here.")).Trim();
            using (new CursorWait())
            {
                if (input != "Place url here." && !string.IsNullOrEmpty(input))
                {
                    RssEntries.Add(new RssData()
                    {
                        Selected = false,
                        Title = "",
                        URL = input
                    });

                    RSSEntriesDisplay.ItemsSource = null;
                    RSSEntriesDisplay.ItemsSource = RssEntries;
                }
            }
        }

        private void Remove_Checked_Feedlist_Click(object sender, RoutedEventArgs e)
        {
            using (new CursorWait())
            {
                RssEntries.RemoveAll(x => x.Selected);
                RSSEntriesDisplay.ItemsSource = null;
                RSSEntriesDisplay.ItemsSource = RssEntries;
            }
        }

        private void Retrieve_Feedlist_Info(object sender, RoutedEventArgs e)
        {
            using (new CursorWait())
            {
                //string[] tasks = new string[RssEntries.Count-1];

                //for (int i = 0; i < RssEntries.Count-1; i++)
                //{
                //    Task<string> task = new Task<string>(() => FetchFeedItemInfo(i));
                //    task.Start();
                //    tasks[i]=task;                  
                //}
                //Task.WaitAll(tasks);
                for (int i = 0; i < RssEntries.Count ; i++)
                {
                    //tasks[i].Wait();
                    //RssEntries[i].Title = tasks[i].Result;
                    RssEntries[i].Title = FetchFeedItemInfo(i);
                }

                RSSEntriesDisplay.ItemsSource = null;
                RSSEntriesDisplay.ItemsSource = RssEntries;
            }
        }

         private string FetchFeedItemInfo(int i)
        {
            try
            {
                using (XmlReader xmlReader = XmlReader.Create(new HttpClient().GetStreamAsync(RssEntries[i].URL).Result))
                {
                    XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                    xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
                    xmlReaderSettings.MaxCharactersFromEntities = 2048;
                    SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
                    return feed.Title.Text;
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
            
        }
    }
}
