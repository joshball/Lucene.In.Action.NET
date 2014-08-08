using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneHelpers;
using Xunit;

namespace ChapterTests._03
{
    public class BasicSearchTests
    {
        protected TestEnvironment TestEnvironment { get; set; }
        public BasicSearchTests()
        {
            TestEnvironment = new TestEnvironment();
        }

        [Fact]
        public void TestTermSearch()
        {
            //            var dir = TestUtil.getBookIndexDirectory(); //A
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
                
//                foreach (var result in topDocsSubjectJUnit.ScoreDocs)
//                {
//                    var explanation = indexSearcher.Explain(termQuerySubjectJUnit, result.Doc);
//                    Console.WriteLine("Explanation of Doc: {0}\n {1}", result.Doc, explanation.ToString());
//                    var document = indexSearcher.Doc(result.Doc, new MapFieldSelector("title"));
//                    var title = document.Get("title");
//                    var subject = document.Get("subject");
//                    Console.WriteLine("Document: {0} title: {1} || subject: {2}", result.Doc, title, subject);
//                }

                // title=JUnit in Action, Second Edition
                // subject=junit unit testing mock objects

                // title=Ant in Action
                // subject=apache ant build tool junit java development
                Assert.Equal(2, topDocsSubjectJUnit.TotalHits); // Ants in Action, "JUnit in Action, Second Edition"
            }

        }



    }
}
