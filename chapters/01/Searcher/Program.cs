using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = System.IO.Directory;
using Version = Lucene.Net.Util.Version;

namespace Searcher
{
    public class Searcher : IDisposable
    {
        protected Lucene.Net.Store.Directory IndexDirectory { get; set; }
        protected IndexSearcher IndexSearcher { get; set; }
        protected Analyzer Analyzer { get; set; }
        public Version CurrentVersion { get; set; }

        public Searcher(string indexDir)
        {
            Console.WriteLine("Search.indexDir: {0}", indexDir);
            IndexDirectory = FSDirectory.Open(indexDir);
            CurrentVersion = Version.LUCENE_30;
            Analyzer = new StandardAnalyzer(CurrentVersion);
            IndexSearcher = new IndexSearcher(IndexDirectory);
        }


        public void Dispose()
        {
            IndexSearcher.Dispose();
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
        public TopDocs Search(string queryString)
        {
            Console.WriteLine("Search.queryString: {0}", queryString);
            var parser = new QueryParser(CurrentVersion, "contents", Analyzer);
            var query = parser.Parse(queryString);

            var hits = IndexSearcher.Search(query, 10);
            Console.WriteLine("Search.IndexSearcher.MaxDoc: {0}", IndexSearcher.MaxDoc);
            //Console.WriteLine("Search.IndexSearcher.fullpath: {0}", IndexSearcher.Doc(1).GetValues("fullpath").First());
            Console.WriteLine("Search.hits.MaxScore: {0}", hits.MaxScore);
            Console.WriteLine("Search.hits.TotalHits: {0}", hits.TotalHits);
            Console.WriteLine("Search.hits.ScoreDocs: {0}", hits.ScoreDocs.Length);
            return hits;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Searcher <indexDir> query");
                return;
            }

            var indexDir = args[0];
            var query = args[1];

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var searcher = new Searcher(indexDir);
                searcher.CheckIndex();
                var hits = searcher.Search(query);
                stopwatch.Stop();
                Console.WriteLine("Searcher found {0} documents (in {1} ms) that matched the query.", hits.TotalHits, stopwatch.Elapsed);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
