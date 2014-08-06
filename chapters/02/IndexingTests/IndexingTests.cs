using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using LuceneHelpers;
using Xunit;

namespace IndexingTests
{
    public class TestIndex
    {
        public readonly Analyzer Analyzer;
        public readonly Directory Directory;

        public List<CityData> CityData = new List<CityData>
        {
            new CityData("1", "Netherlands", "Amsterdam", "Amsterdam has lots of bridges"),
            new CityData("2", "Italy", "Venice", "Venice has lots of canals")
        };

        public IndexReader IndexReaderInstance
        {
            get
            {
                return IndexReader.Open(Directory, true);
            }
        }
        public IndexWriter IndexWriterInstance
        {
            get
            {
                var indexWriter = new IndexWriter(Directory, Analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

                foreach (CityData data in CityData)
                {
                    var doc = new Document();
                    doc.Add(new Field("id", data.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("country", data.Country, Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("contents", data.Notes, Field.Store.NO, Field.Index.ANALYZED));
                    doc.Add(new Field("city", data.City, Field.Store.YES, Field.Index.ANALYZED));
                    indexWriter.AddDocument(doc);
                }
                indexWriter.Commit();
                return indexWriter;
            }
        }
        
        public TestIndex()
        {
            Directory = new RAMDirectory();
            Analyzer = new WhitespaceAnalyzer();
        }

    }

    public class IndexingTests
    {
        public List<CityData> CityData = new List<CityData>
        {
            new CityData("1", "Netherlands", "Amsterdam", "Amsterdam has lots of bridges"),
            new CityData("2", "Italy", "Venice", "Venice has lots of canals")
        };

        public int AddCityData(IndexWriter indexWriter)
        {
            foreach (CityData data in CityData)
            {
                var doc = new Document();
                doc.Add(new Field("id", data.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("country", data.Country, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("contents", data.Notes, Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field("city", data.City, Field.Store.YES, Field.Index.ANALYZED));
                indexWriter.AddDocument(doc);
            }
            indexWriter.Commit();
            return CityData.Count;            
        }

        [Fact]
        public void IndexWriterHasCorrectNumberOfDocs()
        {
            var tester = new LuceneMemoryTester();
            Assert.Equal(2, tester.Index(AddCityData, true));
            using (var indexWriter = tester.GetIndexWriter(false))
            {
                Assert.Equal(CityData.Count, indexWriter.NumDocs());
                Assert.Equal(CityData.Count, indexWriter.MaxDoc());
            }
        }

        [Fact]
        public void IndexReaderHasTheCorrectNumberOfDocs()
        {
            var tester = new LuceneMemoryTester();
            Assert.Equal(2, tester.Index(AddCityData, true));
            using (var indexReader = tester.GetIndexReader())
            {
                Assert.Equal(CityData.Count, indexReader.NumDocs());
                Assert.Equal(CityData.Count, indexReader.MaxDoc);
            }
        }
        
  
        [Fact]
        public void IndexWriterCorrectlyDeletesDocumentsByTermId()
        {
            var tester = new LuceneMemoryTester();
            Assert.Equal(2, tester.Index(AddCityData, true));
            using (var indexWriter = tester.GetIndexWriter(false))
            {
                Assert.Equal(CityData.Count, indexWriter.NumDocs());
                Assert.Equal(CityData.Count, indexWriter.MaxDoc());
                indexWriter.DeleteDocuments(new Term("id", "1"));
                indexWriter.Commit();
                Assert.Equal(2, indexWriter.MaxDoc());
                Assert.Equal(1, indexWriter.NumDocs());
            }
        }
    }
}