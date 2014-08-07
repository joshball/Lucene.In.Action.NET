using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using LuceneHelpers;
using Xunit;
using Version = Lucene.Net.Util.Version;

namespace ChapterTests._02
{
    public class LuceneCitiesFixtureData : LuceneMemoryTester, IDisposable
    {
        public List<CityData> CityData = new List<CityData>
        {
            new CityData("1", "Amsterdam", "Netherlands", "Amsterdam has lots of bridges"),
            new CityData("2", "Venice", "Italy", "Venice has lots of canals")
        };

        public static Version CurrentVersion = Version.LUCENE_30;
        public static Analyzer CurrentAnalyzer = new WhitespaceAnalyzer();

        public LuceneCitiesFixtureData()
            : base(CurrentAnalyzer, CurrentVersion)
        {
            Assert.Equal(2, Index(AddCityData, true));
        }

        public int AddCityData(IndexWriter indexWriter)
        {
            foreach (CityData data in CityData)
            {
                var doc = new Document();
                doc.Add(new Field("id", data.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("city", data.City, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("country", data.Country, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("contents", data.Notes, Field.Store.NO, Field.Index.ANALYZED));
                indexWriter.AddDocument(doc);
            }
            indexWriter.Commit();
            return CityData.Count;
        }

        public void Dispose()
        {
            this.IndexDirectory.Dispose();
        }
    }
}