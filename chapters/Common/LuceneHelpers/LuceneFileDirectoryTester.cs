using Lucene.Net.Analysis;
using Lucene.Net.Store;
using LuceneVersion = Lucene.Net.Util.Version;

namespace LuceneHelpers
{
    public class LuceneFileDirectoryTester : LuceneBase
    {
        public LuceneFileDirectoryTester(string indexDir, Analyzer analyzer, LuceneVersion version)
            : base(FSDirectory.Open(indexDir), analyzer, version)
        {
        }
    }
}