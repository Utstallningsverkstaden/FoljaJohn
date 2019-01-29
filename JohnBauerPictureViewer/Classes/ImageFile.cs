using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnBauerPictureViewer.Classes
{
    public class ImageFile
    {
        public string FileName { get; set; }
        //public string Path { get; set; }
        public List<char> Marks { get; set; }
        //public List<char> MainCategoryMarks { get; set; }
        public bool Rotation { get; set; }
    }
}
