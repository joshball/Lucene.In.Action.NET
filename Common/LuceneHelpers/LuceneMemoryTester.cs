using System;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using LuceneVersion = Lucene.Net.Util.Version;
using LuceneAnalyzer = Lucene.Net.Analysis.Analyzer;

namespace LuceneHelpers
{
    public class LuceneMemoryTester : LuceneBase
    {
        public LuceneMemoryTester(LuceneAnalyzer analyzer, LuceneVersion version)
            : base(new RAMDirectory(), analyzer, version)
        {
        }
    }
}