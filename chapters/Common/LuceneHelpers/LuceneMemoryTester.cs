using System;
using Lucene.Net.Store;

namespace LuceneHelpers
{
    public class LuceneMemoryTester : LuceneBase
    {
        public LuceneMemoryTester()
            : base(new RAMDirectory())
        {
        }
    }
}