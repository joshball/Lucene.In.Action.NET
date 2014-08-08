using System;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;

namespace LuceneHelpers.Analyzers
{
    public class LargeGapStandardAnalyzer : StandardAnalyzer
    {
        public LargeGapStandardAnalyzer(Version matchVersion) : base(matchVersion)
        {
        }

        public override int GetPositionIncrementGap(String field)
        {
            return field.Equals("contents") ? 100 : 0;
        }
    }
}