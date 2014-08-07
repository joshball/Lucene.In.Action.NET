using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using LuceneHelpers;

namespace IndexDump
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: IndexDump <indexDir>");
                return;
            }

            var indexDir = args[0];
            Console.WriteLine("Directory for new Lucene index: [{0}]", indexDir);
            try
            {
                var version = Lucene.Net.Util.Version.LUCENE_30;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                var tester = new LuceneFileDirectoryTester(indexDir, analyzer, version);
                tester.CheckIndex();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
