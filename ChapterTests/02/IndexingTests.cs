using Xunit;
using Lucene.Net;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace ChapterTests._02
{
    public class IndexingTests
    {
        [Fact]
        public void IndexWriterHasCorrectNumberOfDocs()
        {
            var tester = new LuceneCitiesFixtureData();
            using (var indexWriter = tester.GetIndexWriter(false))
            {
                Assert.Equal(tester.CityData.Count, indexWriter.NumDocs());
                Assert.Equal(tester.CityData.Count, indexWriter.MaxDoc());
            }
        }

        [Fact]
        public void IndexReaderHasTheCorrectNumberOfDocs()
        {
            var tester = new LuceneCitiesFixtureData();
            using (var indexReader = tester.GetIndexReader())
            {
                Assert.Equal(tester.CityData.Count, indexReader.NumDocs());
                Assert.Equal(tester.CityData.Count, indexReader.MaxDoc);
            }
        }


        [Fact]
        public void DeletingDocumentsWithoutOptimizationShowsExpectedCounts()
        {
            var tester = new LuceneCitiesFixtureData();
//            var tester = new LuceneMemoryTester(CurrentAnalyzer, CurrentVersion);
//            Assert.Equal(2, tester.Index(AddCityData, true));
            using (var indexWriter = tester.GetIndexWriter(false))
            {
                Assert.Equal(tester.CityData.Count, indexWriter.NumDocs());
                Assert.Equal(tester.CityData.Count, indexWriter.MaxDoc());
                indexWriter.DeleteDocuments(new Term("id", "1"));
                indexWriter.Commit();
                Assert.Equal(2, indexWriter.MaxDoc());
                Assert.Equal(1, indexWriter.NumDocs());
            }
        }

        [Fact]
        public void DeletingDocumentsWithOptimizationShowsExpectedCounts()
        {
            var tester = new LuceneCitiesFixtureData();
            using (var indexWriter = tester.GetIndexWriter(false))
            {
                Assert.Equal(tester.CityData.Count, indexWriter.NumDocs());
                Assert.Equal(tester.CityData.Count, indexWriter.MaxDoc());
                indexWriter.DeleteDocuments(new Term("id", "1"));
                indexWriter.Commit();
                indexWriter.Optimize();
                Assert.Equal(1, indexWriter.MaxDoc());
                Assert.Equal(1, indexWriter.NumDocs());
            }
        }

        [Fact]
        public void TestIndexUpdate()
        {
            var tester = new LuceneCitiesFixtureData();
            Assert.Equal(1, tester.SearchFieldTerm("city", "Amsterdam").TotalHits);

            using (var indexWriter = tester.GetIndexWriter(false))
            {
                var doc = CreateDenHaagCity();
                indexWriter.UpdateDocument(new Term("id", "1"), doc);
            }
            Assert.Equal(0, tester.SearchFieldTerm("city", "Amsterdam").TotalHits);
            Assert.Equal(1, tester.SearchFieldTerm("city", "Haag").TotalHits);
        }

        [Fact]
        public void TestMaxFieldLength()
        {
            var tester = new LuceneCitiesFixtureData();
            Assert.Equal(1, tester.SearchFieldTerm("contents", "bridges").TotalHits);

            // Create a writer with an unlimted max field length
            using (var indexWriter = tester.GetIndexWriter(false))
            {
                var doc = new Document();
                doc.Add(new Field("contents", "these bridges CAN be found (because field length is unlimited)", Field.Store.NO, Field.Index.ANALYZED));
                indexWriter.AddDocument(doc);
            }
            // Seach should now return 2 items
            Assert.Equal(2, tester.SearchFieldTerm("contents", "bridges").TotalHits);

            // Create a writer with a field length of 1 
            using (var indexWriter = tester.GetIndexWriter(false, new IndexWriter.MaxFieldLength(1)))
            {
                var doc = new Document();
                // (and a string with bridge term greater than 1)
                doc.Add(new Field("contents", "these bridges CANNOT be found (because field length is 1)", Field.Store.NO, Field.Index.ANALYZED));
                indexWriter.AddDocument(doc);
            }
            // Seach should still only return 2 items
            Assert.Equal(2, tester.SearchFieldTerm("contents", "bridges").TotalHits);
        }

        private static Document CreateDenHaagCity()
        {
            var doc = new Document();
            doc.Add(new Field("id", "1", Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("country", "Netherlands", Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("contents", "Den Haag has a lot of museums", Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field("city", "Den Haag", Field.Store.YES, Field.Index.ANALYZED));
            return doc;
        }
    }
}