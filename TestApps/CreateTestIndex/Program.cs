using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using LuceneHelpers;
using LuceneHelpers.Analyzers;
using LuceneHelpers.Generators;
using LuceneVersion = Lucene.Net.Util.Version;


namespace CreateTestIndex
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: CreateTestIndex <indexDir> <dataDir>");
                return;
            }

            Console.WriteLine("Index *.txt files in a directory into a Lucene index.");
            Console.WriteLine("CreateTestIndex.exe is covered in the 'Adding search to your application' chapter (3).");
            var indexDir = args[0];
            var dataDir = args[1];
            Console.WriteLine("Directory for new Lucene index: [{0}]", indexDir);
            Console.WriteLine("Directory with data files to index: [{0}]", dataDir);


            try
            {
                const LuceneVersion version = LuceneVersion.LUCENE_30;
                var analyzer = new LargeGapStandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                var tester = new LuceneFileDirectoryTester(indexDir, analyzer, version);
                var docGen = new LuceneTestBookFilesDocumentGenerator(dataDir);
                var numFilesIndexed = tester.IndexDataFiles(docGen, true);
                Console.WriteLine("Indexed {0} files", numFilesIndexed);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
