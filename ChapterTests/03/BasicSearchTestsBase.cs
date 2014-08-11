using System;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
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

        protected void IndexSingleFieldDocs(Field[] fields)
        {
            using (var dir = new RAMDirectory())
            using (var indexWriter = new IndexWriter(dir, new WhitespaceAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var field in fields)
                {
                    var doc = new Document();
                    doc.Add(field);
                    indexWriter.AddDocument(doc);
                }
                indexWriter.Optimize();
            }
        }
    }
}