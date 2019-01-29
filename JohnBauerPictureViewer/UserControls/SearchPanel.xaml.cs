using JohnBauerPictureViewer.Classes;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace JohnBauerPictureViewer.UserControls
{
    public sealed partial class SearchPanel : UserControl
    {
        private Grid root;
        private List<Category> FilterCategories = new List<Category>();

        public SearchPanel(Grid root)
        {
            this.InitializeComponent();
            this.root = root;
            root.Children.Add(this);

            //ShowMainCategories();
        }

        //private void ShowMainCategories()
        //{
        //    CategoryPanel.Children.Clear();
        //    CategoryPanel.Orientation = Orientation.Horizontal;
        //    if ((App.ApplicationData != null) && (App.ApplicationData.MainCategories != null))
        //    {
        //        foreach (var category in App.ApplicationData.MainCategories)
        //        {                    
        //            Button button = new Button();
        //            button.Content = category.MainCategoryName;
        //            button.Margin = new Thickness(10);
        //            button.Tag = category;
        //            button.Click += Button_Click;
        //            CategoryPanel.Children.Add(button);
        //        }
        //    }
        //}

        //private void ShowSubCategories()
        //{
        //    CategoryPanel.Children.Clear();

        //    if ((App.ApplicationData != null) && (App.ApplicationData.Categories != null))
        //    {
        //        var MainCategory = "";
        //        StackPanel panel = new StackPanel();
        //        foreach (var category in App.ApplicationData.Categories)
        //        {
        //            if (MainCategory != category.MainCategoryName)
        //            {
        //                panel = new StackPanel();
        //                panel.Orientation = Orientation.Horizontal;
        //                TextBlock tb = new TextBlock();
        //                tb.Text = category.MainCategoryName;
        //                tb.Margin = new Thickness(20);
        //                panel.Children.Add(tb);
        //                MainCategory = category.MainCategoryName;
        //                CategoryPanel.Children.Add(panel);
        //            }

        //            if (panel.Children.Count > 10)
        //            {
        //                panel = new StackPanel();
        //                panel.Orientation = Orientation.Horizontal;
        //                CategoryPanel.Children.Add(panel);
        //            }

        //            Button button = new Button();
        //            button.Content = category.Name;
        //            button.Margin = new Thickness(10);
        //            button.Tag = category;
        //            button.Click += Button_Click;
        //            panel.Children.Add(button);
        //        }
        //    }
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Category category = (sender as Button).Tag as Category;

        //    AddToFilter(category);
        //}

        private void AddToFilter(Category category)
        {
            FilterCategories.Add(category);

            if (FilterCategories.Count == 1)
            {
                Filter.Text = FilterCategories[0].Name;
                
            }
            else
            {
                Filter.Text += ", " + category.Name;
            }

            FilterFiles(true);
        }

        private void FilterFiles(bool UseMainCategories)
        {
            List<List<ImageFile>> FilteredResult = new List<List<ImageFile>>();

            ResultPanel.Children.Clear();

            if (FilterCategories.Count > 0)
            {
                FilteredResult.Add(App.ApplicationData.ImageFiles);

                bool NoResult = false;

                foreach (var category in FilterCategories)
                {
                    if (!NoResult)
                    {

                        List<ImageFile> FileSource = FilteredResult[FilteredResult.Count - 1];
                        List<ImageFile> FileResult = new List<ImageFile>();

                        foreach (var imagefile in FileSource)
                        {
                            var CategoryIndex = App.ApplicationData.Categories.IndexOf(category);

                            if (CategoryIndex == -1)
                            {
                                CategoryIndex = App.ApplicationData.Categories.IndexOf(category);
                            }

                            if (imagefile.Marks[CategoryIndex] == 'x')
                            {
                                FileResult.Add(imagefile);
                            }
                        }

                        if (FileResult.Count > 0)
                        {
                            FilteredResult.Add(FileResult);
                        }
                        else
                        {
                            if (UseMainCategories)
                            {

                                foreach (var imagefile in FileSource)
                                {
                                    bool ViewThisFile = true;
                                    int CategoryIndex = App.ApplicationData.Categories.IndexOf(category);

                                    if (imagefile.Marks[CategoryIndex] != 'x')
                                    {
                                        FileResult.Add(imagefile);
                                    }
                                }
                                if (FileResult.Count > 0)
                                {
                                    FilteredResult.Add(FileResult);
                                }
                                else
                                {
                                    NoResult = true;
                                }
                            }
                            else
                            {
                                NoResult = true;
                            }
                        }
                    }
                }

                if (!NoResult)
                {
                    List<ImageFile> FileSource2 = FilteredResult[FilteredResult.Count - 1];


                    foreach (var imagefile in FileSource2)
                    {
                        Button B = new Button();
                        B.Content = imagefile.FileName;
                        B.Tag = imagefile;
                        //B.Click += B_Click;
                        ResultPanel.Children.Add(B);

                    }
                }
            }
        }

        //private int GetMainCategoryIndex(Category category, List<Category> mainCategories)
        //{
        //    for (int i = 0; i < mainCategories.Count; i++)
        //    {
        //        if (mainCategories[i].Name == category.Name)
        //        {
        //            return i;
        //        }
        //    }
        //    return 0;
        //}

        private void SearchForImages(Category category)
        {
            ResultPanel.Children.Clear();

            var CategoryIndex = App.ApplicationData.Categories.IndexOf(category);

            foreach (var imagefile in App.ApplicationData.ImageFiles)
            {

                if (imagefile.Marks[CategoryIndex] == 'x')
                {
                    Button B = new Button();
                    B.Content = imagefile.FileName;
                    B.Tag = imagefile;
                    //B.Click += B_Click;
                    ResultPanel.Children.Add(B);
                }
            }
        }

        //private void B_Click(object sender, RoutedEventArgs e)
        //{
        //    new PictureViewer(root, ((sender as Button).Tag as ImageFile).Path);
        //}

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            root.Children.Remove(this);
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterCategories.Clear();
            Filter.Text="-no filter-";
        }
    }
}
