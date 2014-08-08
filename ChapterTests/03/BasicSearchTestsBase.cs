using System;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using LuceneHelpers;

namespace ChapterTests._03
{
    public class BasicSearchTestsBase
    {
        protected TestEnvironment TestEnvironment { get; set; }

        public BasicSearchTestsBase()
        {
            TestEnvironment = new TestEnvironment();

        }
        private static void ExplainResults(IndexSearcher indexSearcher, TermQuery termQuery, TopDocs topDocs)
        {
            foreach (var result in topDocs.ScoreDocs)
            {
                var explanation = indexSearcher.Explain(termQuery, result.Doc);
                Console.WriteLine("Explanation of Doc: {0}\n {1}", result.Doc, explanation.ToString());
                var document = indexSearcher.Doc(result.Doc, new MapFieldSelector("title"));
                var title = document.Get("title");
                var subject = document.Get("subject");
                Console.WriteLine("Document: {0} title: {1} || subject: {2}", result.Doc, title, subject);
            }
        }
    }
}