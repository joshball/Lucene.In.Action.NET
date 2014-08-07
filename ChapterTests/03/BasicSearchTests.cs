using Xunit;

namespace ChapterTests._03
{
    public class BasicSearchTests
    {

        [Fact]
        public void TestTerm()
        {

//            var dir = TestUtil.getBookIndexDirectory(); //A
//            IndexSearcher searcher = new IndexSearcher(dir);  //B
//
//            Term t = new Term("subject", "ant");
//            Query query = new TermQuery(t);
//            TopDocs docs = searcher.search(query, 10);
//            assertEquals("Ant in Action",                //C
//                         1, docs.totalHits);                         //C
//
//            t = new Term("subject", "junit");
//            docs = searcher.search(new TermQuery(t), 10);
//            assertEquals("Ant in Action, " +                                 //D
//                         "JUnit in Action, Second Edition",                  //D
//                         2, docs.totalHits);                                 //D
//
//            searcher.close();
//            dir.close();
        }
    }
}
