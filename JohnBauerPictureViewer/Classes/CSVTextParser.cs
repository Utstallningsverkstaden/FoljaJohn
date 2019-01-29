using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnBauerPictureViewer.Classes
{
    public class CSVTextParser
    {
        internal static List<List<string>> ParseText(string text)
        {
            List<List<string>> Result = new List<List<string>>();
            
            if (!string.IsNullOrWhiteSpace(text))
            {
                while (!string.IsNullOrWhiteSpace(text))
                {
                    string Row = GetOneRow(ref text);

                    List<string> RowData = ParseRow(Row);

                    Result.Add(RowData);
                }
            }
            return Result;
        }

        private static List<string> ParseRow(string row)
        {
            List<string> Result = new List<string>();

            string[] fields = row.Split(',');

            foreach (var item in fields)
            {
                Result.Add(item);
            }
            return Result;
        }

        private static string GetOneRow(ref string text)
        {
            string Result = "";
            if (!string.IsNullOrWhiteSpace(text))
            {
                bool FoundRowEnd = false;
                while ((!string.IsNullOrWhiteSpace(text)) && (!FoundRowEnd))
                {
                    char NextChar = text[0];

                    if (NextChar == '\n')
                    {
                        FoundRowEnd = true;
                    }
                    else
                    {
                        Result += NextChar;
                    }
                    text = text.Substring(1);                    
                }
            }
            return Result;
        }
    }
}
