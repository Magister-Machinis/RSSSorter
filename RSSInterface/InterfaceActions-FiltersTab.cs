using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RSSInterface
{
    public partial class MainWindow : Window
    {
        public List<FilterItem> HighvalFilters { get; set; }
        public List<FilterItem> DiscardFilters { get; set; }
        private void InitializeFilterLists()
        {

            //confirm or initiatilize high value filters
            if (File.Exists(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Highval.txt")))
            {
                string[] highvalitems = File.ReadAllLines(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Highval.txt"));
                
                foreach (string item in highvalitems)
                {
                    HighvalFilters.Add(new FilterItem()
                    {
                        Item = item,
                        Selected = false
                    });
                }

                HighValFilterDisplay.ItemsSource = null;
                HighValFilterDisplay.ItemsSource = HighvalFilters;
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
                
                foreach (string item in Discarditems)
                {
                    DiscardFilters.Add(new FilterItem()
                    {
                        Item = item,
                        Selected = false
                    });
                }
                DiscardFiltersDisplay.ItemsSource = null;
                DiscardFiltersDisplay.ItemsSource = DiscardFilters;
                
            }
            else
            {
                File.Create(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Discard.txt"));
            }
            Discardpath.Text = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Discard.txt");

        }

        private void Add_HighVal_Click(object sender, RoutedEventArgs e)
        {
            string input=(Microsoft.VisualBasic.Interaction.InputBox("Input regex for titles/urls to be flagged as high value.", "New High Value Filter", "Place regex here.")).Trim();

            if (input != "Place regex here." && !string.IsNullOrEmpty(input))
            {
                HighvalFilters.Add(new FilterItem()
                {
                    Item = input,
                    Selected = false
                });
                HighvalFilters = HighvalFilters.DistinctBy(x => x.Item).ToList();
                    
                HighValFilterDisplay.ItemsSource = null;
                HighValFilterDisplay.ItemsSource = HighvalFilters;
            }
        }

        private void Add_Discard_Click(object sender, RoutedEventArgs e)
        {
            string input = (Microsoft.VisualBasic.Interaction.InputBox("Input regex for titles/urls to be flagged as high value.", "New High Value Filter", "Place regex here.")).Trim();

            if (input != "Place regex here." && !string.IsNullOrEmpty(input))
            {
                DiscardFilters.Add(new FilterItem()
                {
                    Item = input,
                    Selected = false
                });
                DiscardFilters = DiscardFilters.DistinctBy(x => x.Item).ToList();

                DiscardFiltersDisplay.ItemsSource = null;
                DiscardFiltersDisplay.ItemsSource = DiscardFilters;
            }
        }

        private void Delete_Selected_Filters_Click(object sender, RoutedEventArgs e)
        {

            for(int i = 0; i < HighvalFilters.Count; i++)
            {
                if (HighvalFilters[i].Selected)
                {
                    HighvalFilters.RemoveAt(i);
                }
            }

            for (int i = 0; i < DiscardFilters.Count; i++)
            {
                if (DiscardFilters[i].Selected)
                {
                    DiscardFilters.RemoveAt(i);
                }
                

            }


            HighValFilterDisplay.ItemsSource = null;
            HighValFilterDisplay.ItemsSource = HighvalFilters;

            DiscardFiltersDisplay.ItemsSource = null;
            DiscardFiltersDisplay.ItemsSource = DiscardFilters;
        }

        private void Save_Filter_Lists_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllLines(Highvalpath.Text, HighvalFilters.Select(x  => x.Item).ToArray());
            File.WriteAllLines(Discardpath.Text, DiscardFilters.Select(x  => x.Item).ToArray());
        }
    }
}
