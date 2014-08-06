using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using LuceneHelpers;
using Directory = System.IO.Directory;

namespace Searcher
{
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
                var tester = new LuceneFileDirectoryTester(indexDir);
                var hits = tester.Search(query);
                stopwatch.Stop();
                Console.WriteLine("Search.hits.MaxScore: {0}", hits.MaxScore);
                Console.WriteLine("Search.hits.TotalHits: {0}", hits.TotalHits);
                Console.WriteLine("Search.hits.ScoreDocs: {0}", hits.ScoreDocs.Length);
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
