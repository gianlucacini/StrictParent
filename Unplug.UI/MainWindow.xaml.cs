using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
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
using Unplug.Common;

namespace Unplug.UI
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WCFClient WCFClient;

        StatusType _currentStatus;
        public StatusType CurrentStatus
        {
            get
            {
                return _currentStatus;
            }
            set
            {

                if (value == StatusType.Loading || value == StatusType.Running)
                {
                    ToogleInput(false);
                }
                else
                {
                    ToogleInput(true);
                }

                _currentStatus = value;
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            InitializeWCFClient();

            InitializeAppStatus();

            this.Title += " V" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        }

        private void InitializeWCFClient()
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            EndpointAddress address = new EndpointAddress("http://localhost:8022/UPService");

            WCFClient = new WCFClient(binding, address);
        }

        private void ToogleInput(Boolean enabled)
        {
            ButtonSaveSettings.IsEnabled = enabled;
            RunUntil.IsEnabled = enabled;
            FromTime.IsEnabled = enabled;
            ToTime.IsEnabled = enabled;
            ComboCriticalNo.IsEnabled = enabled;
            ComboCriticalYes.IsEnabled = enabled;
            ViewUninstall.IsEnabled = enabled;
        }

        public enum StatusType
        {
            Loading,
            NotRunning,
            Running
        }

        private async void InitializeAppStatus()
        {

            CurrentStatus = StatusType.Loading;

            this.Dispatcher.Invoke(() =>
            {
                SettingsEllipse.Fill = Brushes.Gray;
                SettingsMessage.Text = "loading...";

                AppStatusEllipse.Fill = Brushes.Gray;
                AppStatusMessage.Text = "loading...";
            });
            SettingsDto settings;
            try
            {
                settings = WCFClient.GetSettings() as SettingsDto;
            }
            catch (Exception ex)
            {
                throw;
            }

            if (settings.UnplugFrom == null || settings.UnplugUntil == null)
            {
                CurrentStatus = StatusType.NotRunning;

                this.Dispatcher.Invoke(() =>
                {
                    SettingsEllipse.Fill = Brushes.Green;
                    SettingsMessage.Text = "you can add new settings";

                    AppStatusEllipse.Fill = Brushes.Green;
                    AppStatusMessage.Text = "unplug is not running";
                });
            }
            else
            {
                DateTime until = DateTime.Parse(settings.UnplugUntil);

                //Display Settings in UI
                this.Dispatcher.Invoke(() =>
                {
                    RunUntil.SelectedDate = settings.RunUntil;

                    ToTime.Value = until;

                    FromTime.Value = DateTime.Parse(settings.UnplugFrom);

                    if (settings.Unkillable)
                    {
                        ComboCriticalYes.IsChecked = true;
                        ComboCriticalNo.IsChecked = false;
                    }
                    else
                    {
                        ComboCriticalNo.IsChecked = true;
                        ComboCriticalYes.IsChecked = false;
                    }
                });

                var statusResponse = await BusinessLayer.AppStatusService.CheckStatus(settings);

                if (statusResponse.Interval.HasValue == false)
                {
                    CurrentStatus = StatusType.NotRunning;

                    this.Dispatcher.Invoke(() =>
                    {
                        SettingsEllipse.Fill = Brushes.Green;
                        SettingsMessage.Text = "you can modify your settings";

                        AppStatusEllipse.Fill = Brushes.Green;
                        AppStatusMessage.Text = "unplug is not running";
                    });
                }
                else
                {

                    DateTime endDateTime = new DateTime(settings.RunUntil.Year, settings.RunUntil.Month, settings.RunUntil.Day, until.Hour, until.Minute, 0);

                    CurrentStatus = StatusType.Running;

                    this.Dispatcher.Invoke(() =>
                    {
                        if(statusResponse.Status == BusinessLayer.AppStatus.Unknown)
                        {
                            SettingsEllipse.Fill = Brushes.Yellow;
                            SettingsMessage.Text = "cannot retrieve current date and time, please reconnect and wait a few seconds";
                        }
                        else if(statusResponse.Status == BusinessLayer.AppStatus.Offline)
                        {
                            SettingsEllipse.Fill = Brushes.Red;
                            SettingsMessage.Text = "you are offline!";
                        }
                        else
                        {
                            SettingsEllipse.Fill = Brushes.Green;
                            SettingsMessage.Text = "you are online!";
                        }

                        AppStatusEllipse.Fill = Brushes.Red;
                        AppStatusMessage.Text = $"you can't modify your settings until {endDateTime:G}";
                    });
                }
            }
        }

        private void ButtonSaveSettings_Click(Object sender, RoutedEventArgs e)
        {
            if (InputSettingsAreValid() == false)
                return;

            DateTime[] parseCorrectDateTimes = BusinessLayer.AppStatusService.ParseCorrectDateTime(DateTime.Now, FromTime.Value.Value.ToString("t"), ToTime.Value.Value.ToString("t"));
            
            TimeSpan totBlockTime = TimeSpan.FromTicks(parseCorrectDateTimes[0].Ticks - parseCorrectDateTimes[1].Ticks);

            DateTime runUntil = RunUntil.SelectedDate.Value;

            DateTime endDateTime = new DateTime(runUntil.Year, runUntil.Month, runUntil.Day, ToTime.Value.Value.Hour, ToTime.Value.Value.Minute, 0);

            String message = $"All connections will be blocked from {FromTime.Value.Value:t} to {ToTime.Value.Value:t}\n(for a total of {Math.Abs(totBlockTime.Hours)} hours and {Math.Abs(totBlockTime.Minutes)} minutes)\nuntil {endDateTime:G}\nDo you confirm?";

            var mbResult = MessageBox.Show(message, "Confirm Settings", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (mbResult != MessageBoxResult.Yes)
                return;

            WCFClient.SaveSettings(new SettingsDto()
            {
                RunUntil = endDateTime,
                TimeZoneID = TimeZoneInfo.Local.Id,
                UnplugFrom = FromTime.Value.Value.ToString("t"),
                UnplugUntil = ToTime.Value.Value.ToString("t"),
                Unkillable = ComboCriticalYes.IsChecked.Value
            });

            InitializeAppStatus();

            WCFClient.SettingsChanged();
        }

        private void ViewUninstall_Selected(Object sender, RoutedEventArgs e)
        {
            if (CurrentStatus != StatusType.NotRunning)
                return;

            //TODO Uninstall 

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = $"/im Unplug.Service.exe /f /t",
                    CreateNoWindow = true,
                    Verb = "runas",
                    UseShellExecute = true
                }).WaitForExit();

            }
            catch
            {
                return;
            }

            try
            {

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "msiexec.exe",
                    Arguments = "/X{498F286A-0A6B-47FE-9DB6-FF69FA3982FC}",
                    CreateNoWindow = true,
                    Verb = "runas",
                    UseShellExecute = false
                });
            }
            catch 
            {

            }


            Environment.Exit(0);
        }

        private void ViewInfo_Selected(Object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/gianlucacini/Unplug");
        }

        private Boolean InputSettingsAreValid()
        {
            if (FromTime.Value.HasValue == false)
            {
                FromTime.Focus();
                MessageBox.Show("please select a start time", "Invalid Settings", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (ToTime.Value.HasValue == false)
            {
                ToTime.Focus();
                MessageBox.Show("please select a end time", "Invalid Settings", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (FromTime.Value.Value == ToTime.Value.Value)
            {
                MessageBox.Show("selected timespans are not valid", "Invalid Settings", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (RunUntil.SelectedDate.HasValue == false)
            {
                RunUntil.Focus();
                MessageBox.Show("please select an end date", "Invalid Settings", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (RunUntil.SelectedDate.Value.Date < DateTime.Now.Date)
            {
                RunUntil.Focus();
                MessageBox.Show("end date must be greater than today", "Invalid Settings", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            return true;
        }
    }
}
