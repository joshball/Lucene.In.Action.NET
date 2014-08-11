using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Xunit;

namespace ChapterTests._03
{
    public class QueryParserTest : BasicSearchTestsBase
    {
        [Fact]
        public void TestWildcards()
        {
            using (var directory = new RAMDirectory())
            {
                IndexSingleFieldDocs(directory, new[] { 
                    new Field("contents", "wild", Field.Store.YES, Field.Index.ANALYZED),
                    new Field("contents", "child", Field.Store.YES, Field.Index.ANALYZED),
                    new Field("contents", "mild", Field.Store.YES, Field.Index.ANALYZED),
                    new Field("contents", "mildew", Field.Store.YES, Field.Index.ANALYZED) 
                });

                using (var indexSearcher = new IndexSearcher(directory))
                {
                    var query = new WildcardQuery(new Term("contents", "?ild*"));
                    var topDocs = indexSearcher.Search(query, 10);
                    Assert.Equal(3, topDocs.TotalHits); // "child no match"
                    Assert.Equal(topDocs.ScoreDocs[0].Score, topDocs.ScoreDocs[1].Score, 1); // "score the same"
                    Assert.Equal(topDocs.ScoreDocs[1].Score, topDocs.ScoreDocs[2].Score, 1); // "score the same"
                }
            }

        }

    }
}
