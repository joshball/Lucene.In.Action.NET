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
    class PhraseQueryTest : BasicSearchTestsBase
    {

        [Fact]
        public void TestSlopComparisons()
        {
            using (var dir = FSDirectory.Open(TestEnvironment.TestIndexDirectory))
            using (var indexSearcher = new IndexSearcher(dir))
            {
                var phrases = new [] {"quick", "fox"};

                Assert.False(PhaseQueryHasHits(phrases, 0), "Exact phrase not found");
                Assert.True(PhaseQueryHasHits(phrases, 1), "Close enough");
            }
        }

        protected bool PhaseQueryHasHits(string[] phrases, int i)
        {
            using (var dir = FSDirectory.Open(TestEnvironment.TestIndexDirectory))
            using (var indexSearcher = new IndexSearcher(dir))
            {
                var phraseQuery = new PhraseQuery
                {
                    Slop = 0
                };
                foreach (var phrase in phrases)
                {
                    phraseQuery.Add(new Term("field", phrase));
                }
                // Search, without subcategories
                var topDocs = indexSearcher.Search(phraseQuery, 10);
                return topDocs.TotalHits > 0;
            }
        }
    }
}
