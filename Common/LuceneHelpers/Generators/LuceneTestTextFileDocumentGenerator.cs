using System.IO;
using Lucene.Net.Documents;

namespace LuceneHelpers.Generators
{
    public class LuceneTestTextFileDocumentGenerator : LuceneTestFileDocumentGeneratorBase
    {
        public LuceneTestTextFileDocumentGenerator(string fileDirectoryPath) : base(fileDirectoryPath, "*.txt")
        {
        }

        protected override Document CreateDocument(string fileName, string dataRootDirectory)
        {
            var doc = new Document();
            doc.Add(new Field("contents", new StreamReader(fileName)));
            doc.Add(new Field("filename", Path.GetFileName(fileName), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("fullpath", fileName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }
    }
}