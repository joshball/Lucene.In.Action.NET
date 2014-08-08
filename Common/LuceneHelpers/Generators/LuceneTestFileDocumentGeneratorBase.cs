using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Documents;

namespace LuceneHelpers.Generators
{
    public abstract class LuceneTestFileDocumentGeneratorBase : ILuceneDocumentGenerator
    {
        public string FileDirectoryPath { get; set; }
        public List<string> TestTextFiles { get; set; }
        public bool Debug { get; set; }


        protected LuceneTestFileDocumentGeneratorBase(string fileDirectoryPath, string fileFilter)
        {
            FileDirectoryPath = fileDirectoryPath;
//            TestTextFiles = Directory.GetFiles(FileDirectoryPath, fileFilter, SearchOption.AllDirectories).Select(Path.GetFullPath).ToList();
            TestTextFiles = Directory.GetFiles(FileDirectoryPath, fileFilter, SearchOption.AllDirectories).ToList();
            Debug = false;
        }

        public IEnumerable<Document> LuceneDocuments()
        {
            if (Debug)
            {
                Console.WriteLine("Generator: Found {0} files", TestTextFiles.Count);
            }
            foreach (var file in TestTextFiles)
            {
                if (Debug)
                {
                    Console.WriteLine(" - Indexing file: {0}", file);
                }
                var luceneDocument = CreateDocument(file, FileDirectoryPath);
                yield return luceneDocument;
            }
        }

        protected abstract Document CreateDocument(string fileName, string dataRootDirectory);
    }
}