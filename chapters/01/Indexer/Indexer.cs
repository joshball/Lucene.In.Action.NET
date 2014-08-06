using System;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Directory = System.IO.Directory;

namespace Indexer
{
    public class Indexer
    {
        protected Analyzer Analyzer { get; set; }
        protected Lucene.Net.Store.Directory IndexDirectory { get; set; }

        public Indexer(string indexDir)
        {
            IndexDirectory = FSDirectory.Open(indexDir);
            Analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        }


        public int Index(string dataDir)
        {
            var files = Directory.GetFiles(dataDir, "*.txt");
            using (var writer = new IndexWriter(IndexDirectory, Analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var file in files)
                {
                    var doc = CreateDocument(file);
                    writer.AddDocument(doc);
                }
                writer.Commit();;
            }
            return files.Length;
        }

        private Document CreateDocument(string fileName)
        {
            var doc = new Document();
            doc.Add(new Field("contents", new StreamReader(fileName)));
            doc.Add(new Field("filename", Path.GetFileName(fileName), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("fullpath", fileName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }

    }
}