using Lucene.Net.Store;

namespace LuceneHelpers
{
    public class LuceneFileDirectoryTester : LuceneBase
    {
        public LuceneFileDirectoryTester(string indexDir) : base(FSDirectory.Open(indexDir))
        {
        }
    }
}