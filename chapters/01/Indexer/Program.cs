using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Lucene.Net;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Directory = System.IO.Directory;

namespace Indexer
{
    public class Indexer
    {
        protected Analyzer Analyzer { get; set; }
        protected Lucene.Net.Store.Directory IndexDirectory { get; set; }

        public Indexer(string indexDir)
        {
            IndexDirectory = FSDirectory.Open(indexDir);
            Analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        }


        public int Index(string dataDir)
        {
            var files = Directory.GetFiles(dataDir, "*.txt");
            using (var writer = new IndexWriter(IndexDirectory, Analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var file in files)
                {
                    var doc = CreateDocument(file);
                    writer.AddDocument(doc);
                }
            }
            return files.Length;
        }

        private Document CreateDocument(string fileName)
        {
            var doc = new Document();
            doc.Add(new Field("contents", new StreamReader(fileName)));
//            Alternatively:
//            var fileString = System.IO.File.ReadAllText(fileName);
//            doc.Add(new Field("contents", fileString, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("filename", Path.GetFileName(fileName), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("fullpath", fileName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }

        public void CheckIndex()
        {
            using (var reader = IndexReader.Open(this.IndexDirectory, true))
            {
                Console.WriteLine("\n IndexReader Docs:");
                var numDocs = reader.NumDocs();
                Console.WriteLine(" - num docs: {0}", reader.NumDocs());
                Console.WriteLine(" - max docs: {0}", reader.MaxDoc);
                Console.WriteLine(" - max num deleted docs: {0}", reader.NumDeletedDocs);
                Console.WriteLine(" - has deletions: {0}", reader.HasDeletions);
                Console.WriteLine(" - ref count: {0}", reader.RefCount);
                Console.WriteLine(" - directory: {0}", reader.IndexCommit.Directory);
                Console.WriteLine(" - SegmentsFileName: {0}", reader.IndexCommit.SegmentsFileName);
                Console.WriteLine(" - Filenames:");
                foreach (var fileName in reader.IndexCommit.FileNames)
                {
                    Console.WriteLine("     - {0}", fileName);
                }

                Console.WriteLine("\n Field Names:");
                var fieldNames = reader.GetFieldNames(IndexReader.FieldOption.ALL);
                foreach (var fieldName in fieldNames)
                {
                    Console.WriteLine(" - {0}", fieldName);
                }

                Console.WriteLine("\n Documents:");
                for (var i = 0; i < reader.NumDocs(); i++)
                {
                    Console.WriteLine("\n --------------- DOCUMENT {0} ---------------", i);
                    var doc = reader.Document(i);
                    var fields = doc.GetFields();
                    Console.WriteLine("    Total Fields: {0}", fields.Count);
                    foreach (var fieldName in fields)
                    {
                        Console.WriteLine("\n    Field Name: {0}", fieldName.Name);
                        Console.WriteLine("\t    StringValue.Length: {0}", fieldName.StringValue.Length);
                    }
                    //                    Console.WriteLine(" DOC:  {0}", doc.ToString());
                    Console.WriteLine("--------------- END DOCUMENT {0} ---------------", i);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Indexer <indexDir> <dataDir>");
                return;
            }

            var indexDir = args[0];
            var dataDir = args[1];

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var indexer = new Indexer(indexDir);
                var numFilesIndexed = indexer.Index(dataDir);
                indexer.CheckIndex();
                stopwatch.Stop();
                Console.WriteLine("Indexing {0} files took {1} ms.", numFilesIndexed, stopwatch.Elapsed);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
