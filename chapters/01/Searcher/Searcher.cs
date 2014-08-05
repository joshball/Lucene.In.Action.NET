using System;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
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
}