using System;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace JohnBauerPictureViewer.UserControls
{
    public sealed partial class PictureViewer : UserControl
    {
        private string path;
        private Grid root;

        public PictureViewer(Grid root, string path)
        {
            this.InitializeComponent();
            this.root = root;
            this.path = path;

            root.Children.Add(this);

            Viewer.Source = new Uri(path, UriKind.RelativeOrAbsolute);
            //BitmapImage B = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));

            //ImageControl.Source = B;
        }

        private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

        }

        private void Grid_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {
            root.Children.Remove(this);

        }

        private void ImageControl_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void ImageControl_ImageOpened(object sender, RoutedEventArgs e)
        {

        }
    }
}
