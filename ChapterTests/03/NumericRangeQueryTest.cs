using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneHelpers;
using Xunit;

namespace ChapterTests._03
{
    public class NumericRangeQueryTest : BasicSearchTestsBase
    {
        [Fact]
        public void TestInclusive()
        {
            using (var dir = FSDirectory.Open(TestEnvironment.TestIndexDirectory))
            using (var indexSearcher = new IndexSearcher(dir))
            {
                var termRangeQuery = new TermRangeQuery(field: "title2", lowerTerm: "d", upperTerm: "j", includeLower: true, includeUpper: true);
                var topDocs = indexSearcher.Search(termRangeQuery, 100);
                Assert.Equal(3, topDocs.TotalHits);

//                foreach (var scoreDoc in topDocs.ScoreDocs)
//                {
//                    Console.WriteLine("  match: {0}:{1}", indexSearcher.Doc(scoreDoc.Doc).Get("author"));
//                }
            }

        }

        [Fact]
        public void TestExclusive()
        {
            using (var dir = FSDirectory.Open(TestEnvironment.TestIndexDirectory))
            using (var indexSearcher = new IndexSearcher(dir))
            {
                // pub date of TTC was September 2006
                var numericRangeQuery = NumericRangeQuery.NewIntRange("pubmonth", 200605, 200609, false, false);

                var topDocs = indexSearcher.Search(numericRangeQuery, 100);
                Assert.Equal(0, topDocs.TotalHits);
            }

        }

    }
}
