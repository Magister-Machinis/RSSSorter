using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RSSInterface
{
    public partial class MainWindow : Window
    {
        const string taskname = "RSStask";
        private void Set_Scheduled_Task_Click(object sender, RoutedEventArgs e)
        {
            Remove_Scheduled_Task_Click(sender, e);

            int hour = RuntimeHours.Text != "Hour(24h)" ? int.Parse(RuntimeHours.Text) : 1,
                minute = RunTimeMinutes.Text != "Minute" ? int.Parse(RunTimeMinutes.Text) : 0; 
            string days = Retentiontime.Text != "Days to retain entries" ? Retentiontime.Text: "30";
            
            Microsoft.Win32.TaskScheduler.Task scheduledtask;

            ExecAction exectask = new ExecAction() 
            {
                Path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),"runcmd.cmd")),
                Arguments  = days,
                WorkingDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory()))
            };

            switch (((ComboBoxItem)ScheduleTiming.SelectedItem).Content.ToString())
            {
                case "Weekly":
                    scheduledtask = new TaskService().AddTask(taskname, new WeeklyTrigger(DaysOfTheWeek.Sunday) { StartBoundary = DateTime.Today + TimeSpan.FromHours(hour) + TimeSpan.FromMinutes(minute)}, exectask);
                    break;
                case "Monthly":
                    scheduledtask = new TaskService().AddTask(taskname, new MonthlyTrigger(1) { StartBoundary = DateTime.Today + TimeSpan.FromHours(hour) + TimeSpan.FromMinutes(minute) }, exectask);
                    break;
                default:
                    scheduledtask = new TaskService().AddTask(taskname, new DailyTrigger(1) { StartBoundary = DateTime.Today + TimeSpan.FromHours(hour) + TimeSpan.FromMinutes(minute) }, exectask);
                    break;
            }

            scheduledtask.RegisterChanges();
            scheduledtask.Dispose();
            RunTaskNow.IsEnabled=true;
        }
        private bool Check_if_Task_exists()
        {
            using (Microsoft.Win32.TaskScheduler.Task task = new TaskService().GetTask(taskname))
            {
                return task != null;
            }
            
        }
        private void Remove_Scheduled_Task_Click(object sender, RoutedEventArgs e)
        {
            if (Check_if_Task_exists())
            {
                using(TaskService ts = new TaskService())
                {
                    ts.RootFolder.DeleteTask(taskname);
                }
                RunTaskNow.IsEnabled = false;
            }
        }

        private void Run_now_Click(object sender, RoutedEventArgs e)
        {
            using (new CursorWait())
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c schtasks /run /tn {taskname}"));
            }
        }
    }
}
