using JohnBauerPictureViewer.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace JohnBauerPictureViewer.UserControls
{
    public sealed partial class EditMenu : UserControl
    {
        private Grid root;

        public EditMenu(Grid root)
        {
            this.InitializeComponent();
            this.root = root;
            root.Children.Add(this);
        }

        private async void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Looking for file...");

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".csv");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                ShowMessage("Selected file: " + file.Name);

                string text = await Windows.Storage.FileIO.ReadTextAsync(file);

                ShowMessage("Got text: " + text.Substring(0, 30) + "...");

                var RawData = CSVTextParser.ParseText(text);

                var Data = ListReader.ReadLists(RawData);

                App.ApplicationData = Data;

                if (App.ApplicationData != null)
                {
                    ShowMessage("Data found in file:");
                    ShowMessage(App.ApplicationData.Categories.Count.ToString() + " Categories");
                    ShowMessage(App.ApplicationData.ImageFiles.Count.ToString() + " ImageFiles");
                    ShowMessage("");
                    for (int i = 0; i < 10; i++)
                    {
                        if (i < App.ApplicationData.ImageFiles.Count)
                        {
                            ShowMessage("File " + i.ToString() + ":");
                            ShowMessage("   FileName: " + App.ApplicationData.ImageFiles[i].FileName);
                            //ShowMessage("   Path: " + App.ApplicationData.ImageFiles[i].Path);
                            ShowMessage("   No of marks: " + App.ApplicationData.ImageFiles[i].Marks.Count.ToString());

                            string Marks = "";
                            for (int j = 0; j < App.ApplicationData.ImageFiles[i].Marks.Count; j++)
                            {
                                if (App.ApplicationData.ImageFiles[i].Marks[j] == 'x')
                                {
                                    Marks += App.ApplicationData.Categories[j].Name + ", ";
                                }
                            }                            
                            ShowMessage("   Marks: " + Marks);
                            ShowMessage("");
                        }
                    }
                }
            }
            else
            {
                ShowMessage("No file selected!");
            }

        }

        private void ShowMessage(string Message)
        {
            OutputText.Text += Message+"\n";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            root.Children.Remove(this);

        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            //Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile saveToFile = await storageFolder.CreateFileAsync("JohnBauerData.json",
                                                        Windows.Storage.CreationCollisionOption.ReplaceExisting);
            var Data = JsonConvert.SerializeObject(App.ApplicationData);
            await Windows.Storage.FileIO.WriteTextAsync(saveToFile, Data);

        }
    }
}
