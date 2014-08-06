using System;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using LuceneVersion = Lucene.Net.Util.Version;

namespace LuceneHelpers
{
    public class LuceneMemoryTester : LuceneBase
    {
        public LuceneMemoryTester(Analyzer analyzer, LuceneVersion version)
            : base(new RAMDirectory(), analyzer, version)
        {
        }
    }
}