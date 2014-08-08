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
    public class PrefixQueryTest : BasicSearchTestsBase
    {
        [Fact]
        public void TestPrefix()
        {
            using (var dir = FSDirectory.Open(TestEnvironment.TestIndexDirectory))
            using (var indexSearcher = new IndexSearcher(dir))
            {

                var term = new Term("category","/technology/computers/programming");
                // Search, including subcategories
                var prefixQuery = new PrefixQuery(term);

                var topDocs = indexSearcher.Search(prefixQuery, 10);
                var programmingAndBelow = topDocs.TotalHits;

                // Search, without subcategories
                topDocs = indexSearcher.Search(new TermQuery(term), 10);
                var justProgramming = topDocs.TotalHits;

                Assert.True(programmingAndBelow > justProgramming);
            }

        }
    }
}
