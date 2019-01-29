using JohnBauerPictureViewer.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace JohnBauerPictureViewer.UserControls
{
    public sealed partial class ProductionMode : UserControl
    {
        private List<string> CommandsToBeSent = new List<string>();


        private List<Category> Filter = new List<Category>();

        private Grid root;
        private List<ImageFile> ResultList = new List<ImageFile>();
        private Random rand;
        private DateTime LastKeyDown = DateTime.Now;
        private bool AddedPieceDetected = false;
        private DispatcherTimer timer;
        private WatchoutControlProtocol protocol;
        private bool ViewDebug;

        public List<ImageFile> RandomResult { get; private set; }

        public ProductionMode(Grid root)
        {
            this.InitializeComponent();        
            this.root = root;
            root.Children.Add(this);


            //protocol = new WatchoutControlProtocol(App.WatchoutIP, App.WatchoutPort, null);// "10.1.1.28", "8080", null);            
            //protocol.SendCommand("ping");

            Debug.Text = "";

            rand = new Random(DateTime.Now.Millisecond);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();

            FilterFiles();

        }

        private async void Timer_Tick(object sender, object e)
        {
            if (CommandsToBeSent.Count>0)
            {
                string commnad = CommandsToBeSent[0];
                CommandsToBeSent.RemoveAt(0);
                await SendCommand(commnad);
            }

            if (AddedPieceDetected)
            {
                TimerDebug.Text = (DateTime.Now - LastKeyDown).TotalMilliseconds.ToString();
                
                if ((DateTime.Now - LastKeyDown).TotalMilliseconds > App.PauseMilliseconds)//1500)
                {
                    AddedPieceDetected = false;

                    if (Filter.Count > 0)
                    {
                        AddCommand("run \"" + App.Show1 + "\"");// \"shortFade1\"");
                        AddCommand("gotoControlCue \"" + App.Que1 + "\"");   // \"startTwinkle\"");
                        AddCommand("run");

                        FilterFiles();
                    }
                    
                }
            }
        }

        private void AddCommand(string command)
        {
            CommandsToBeSent.Add(command);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

        }

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            LastKeyDown = DateTime.Now;

            switch (args.VirtualKey)
            {
                case VirtualKey.A:
                    AddToFilter(App.ApplicationData.Categories[0]);
                    break;
                case VirtualKey.B:
                    RemoveFromFilter(App.ApplicationData.Categories[0]);
                    break;
                case VirtualKey.C:
                    AddToFilter(App.ApplicationData.Categories[1]);
                    break;
                case VirtualKey.D:
                    RemoveFromFilter(App.ApplicationData.Categories[1]);
                    break;
                case VirtualKey.E:
                    AddToFilter(App.ApplicationData.Categories[2]);
                    break;
                case VirtualKey.F:
                    RemoveFromFilter(App.ApplicationData.Categories[2]);
                    break;
                case VirtualKey.G:
                    AddToFilter(App.ApplicationData.Categories[3]);
                    break;
                case VirtualKey.H:
                    RemoveFromFilter(App.ApplicationData.Categories[3]);
                    break;
                case VirtualKey.I:
                    AddToFilter(App.ApplicationData.Categories[4]);
                    break;
                case VirtualKey.J:
                    RemoveFromFilter(App.ApplicationData.Categories[4]);
                    break;

                case VirtualKey.Delete:                    
                    ToggleDebug();
                    break;

                case VirtualKey.Escape:
                    protocol = null;
                    root.Children.Remove(this);
                    break;

            }

            ShowFilter();
        }

        private void AddToFilter(Category category)
        {
            if (Filter.IndexOf(category) < 0)
            {
                Filter.Add(category);
                AddedPieceDetected = true;
            }
        }

        private async Task SendCommand(string Command)
        {
            try
            {
                WatchoutDebug.Text += Command + "\n";
                var protocol = new WatchoutControlProtocol(App.WatchoutIP, App.WatchoutPort, null);// "10.1.1.28", "8080", null);            
                await protocol.SendCommand("authenticate 1\r\n" + Command);

                
                

            }
            catch
            {

            }
        }

        private async void RemoveFromFilter(Category category)
        {
            if (Filter.Count > 0)
            {
                if (Filter.IndexOf(category) >= 0)
                {
                    //AddedPieceDetected = false;
                    Filter.Remove(category);

                    if (!AddedPieceDetected)
                    {
                        if (Filter.Count > 0)
                        {

                            AddCommand("authenticate 1");
                            AddCommand("run \"" + App.Show2 + "\"");// \"shortFade2\"");
                            AddCommand("gotoControlCue \"" + App.Que2 + "\"");//\"startSketches\"");
                            AddCommand("run");

                            FilterFiles();

                        }
                        else
                        {
                            AddCommand("authenticate 1");
                            AddCommand("run \"" + App.Show2 + "\"");//\"shortFade2\"");
                            AddCommand("gotoControlCue \"" + App.Que3 + "\"");// \"startPause\"");
                            AddCommand("run");

                            FilterFiles();

                        }
                    }
                }
            }
        }


        



        private void ShowFilter()
        {
            Debug.Text = "";
            foreach (var item in Filter)
            {
                if (Debug.Text.Length>0)
                {
                    Debug.Text += ", ";
                }

                Debug.Text += item.Name;
            }            
        }

        private void FilterFiles()
        {
            
            List<ImageFile> FilteredResult = new List<ImageFile>();

            ResultList.Clear();

            if (Filter.Count > 0)
            {
                foreach (var item in App.ApplicationData.ImageFiles)
                {
                    FilteredResult.Add(item);
                }

                foreach (var category in Filter)
                {                    
                    var CategoryIndex = App.ApplicationData.Categories.IndexOf(category);

                    List<ImageFile> FileResult = new List<ImageFile>();

                    foreach (var imagefile in FilteredResult)
                    {
                        if (imagefile.Marks[CategoryIndex] == 'x')
                        {
                            FileResult.Add(imagefile);
                        }
                    }

                    FilteredResult.Clear();

                    if (FileResult.Count > 0)
                    {
                        foreach (var item in FileResult)
                        {
                            FilteredResult.Add(item);
                        }
                    }
                }
            }

            RandomResult = new List<ImageFile>();

            if (FilteredResult.Count>= App.NumberOfPicturesToSearchFor)
            {
                for (int i = 0; i < App.NumberOfPicturesToSearchFor; i++)
                {
                    int Index = rand.Next(FilteredResult.Count);
                    RandomResult.Add(FilteredResult[Index]);
                    FilteredResult.RemoveAt(Index);
                }
            }
            else
            {

                while (FilteredResult.Count>0)
                {
                    int Index = rand.Next(FilteredResult.Count);
                    RandomResult.Add(FilteredResult[Index]);
                    FilteredResult.RemoveAt(Index);
                }
                while (RandomResult.Count<App.NumberOfPicturesToSearchFor)
                {
                    //int Index = rand.Next(App.ApplicationData.ImageFiles.Count);
                    //int Index2 = RandomResult.IndexOf(App.ApplicationData.ImageFiles[Index]);
                    //if (Index2 < 0)
                    //{
                    //    RandomResult.Add(App.ApplicationData.ImageFiles[Index]);
                    //}

                    var file = new ImageFile();
                    file.FileName = "spree_blue_RGB.png";
                    RandomResult.Add(file);
                }
            }

            ShowFiles();

            

                //DebugPanel.Children.Clear();
                //foreach (var imagefile in FilteredResult)
                //{
                //    Button B = new Button();
                //    B.Content = imagefile.FileName;
                //    B.Tag = imagefile;
                //    B.Background = new SolidColorBrush(Colors.White);
                //    B.Foreground = new SolidColorBrush(Colors.Black);
                //    B.BorderBrush = new SolidColorBrush(Colors.Yellow);
                //    DebugPanel.Children.Add(B);

                //}
                
            
        }

        private async void ShowFiles()
        {
            //await SendCommand("authenticate 1");
            //await SendCommand("run \"" + App.Show1 + "\"");// \"shortFade1\"");
            //await SendCommand("gotoControlCue \""+ App.Que1 + "\"");   // \"startTwinkle\"");
            //await SendCommand("run");
            
            Storyboard SB = new Storyboard();
            SB.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            SB.Completed += SB_Completed;            
            SB.Begin();
        }

        private void SB_Completed(object sender, object e)
        {
            ImageGrid.ClearImageFiles();
            foreach (var item in RandomResult)
            {
                ImageGrid.ShowImageFile(item);
            }
        }
        
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp -= CoreWindow_KeyUp;
        }

        private void ToggleDebug()
        {
            ViewDebug = !ViewDebug;
            if (ViewDebug)
            {
                DebugGrid.Visibility = Visibility.Visible;

                WatchoutDebug.Text = App.WatchoutIP + ":" + App.WatchoutPort + "\n";


            }
            else
            {
                DebugGrid.Visibility = Visibility.Collapsed;
            }
            ImageGrid.ShowFrames(ViewDebug);
        }

    }
}
