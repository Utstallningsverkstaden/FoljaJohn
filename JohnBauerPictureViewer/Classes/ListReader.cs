using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnBauerPictureViewer.Classes
{
    public class ListReader
    {
        internal static ImageFileData ReadLists(List<List<string>> rawData)
        {
            ImageFileData Result = new ImageFileData();

            Result.Categories = GetCategories(rawData);

            //Result.MainCategories = GetMainCategories(Result.Categories);

            Result.ImageFiles = GetImageFiles(rawData, Result.Categories);//, Result.MainCategories);
            
            return Result;
        }

        //private static List<Category> GetMainCategories(List<Category> categories)
        //{
        //    List<Category> Result = new List<Category>();

        //    string MainCategoryName = "";
        //    int MainCategoryColumn = 0;

        //    foreach (var category in categories)
        //    {
        //        if (MainCategoryName != category.MainCategoryName)
        //        {
        //            var MainCategory = new Category();

        //            MainCategory.MainCategoryName = category.MainCategoryName;
        //            MainCategory.MainCategoryColumn = category.MainCategoryColumn;
        //            MainCategory.Name = "-";
        //            MainCategory.ListColumn = 0;

        //            MainCategoryName = category.MainCategoryName;

        //            Result.Add(MainCategory);
        //        }
        //    }

        //    return Result;
        //}

        private static List<ImageFile> GetImageFiles(List<List<string>> rawData, List<Category> categories)//, List<Category> MainCategories)
        {
            List<ImageFile> Result = new List<ImageFile>();

            if (rawData.Count > 1)
            {
                for (int i = 1; i < rawData.Count; i++)
                {
                    var Row = rawData[i];

                    ImageFile imagefile = new ImageFile();

                    imagefile.FileName = Row[0];
                    imagefile.Rotation = Row[1].Trim().ToLower() == "x";
                    imagefile.Marks = GetMarks(Row, categories);
                    Result.Add(imagefile);
                }
            }
            return Result;
        }

        //private static List<char> GetMainCategoryMarks(List<string> Row, List<Category> mainCategories)
        //{
        //    List<char> Result = new List<char>();

        //    for (int i = 0; i < mainCategories.Count; i++)
        //    {
        //        Category category = mainCategories[i];

        //        if (category.MainCategoryColumn < Row.Count)
        //        {
        //            Result.Add(GetMarkChar(Row[category.MainCategoryColumn]));
        //        }
        //    }
        //    return Result;
        //}

        private static List<char> GetMarks(List<string> Row, List<Category> categories)
        {
            List<char> Result = new List<char>();

            for (int i = 0; i < categories.Count; i++)
            {
                Category category = categories[i];
                
                if (category.ListColumn < Row.Count)
                {
                    Result.Add(GetMarkChar(Row[category.ListColumn]));
                }

            }
            return Result;
        }

        private static char GetMarkChar(string v)
        {
            if (v != null)
            {
                if (v.Trim().ToLower() == "x")
                { 
                        return 'x';
                }
            }
            return ' ';
        }

        private static List<Category> GetCategories(List<List<string>> rawData)
        {
            List<Category> Result = new List<Category>();

            if (rawData.Count > 2)
            {
                var Row = rawData[0];
                for (int i = 2; i < Row.Count; i++)
                {
                    Category category = new Category();
                    category.Name = Row[i].Trim();
                    category.ListColumn = i;
                    Result.Add(category);

                }
            }
            return Result;
        }
    }
}
