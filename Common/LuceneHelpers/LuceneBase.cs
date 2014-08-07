﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using LuceneDirectory = Lucene.Net.Store.Directory;
using SystemDirectory = System.IO.Directory;
using LuceneVersion = Lucene.Net.Util.Version;
using LuceneAnalyzer = Lucene.Net.Analysis.Analyzer;

namespace LuceneHelpers
{
    public class LuceneBase
    {
        public LuceneAnalyzer Analyzer { get; set; }
        public LuceneDirectory IndexDirectory { get; set; }
        public LuceneVersion CurrentLuceneVersion { get; set; }

        public LuceneBase(LuceneDirectory directory, LuceneAnalyzer analyzer, LuceneVersion version)
        {
            Analyzer = analyzer;
            CurrentLuceneVersion = version;
            IndexDirectory = directory;
        }

        public IndexWriter GetIndexWriter(bool createNewIndex)
        {
            return new IndexWriter(IndexDirectory, Analyzer, createNewIndex, IndexWriter.MaxFieldLength.UNLIMITED);
        }
        public IndexWriter GetIndexWriter(bool createNewIndex, IndexWriter.MaxFieldLength maxFieldLength)
        {
            return new IndexWriter(IndexDirectory, Analyzer, createNewIndex, maxFieldLength);
        }
        public IndexReader GetIndexReader()
        {
            return IndexReader.Open(IndexDirectory, true);
        }
        public IndexSearcher GetIndexSearcher()
        {
            return new IndexSearcher(IndexDirectory, true);
        }

        public int Index(Func<IndexWriter, int> indexWriterAction, bool createNewIndex)
        {
            using (var writer = new IndexWriter(IndexDirectory, Analyzer, createNewIndex, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var count = indexWriterAction(writer);
                writer.Commit();
                return count;
            }
        }

        public int IndexDataFiles(List<string> filenames, bool createNewIndex)
        {
            using (var writer = new IndexWriter(IndexDirectory, Analyzer, createNewIndex, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.SetInfoStream(new StreamWriter(Console.OpenStandardOutput()));
                foreach (var file in filenames)
                {
                    Console.WriteLine("Indexing file: {0}", file);
                    var doc = CreateDocument(file);
                    writer.AddDocument(doc);
                }
                writer.Commit();;
            }
            return filenames.Count;
        }

        private Document CreateDocument(string fileName)
        {
            var doc = new Document();
            doc.Add(new Field("contents", new StreamReader(fileName)));
            doc.Add(new Field("filename", Path.GetFileName(fileName), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("fullpath", fileName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }


        public TopDocs Search(string queryString)
        {
            using (var searcher = new IndexSearcher(IndexDirectory))
            {
                var parser = new QueryParser(CurrentLuceneVersion, "contents", Analyzer);
                var query = parser.Parse(queryString);
                return searcher.Search(query, 10);
            }
        }

        public TopDocs SearchFieldTerm(string fieldName, string searchString)
        {
            using (var searcher = new IndexSearcher(IndexDirectory))
            {
                var term = new Term(fieldName, searchString);
                var query = new TermQuery(term);
                return searcher.Search(query, 10);
            }
        }


        public void CheckIndex()
        {
            using (var reader = IndexReader.Open(this.IndexDirectory, true))
            {
                Console.WriteLine("\n IndexReader Docs:");
                Console.WriteLine(" - num docs: {0}", reader.NumDocs());
                Console.WriteLine(" - max docs: {0}", reader.MaxDoc);
                Console.WriteLine(" - max num deleted docs: {0}", reader.NumDeletedDocs);
                Console.WriteLine(" - has deletions: {0}", reader.HasDeletions);
                Console.WriteLine(" - ref count: {0}", reader.RefCount);
                Console.WriteLine(" - directory: {0}", reader.IndexCommit.Directory);
                Console.WriteLine(" - SegmentsFileName: {0}", reader.IndexCommit.SegmentsFileName);
                Console.WriteLine(" - Filenames:");
                foreach (var fileName in reader.IndexCommit.FileNames)
                {
                    Console.WriteLine("     - {0}", fileName);
                }

                Console.WriteLine("\n Field Names:");
                var fieldNames = reader.GetFieldNames(IndexReader.FieldOption.ALL);
                foreach (var fieldName in fieldNames)
                {
                    Console.WriteLine(" - {0}", fieldName);
                }

                Console.WriteLine("\n Documents:");
                for (var i = 0; i < reader.NumDocs(); i++)
                {
                    Console.WriteLine("\n --------------- DOCUMENT {0} ---------------", i);
                    var doc = reader.Document(i);
                    var fields = doc.GetFields();
                    Console.WriteLine("    Total Fields: {0}", fields.Count);
                    foreach (var fieldName in fields)
                    {
                        Console.WriteLine("\n    Field Name: {0}", fieldName.Name);
                        Console.WriteLine("\t    StringValue.Length: {0}", fieldName.StringValue.Length);
                    }
                    //                    Console.WriteLine(" DOC:  {0}", doc.ToString());
                    Console.WriteLine("--------------- END DOCUMENT {0} ---------------", i);
                }
            }
        }

    }
}
