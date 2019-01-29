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
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace JohnBauerPictureViewer.UserControls
{
    public sealed partial class ImageGridViewer : UserControl
    {
        public ImageGridViewer()
        {
            this.InitializeComponent();
        }

        internal void ShowImageFile(ImageFile ImageFile)
        {
            bool LocationFound = false;
            if (BigPicture.HasNoImage)
            {
                BigPicture.ShowImageFile(ImageFile, 1060, 1060);
                LocationFound = true;
            }

            foreach (var item in FirstImageGridRow.Children)
            {
                if (!LocationFound)
                {
                    if (item is ImageViewer)
                    {
                        if ((item as ImageViewer).HasNoImage)
                        {
                            (item as ImageViewer).ShowImageFile(ImageFile, 410, 525);
                            LocationFound = true;
                        }
                    }
                }
            }
            foreach (var item in SecondImageGridRow.Children)
            {
                if (!LocationFound)
                {
                    if (item is ImageViewer)
                    {
                        if ((item as ImageViewer).HasNoImage)
                        {
                            (item as ImageViewer).ShowImageFile(ImageFile, 410, 525);
                            LocationFound = true;
                        }
                    }
                }
            }

        }

        internal void ClearImageFiles()
        {
            BigPicture.ClearImageFile();

            foreach (var item in FirstImageGridRow.Children)
            {
                if (item is ImageViewer)
                {
                    (item as ImageViewer).ClearImageFile();
                }                
            }
            foreach (var item in SecondImageGridRow.Children)
            {
                if (item is ImageViewer)
                {
                    (item as ImageViewer).ClearImageFile();
                }
            }

        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //ImageGridPanel.Margin = new Thickness(e.NewSize.Height / 20);

            //FirstImageGridRow.Height = e.NewSize.Height / 2.1;
            
            //SecondImageGridRow.Height = FirstImageGridRow.Height;

            //foreach (var item in FirstImageGridRow.Children)
            //{
            //    (item as ImageViewer).Height = FirstImageGridRow.Height;
            //    (item as ImageViewer).Width = FirstImageGridRow.Height;

            //    (item as ImageViewer).Margin = new Thickness(e.NewSize.Height / 20);
            //}
            //foreach (var item in SecondImageGridRow.Children)
            //{
            //    (item as ImageViewer).Height = FirstImageGridRow.Height;
            //    (item as ImageViewer).Width = FirstImageGridRow.Height;
            //    (item as ImageViewer).Margin = new Thickness(e.NewSize.Height / 20);
            //}
            
        }

        internal void FadeAway()
        {
            //DoubleAnimation DA = new DoubleAnimation();
            //DA.From = 1;
            //DA.To = 0;
            //DA.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            //DA.FillBehavior = FillBehavior.HoldEnd;
            
            //Storyboard.SetTarget(DA, ImageGridPanel);
            //Storyboard.SetTargetProperty(DA, "Opacity");            

            Storyboard SB = new Storyboard();
            SB.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            //SB.Children.Add(DA);
            SB.Completed += DA_Completed;
            SB.Begin();

        }
        

        private void DA_Completed(object sender, object e)
        {
            ClearImageFiles();
            ImageGridPanel.Opacity = 1;
        }

        internal void ShowFrames(bool viewDebug)
        {
            if (viewDebug)
            {
                Frames.Visibility = Visibility.Visible;
            }
            else
            {
                Frames.Visibility = Visibility.Collapsed;
            }


            BigPicture.ShowDebugInfo(viewDebug);
            foreach (var item in FirstImageGridRow.Children)
            {
                if (item is ImageViewer)
                {
                    (item as ImageViewer).ShowDebugInfo(viewDebug);
                }
            }
            foreach (var item in SecondImageGridRow.Children)
            {
                if (item is ImageViewer)
                {
                    (item as ImageViewer).ShowDebugInfo(viewDebug);
                }
            }
        }
    }
}
