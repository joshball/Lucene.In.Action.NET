using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Xunit;
using LuceneVersion = Lucene.Net.Util.Version;

namespace ChapterTests._03
{
    public class BasicSearchTests : BasicSearchTestsBase
    {

        [Fact]
        public void TestTermSearch()
        {
            using (var dir = FSDirectory.Open(TestEnvironment.TestIndexDirectory))
            using (var indexSearcher = new IndexSearcher(dir))
            {
                var termSubjectAnt = new Term("subject", "ant");
                var termQuerySubjectAnt = new TermQuery(termSubjectAnt);
                var topDocsSubjectAnt = indexSearcher.Search(termQuerySubjectAnt, 10);

                // title=Ant in Action
                // subject=apache ant build tool junit java development
                Assert.Equal(1, topDocsSubjectAnt.TotalHits);


                var termSubjectJUnit = new Term("subject", "junit");
                var termQuerySubjectJUnit = new TermQuery(termSubjectJUnit);
                var topDocsSubjectJUnit = indexSearcher.Search(termQuerySubjectJUnit, 10);

//                ExplainResults(indexSearcher, termQuerySubjectJUnit, topDocsSubjectJUnit);

                // title=JUnit in Action, Second Edition
                // subject=junit unit testing mock objects

                // title=Ant in Action
                // subject=apache ant build tool junit java development
                Assert.Equal(2, topDocsSubjectJUnit.TotalHits); // Ants in Action, "JUnit in Action, Second Edition"
            }

        }

        /// <summary>
        /// Search for a specific term (smallest indexed piece). In this case, ISBN.
        /// Terms are case sensitive.
        /// Terms are Good for getting docs by key.
        /// if indexed with NOT_ANALYZED, then same value can be used.
        /// NOT_ANALYZED != unique
        /// 
        /// </summary>
        [Fact]
        public void TestKeywordSearch()
        {
            using (var dir = FSDirectory.Open(TestEnvironment.TestIndexDirectory))
            using (var indexSearcher = new IndexSearcher(dir))
            {
                var term = new Term("isbn", "9781935182023");
                var termQuery = new TermQuery(term);
                var topDocs = indexSearcher.Search(termQuery, 10);
                Assert.Equal(1, topDocs.TotalHits);
            }
            
        }

        [Fact]
        public void TestQueryParser()
        {
            using (var dir = FSDirectory.Open(TestEnvironment.TestIndexDirectory))
            using (var indexSearcher = new IndexSearcher(dir))
            {
                var queryParser = new QueryParser(LuceneVersion.LUCENE_30, "contents", new SimpleAnalyzer());
                var query = queryParser.Parse("+JUNIT +ANT -MOCK");

                var topDocs = indexSearcher.Search(query, 10);
                Assert.Equal(1, topDocs.TotalHits);

                var document = indexSearcher.Doc(topDocs.ScoreDocs[0].Doc);
                Assert.Equal("Ant in Action", document.Get("title"));

                var queryTwo = queryParser.Parse("mock OR junit");
                var topDocsTwo = indexSearcher.Search(queryTwo, 10);
                // "Ant in Action, JUnit in Action, Second Edition"
                Assert.Equal(2, topDocsTwo.TotalHits);
            }
            
        }

        [Fact]
        public void TestTermRangeQuery()
        {
            using (var dir = FSDirectory.Open(TestEnvironment.TestIndexDirectory))
            using (var indexSearcher = new IndexSearcher(dir))
            {
                var termRangeQuery = new TermRangeQuery(field:"title2", lowerTerm: "d", upperTerm:"j",includeLower:true, includeUpper:true);
                var topDocs = indexSearcher.Search(termRangeQuery, 100);
                Assert.Equal(3, topDocs.TotalHits);
            }
            
        }
    }
}
