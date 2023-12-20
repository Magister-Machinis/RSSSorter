using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RSSInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<RssData> RssEntries { get; init; }
        public List<FilterItem> HighvalFilters { get; init; }
        public List<FilterItem> DiscardFilters { get; init; }
        public MainWindow()
        {
            InitializeComponent();

            RssEntries= new List<RssData>()
            { new RssData() {
            URL = "example.com/rss",
            Title  = "Create or load rss list to populate this table.",
            Selected = true,
            } };

            //confirm or initiatilize feed list folder
            if(!Directory.Exists(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "RssLists")))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "RssLists"));
            }

            //confirm or initiatilize high value filters
            if (File.Exists(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Highval.txt")))
            {
                string[] highvalitems = File.ReadAllLines(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Highval.txt"));
                HighvalFilters= new List<FilterItem>();
                foreach(string item in highvalitems)
                {
                    HighvalFilters.Add(new FilterItem()
                    {
                        Item = item,
                        Selected = false
                    });
                }
            }
            else
            {
                File.Create(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Highval.txt"));
            }
            Highvalpath.Text = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Highval.txt");

            //confirm or initiatilize discard value filters
            if (File.Exists(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Discard.txt")))
            {
                string[] Discarditems = File.ReadAllLines(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Discard.txt"));
                DiscardFilters = new List<FilterItem>();
                foreach (string item in Discarditems)
                {
                    DiscardFilters.Add(new FilterItem()
                    {
                        Item = item,
                        Selected = false
                    });
                }
            }
            else
            {
                File.Create(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Discard.txt"));
            }
            Discardpath.Text = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Discard.txt");
        }

       

        private void RestricttoNumbers(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

       
    }

    public class RssData
    {
        public string URL { get; set; }
        public string Title { get; set; }
        public bool Selected { get; set; }
    }
    public class FilterItem
    {
        public string Item { get; set; }
        public bool Selected { get; set; }
    }
}
