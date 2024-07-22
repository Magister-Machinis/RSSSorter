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
using System.Windows.Forms;
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


        public MainWindow()
        {
            using (new CursorWait())
            {
                InitializeComponent();

                //initial feedlist tab
                RssEntries = new List<RssData>();
                InitializeFeedTab();




                //initialize filter lists
                HighvalFilters = new List<FilterItem>();
                DiscardFilters = new List<FilterItem>();
                InitializeFilterLists();
            }

        }



        private void RestricttoNumbers(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private class CursorWait: IDisposable
        {
            public CursorWait()
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            }
            public void Dispose()
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
        }

        private void Segmented_Checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (SegmentSize.IsEnabled)
            {
                SegmentSize.IsEnabled = false;
            }
            else
            {
                SegmentSize.IsEnabled = true;
                SegmentSize.Text = "25";
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBlock.Text = "Continuing";
            ContinueButton.IsEnabled = false;
        }
        
    }
}
