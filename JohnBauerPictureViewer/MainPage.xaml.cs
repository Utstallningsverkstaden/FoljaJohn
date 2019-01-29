using JohnBauerPictureViewer.Classes;
using JohnBauerPictureViewer.UserControls;
using Newtonsoft.Json;
using Spree.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace JohnBauerPictureViewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            App.Settings.FullScreen();

            LoadData();

            CheckSettingsFile();

        }

        private async void CheckSettingsFile()
        {
            await App.Settings.SetupSettingsFile("JohnBauer.json");

            App.WatchoutIP = App.Settings.GetValue("WatchoutIP");
            App.WatchoutPort = App.Settings.GetValue("WatchoutPort");
            App.Show1 = App.Settings.GetValue("Show1");
            App.Show2 = App.Settings.GetValue("Show2");
            App.Que1 = App.Settings.GetValue("Que1");
            App.Que2 = App.Settings.GetValue("Que2");
            App.Que3 = App.Settings.GetValue("Que3");
            App.PauseMilliseconds = Convert.ToInt32(App.Settings.GetValue("PauseMilliseconds"));
            
            new ProductionMode(Root);
            
        }

        private async void LoadData()
        {
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile dataToFile = await storageFolder.GetFileAsync("JohnBauerData.json");
            if (dataToFile != null)
            {
                var Data = await FileIO.ReadTextAsync(dataToFile);
                App.ApplicationData = JsonConvert.DeserializeObject<ImageFileData>(Data);
            }
        }

        private void EditContentButton_Click(object sender, RoutedEventArgs e)
        {
            new EditMenu(Root);
        }

        private void ViewSearchButton_Click(object sender, RoutedEventArgs e)
        {
            new SearchPanel(Root);
        }

        private void WatchoutDashboardButton_Click(object sender, RoutedEventArgs e)
        {
            new WatchOutDashboard(Root);

        }

        private void ProductionModeButton_Click(object sender, RoutedEventArgs e)
        {
            new ProductionMode(Root);

        }
    }
}
