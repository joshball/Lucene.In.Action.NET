using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Xunit;

namespace IndexingTests
{
    public class CityData
    {
        public string Id { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Notes { get; set; }

        public CityData()
        {
        }

        public CityData(string id, string city, string country, string notes)
        {
            Id = id;
            City = city;
            Country = country;
            Notes = notes;
        }
    }

    public class IndexingTests
    {
        protected List<CityData> CityData = new List<CityData>
        {
            new CityData("1", "Netherlands", "Amsterdam", "Amsterdam has lots of bridges"),
            new CityData("1", "Italy", "Venice", "Venice has lots of canals")
        };
        private readonly Directory _directory;

        public IndexingTests()
        {
            this._directory = new RAMDirectory();
            var indexWriter = GetIndexWriter();
            foreach (var data in CityData)
            {
                var doc = new Document();
                doc.Add(new Field("id", data.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("country", data.Country, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("contents", data.Notes, Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field("city", data.City, Field.Store.YES, Field.Index.ANALYZED));
                indexWriter.AddDocument(doc);
            }
            indexWriter.Dispose();
        }

        private IndexWriter GetIndexWriter() 
        {
            return new IndexWriter(_directory, new WhitespaceAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED);
        }

        [Fact]
        public void IndexWriterHasCorrectNumberOfDocs()
        {
            var indexWriter = GetIndexWriter();
            Assert.Equal(CityData.Count, indexWriter.NumDocs());

        }
    }
}
