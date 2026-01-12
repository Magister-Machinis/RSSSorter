using DataFormats;
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
using System.Windows.Threading;

namespace RSSInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        RunLog runLog;
        const string msgtitle = "RSSGUI";
        public MainWindow()
        {
            runLog = new RunLog(@".\activitylog.csv");
            using (new CursorWait())
            {
                this.runLog.Writeline("Initializing GUI", msgtitle, Verbosity.INFO);
                
                InitializeComponent();

                //initial feedlist tab
                RssEntries = new List<RssData>();
                InitializeFeedTab();

                System.Windows.Application.Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
                //initialize filter lists
                HighvalFilters = new List<FilterItem>();
                DiscardFilters = new List<FilterItem>();
                InitializeFilterLists();
                
                //adding object references to click events for logging
                //Create_Feedlist.Click += (object sender, RoutedEventArgs e) => { Create_Feedlist_Click(sender, e,ref runLog); };
                
            }

        }


        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            runLog.Writeline($"Unhandled exception: {e.Exception.Message}", msgtitle, Verbosity.ERROR);
            System.Windows.MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            runLog.Writeline("Closing RSSGui", msgtitle, Verbosity.SUCCESS);
            using (new CursorWait())
            {
                runLog.SaveLog();
                if (HD_Save_List.IsEnabled == true)
                {
                    File.WriteAllLines(Highvalpath.Text, HighvalFilters.Select(x => x.Item).ToArray());
                    File.WriteAllLines(Discardpath.Text, DiscardFilters.Select(x => x.Item).ToArray());
                }
                if (Save_Feed_List.IsEnabled == true)
                {
                    File.WriteAllLines(FeedlistPath, RssEntries.Select(x => x.URL));
                }
            }
        }
    }
}
