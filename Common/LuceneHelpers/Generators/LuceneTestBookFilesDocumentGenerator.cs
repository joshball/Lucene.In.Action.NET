using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Lucene.Net.Documents;

namespace LuceneHelpers.Generators
{
    public class BookProps
    {
        public BookProps(string fileName)
        {
            FileName = fileName;
            PropertyFileLines = TestUtils.ReadPropertiesFile(fileName);
            ISBN = GetPropertyValue("isbn");
            Title = GetPropertyValue("title");
            Subject = GetPropertyValue("subject");
            Authors = GetPropertyValue("authors");
            Category = GetPropertyValue("category");
            Url = GetPropertyValue("url");
            PubMonth = GetPropertyValue("pubmonth");
        }

        public string FileName { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Title2 { get; set; }
        public string Subject { get; set; }
        public string Authors { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string PubMonth { get; set; }

        public Dictionary<string, string> PropertyFileLines { get; set; }

        public string GetPropertyValue(string property)
        {
            if (!PropertyFileLines.ContainsKey(property))
            {
                Console.WriteLine("Missing Property: {0} ({1})", property, FileName);
                return String.Empty;
            }
            return PropertyFileLines[property];
        }
    }

    public class BookPropsParsed
    {
        public BookPropsParsed(string fileName, string dataRootDirectory)
        {
            FileName = fileName;
            BookProps = new BookProps(fileName);
            if (fileName.IndexOf(dataRootDirectory, System.StringComparison.Ordinal) != 0)
            {
                throw new Exception("Bad root path");
            }
            var relativePath = fileName.Substring(dataRootDirectory.Length);
            var categories = String.Join("/", relativePath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.None));
            // category comes from relative path below the base directory
//            String category = file.getParent().substring(rootDir.length());    //1
//            category = category.replace(File.separatorChar, '/');              //1
            ISBN = BookProps.ISBN;
            Title = BookProps.Title;
            Subject = BookProps.Subject;
            Category = categories;
            Url = BookProps.Url;
            Authors = BookProps.Authors.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            ParsePubMonth();
        }

        public string FileName { get; set; }
        public BookProps BookProps { get; set; }
        public string[] Authors { get; set; }

        public string ISBN { get; set; }
        public string Title { get; set; }

        public string LowerdTitle
        {
            get { return Title.ToLower(); }
        }

        public string Subject { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public int? PubMonthInt { get; set; }
        public int? PubMonthAsDay { get; set; }

        public string[] Content
        {
            get { return new[] {BookProps.Title, BookProps.Subject, BookProps.Authors, BookProps.Category}; }
        }


        public void ParsePubMonth()
        {
            if (String.IsNullOrWhiteSpace(BookProps.PubMonth))
            {
                return;
            }
            try
            {
                PubMonthInt = Int32.Parse(BookProps.PubMonth);
                DateTime pubMonthDate = DateTime.ParseExact(BookProps.PubMonth, "yyyyMM", CultureInfo.InvariantCulture,
                    DateTimeStyles.None);
                double pubMonthAsMsSinceEpoch = pubMonthDate.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                PubMonthAsDay = (int) pubMonthAsMsSinceEpoch/(1000*3600*24);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception parsing PubMonth: {0} ({1})", BookProps.PubMonth, FileName);
                Console.WriteLine("Exception: {0}", exception);
                throw;
            }
        }
    }

    public class LuceneTestBookFilesDocumentGenerator : LuceneTestFileDocumentGeneratorBase
    {
        public LuceneTestBookFilesDocumentGenerator(string fileDirectoryPath)
            : base(fileDirectoryPath, "*.properties")
        {
        }


        protected override Document CreateDocument(string fileName, string dataRootDirectory)
        {
            Console.WriteLine("CreateDocument: {0}", fileName);

            var pbp = new BookPropsParsed(fileName, dataRootDirectory);

            var doc = new Document();

            doc.Add(new Field("isbn", pbp.ISBN, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("category", pbp.Category, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("title", pbp.Title, Field.Store.YES, Field.Index.ANALYZED,
                Field.TermVector.WITH_POSITIONS_OFFSETS));
            doc.Add(new Field("title2", pbp.LowerdTitle, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS,
                Field.TermVector.WITH_POSITIONS_OFFSETS));

            foreach (string author in pbp.Authors)
            {
                doc.Add(new Field("author", author, Field.Store.YES, Field.Index.NOT_ANALYZED,
                    Field.TermVector.WITH_POSITIONS_OFFSETS));
            }

            doc.Add(new Field("url", pbp.Url, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
            doc.Add(new Field("subject", pbp.Subject, Field.Store.YES, Field.Index.ANALYZED,
                Field.TermVector.WITH_POSITIONS_OFFSETS));
            if (pbp.PubMonthInt != null)
            {
                var pubMonthAsIntNumericField = new NumericField("pubmonth", Field.Store.YES, true);
                pubMonthAsIntNumericField.SetIntValue((int) pbp.PubMonthInt);
                doc.Add(pubMonthAsIntNumericField);
                if (pbp.PubMonthAsDay != null)
                {
                    var pubMonthAsDayNumericField = new NumericField("pubmonthAsDay", Field.Store.YES, true);
                    pubMonthAsDayNumericField.SetIntValue((int) pbp.PubMonthAsDay);
                    doc.Add(pubMonthAsDayNumericField);
                }
            }

            foreach (string text in pbp.Content)
            {
                doc.Add(new Field("contents", text, Field.Store.NO, Field.Index.ANALYZED,
                    Field.TermVector.WITH_POSITIONS_OFFSETS));
            }
            return doc;
        }
    }
}