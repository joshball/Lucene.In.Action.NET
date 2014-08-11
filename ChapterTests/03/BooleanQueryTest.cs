using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneHelpers;
using Xunit;

namespace ChapterTests._03
{
    public class BooleanQueryTest : BasicSearchTestsBase
    {
        [Fact]
        public void TestBooleanAnd()
        {
            using (var dir = FSDirectory.Open(TestEnvironment.TestIndexDirectory))
            using (var indexSearcher = new IndexSearcher(dir))
            {
                var searchingBooks = new TermQuery(new Term("subject", "search"));

                // Search, including subcategories
                var books2010 = NumericRangeQuery.NewIntRange("pubmonth", 201001, 201012, true, true);

                var searchingBooks2010 = new BooleanQuery
                {
                    {searchingBooks, Occur.MUST}, 
                    {books2010, Occur.MUST}
                };

                // Search, without subcategories
                var topDocs = indexSearcher.Search(searchingBooks2010, 10);

                Assert.True(TestUtils.HitsIncludeTitle(indexSearcher, topDocs, "Lucene in Action, Second Edition"));
            }

        }
    }
}
