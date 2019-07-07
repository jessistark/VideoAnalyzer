using System;
using Windows.System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using System.Diagnostics;
using Windows.Storage;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VideoAnalyzer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool playing = false;


        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseFile();
        }

        private async void ChooseFile()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            VideoName.Text = file.Name;

            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            Video.SetSource(stream, file.ContentType);
            Video.PlaybackRate = 1.5;

        }

        private void DisableLogging()
        {

        }

        private void EnableLogging()
        {

        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (playing)
            {
                playing = false;
                Video.Pause();
                PlayPauseButton.Content = "Play";
                DisableLogging();
            } else
            {
                playing = true;
                Video.Play();
                PlayPauseButton.Content = "Pause";
                EnableLogging();
            }
        }

        private void Video_MediaOpened(object sender, RoutedEventArgs e)
        {
            PlayPauseButton.IsEnabled = true;
        }

        private void VideoCodes_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.C:
                    LogText.Text += Video.Position + ",Correct seam" + Environment.NewLine;
                    break;
                case VirtualKey.R:
                    LogText.Text += Video.Position + ",Redo seam" + Environment.NewLine;
                    break;
                case VirtualKey.A:
                    LogText.Text += Video.Position + ",Look at interface" + Environment.NewLine;
                    break;
                case VirtualKey.W:
                    LogText.Text += Video.Position + ",Look at work" + Environment.NewLine;
                    break;
                case VirtualKey.E:
                    LogText.Text += Video.Position + ",Look elsewhere" + Environment.NewLine;
                    break;
                case VirtualKey.G:
                    LogText.Text += Video.Position + ",New complete green" + Environment.NewLine;
                    break;
                case VirtualKey.F:
                    LogText.Text += Video.Position + ",New incomplete green" + Environment.NewLine;
                    break;
                case VirtualKey.Q:
                    LogText.Text += Video.Position + ",End" + Environment.NewLine;
                    break;
            }
            VideoCodes.Text = "";
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            CreateLog(VideoName.Text.Split('.').First<String>());
        }
        
        

        public async void CreateLog(string filename)
        {
            try
            {
                StorageFile logFile = await DownloadsFolder.CreateFileAsync(filename + ".csv", Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                Log(logFile, LogText.Text);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }

        private static async void LogAsync(StorageFile logFile, String logMessage)
        {
            String toWrite = System.DateTime.Now.TimeOfDay.ToString().Split('.')[0] + "," + logMessage + "\n";

            try
            {
                await FileIO.AppendTextAsync(logFile, toWrite);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public static void Log(StorageFile logFile, String logMessage)
        {
            Action act = delegate () { LogAsync(logFile, logMessage); };
            Task.Run(act);
        }
    }
}
