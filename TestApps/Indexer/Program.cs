using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Lucene.Net;
using Lucene.Net.Analysis.Standard;
using LuceneHelpers;
using LuceneHelpers.Generators;
using LuceneVersion = Lucene.Net.Util.Version;

namespace Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Indexer <indexDir> <dataDir>");
                return;
            }

            Console.WriteLine("Index *.txt files in a directory into a Lucene index.");
            Console.WriteLine("Use the Searcher to search this index.");
            Console.WriteLine("Indexer.exe is covered in the 'Meet Lucene' chapter.");
            var indexDir = args[0];
            var dataDir = args[1];
            Console.WriteLine("Directory for new Lucene index: [{0}]", indexDir);
            Console.WriteLine("Directory with data files to index: [{0}]", dataDir);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                const LuceneVersion version = LuceneVersion.LUCENE_30;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                var tester = new LuceneFileDirectoryTester(indexDir, analyzer, version);
                var docGen = new LuceneTestTextFileDocumentGenerator(dataDir);
                var numFilesIndexed = tester.IndexDataFiles(docGen, true);
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
