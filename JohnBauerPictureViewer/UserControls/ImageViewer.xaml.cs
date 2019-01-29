using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JohnBauerPictureViewer.Classes;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace JohnBauerPictureViewer.UserControls
{
    public sealed partial class ImageViewer : UserControl
    {

        public bool RotateLeft { get; set; }
        public bool RotateRight { get; set; }

        private bool _HasNoImage = true;

        public ImageViewer()
        {
            this.InitializeComponent();
        }

        public bool HasNoImage { get { return _HasNoImage;  }  }


        private void BitmapImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            //if (Width < 1000)
            //{
            //    ImageControl.Width = (sender as BitmapImage).PixelWidth;
            //    ImageControl.Height = (sender as BitmapImage).PixelHeight;

            //    Canvas.SetLeft(ImageControl, Width / 2 - (ImageControl.Width / 2));
            //    Canvas.SetTop(ImageControl, Height / 2 - (ImageControl.Height / 2));

            //    Rotate.CenterX = ImageControl.Width / 2;
            //    Rotate.CenterY = ImageControl.Height / 2;

            //}
            //else
            //{
            //    ImageControl.Width = (sender as BitmapImage).PixelWidth;
            //    ImageControl.Height = (sender as BitmapImage).PixelHeight;

            //}
        }

        public async void ShowImageFile(ImageFile item, double ImageWidth, double ImageHeight)
        {   

            FileNameTextBlock.Text = "";
            _HasNoImage = false;
            try
            {
                ImageControl.Source = await App.GetBitmapImage(item.FileName);//, BitmapImage_ImageOpened);
                
                FileNameTextBlock.Text = item.FileName + "\n";

                if (item.Rotation)
                {
                    if (RotateRight)
                    {
                        Rotate.Angle = 90;

                        ImageControl.Width = ImageHeight;
                        ImageControl.Height = ImageWidth;

                        Canvas.SetLeft(ImageControl, 410);
                        Canvas.SetTop(ImageControl, 0);

                        FileNameTextBlock.Text += "Rotated right\n";
                    }
                    else if (RotateLeft)
                    {
                        Rotate.Angle = -90;

                        ImageControl.Width = ImageHeight;
                        ImageControl.Height = ImageWidth;

                        Canvas.SetLeft(ImageControl, 0);
                        Canvas.SetTop(ImageControl, 525);
                        FileNameTextBlock.Text += "Rotated left\n";
                        
                    }

                }
                else
                {
                    Rotate.Angle = 0;

                    ImageControl.Width = ImageWidth;
                    ImageControl.Height = ImageHeight;

                    Canvas.SetLeft(ImageControl, 0);
                    Canvas.SetTop(ImageControl, 0);

                }

                if (item.Marks != null)
                {
                    for (int i = 0; i < item.Marks.Count; i++)
                    {

                        if (item.Marks[i] == 'x')
                        {
                            FileNameTextBlock.Text += App.ApplicationData.Categories[i].Name + ", ";
                        }
                    }
                }
            }
            catch
            {

            }
            
        }

        internal void ClearImageFile()
        {
            FileNameTextBlock.Text = "";
            _HasNoImage = true;
            ImageControl.Source = null;
            
        }

        internal void ShowDebugInfo(bool viewDebug)
        {
            if (viewDebug)
            {
                DebugGrid.Visibility = Visibility.Visible;
            }
            else
            {
                DebugGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Rotate.Angle += 10;

        }

        private void imageSource_ImageOpened(object sender, RoutedEventArgs e)
        {

        }
    }
}
