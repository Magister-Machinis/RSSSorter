using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataFormats;
namespace RSSInterface
{
    public partial class MainWindow : Window
    {
        public class FeedResult : CSVLINES
        {
            public bool Selected {get; set; } = false;
        }
        public List<FeedResult> FeedResults { get; set; }

        private void Load_all_results_Click(object sender, RoutedEventArgs e)
        {
            Load_FeedResult(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "output", "MainList.csv")));
        }

        private void Load_select_results_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "output")),
                Filter = "CSV Files (*.csv)|*.csv",
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Select Feed list to view.",
                Multiselect = true
            };

            if (ofd.ShowDialog() != true)
            {
                return;
            }
            List<FeedResult> TempHolder = new List<FeedResult>();
            foreach(string filename in ofd.FileNames) 
            {
                Load_FeedResult(filename);
                TempHolder.AddRange(FeedResults);
            }
            FeedResults = TempHolder.DistinctBy(x => x.Url).ToList();

            FeedResultsDisplay.ItemsSource = null;
            FeedResultsDisplay.ItemsSource = FeedResults;
            
        }

        private void Load_FeedResult (string filepath)
        {
            using (new CursorWait())
            {
                using(StreamReader sr = new StreamReader(filepath))
                {
                    using (CsvReader csv = new CsvReader(sr, new CsvConfiguration(CultureInfo.InvariantCulture) 
                    {
                        MissingFieldFound = null, 
                        HeaderValidated=null 
                    }))
                    {

                        FeedResults = csv.GetRecords<FeedResult>().ToList();

                        FeedResultsDisplay.ItemsSource = null;
                        FeedResultsDisplay.ItemsSource = FeedResults;
                    }
                }
            }
        }

        private void Select_all_Entries_click(object sender, RoutedEventArgs e)
        {
            SelectItemsWithinTime(999);
        }
        private void Select_Some_Entries_click(object sender, RoutedEventArgs e)
        {
            SelectItemsWithinTime(int.Parse(Selectiontime.Text));
        }
        private void SelectItemsWithinTime(int days)
        {
            using(new CursorWait())
            {
                foreach(FeedResult item in  FeedResults)
                {
                    if(item.LastUpdate >  DateTime.Now.AddDays(days*-1))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }

                FeedResultsDisplay.ItemsSource = null;
                FeedResultsDisplay.ItemsSource = FeedResults;
            }
        }

        private void Load_Selected_Entries_Click(object sender, RoutedEventArgs e)
        {
            using (new CursorWait())
            {
                foreach (FeedResult item in FeedResults.Where(x => x.Selected ==true))
                {
                   Task.Run(() => Process.Start(new ProcessStartInfo("cmd", $"/c start {item.Url}")));
                }
            }
        }
    }
}
