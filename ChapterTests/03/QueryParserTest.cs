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

        [Fact]
        public void TestWildcards()
        {
        }

    }
}
