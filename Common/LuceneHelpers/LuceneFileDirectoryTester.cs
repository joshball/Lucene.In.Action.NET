using Lucene.Net.Analysis;
using Lucene.Net.Store;
using LuceneVersion = Lucene.Net.Util.Version;
using LuceneAnalyzer = Lucene.Net.Analysis.Analyzer;

namespace LuceneHelpers
{
    public class LuceneFileDirectoryTester : LuceneBase
    {
        public LuceneFileDirectoryTester(string indexDir, LuceneAnalyzer analyzer, LuceneVersion version)
            : base(FSDirectory.Open(indexDir), analyzer, version)
        {
        }
    }
}