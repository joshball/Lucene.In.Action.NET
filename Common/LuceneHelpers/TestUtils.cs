using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using SystemDirectory = System.IO.Directory;

namespace LuceneHelpers
{
    public class TestUtils
    {
        public static Dictionary<string, string> ConvertLinesToProperties(string[] lines)
        {
            return lines.ToDictionary(row => row.Split('=')[0], row => string.Join("=", row.Split('=').Skip(1).ToArray()));
        }

        public static Dictionary<string, string> ReadPropertiesFile(string filename)
        {
            // foreach (var row in File.ReadAllLines(filename))
            //      properties.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
            return ConvertLinesToProperties(File.ReadAllLines(filename));
        }

        // borrowed from: http://stackoverflow.com/a/1615860/39758
        static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m =>
                {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }

        public static bool HitsIncludeTitle(IndexSearcher searcher, TopDocs topDocs, String title)
        {
            var found = topDocs
                .ScoreDocs
                .Select(scoreDoc => searcher.Doc(scoreDoc.Doc))
                .Any(doc => title.Equals(doc.Get("title")));
            if(found)
            {
                return true;
            }
            Console.WriteLine("title '" + title + "' not found");
            return false;
        }
    }
}
