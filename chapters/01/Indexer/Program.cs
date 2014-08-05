﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Lucene.Net;

namespace Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Indexer <indexDir> <dataDir>");
                return;
            }

            Console.WriteLine("Index *.txt files in a directory into a Lucene index.");
            Console.WriteLine("Use the Searcher to search this index.");
            Console.WriteLine("Indexer.exe is covered in the 'Meet Lucene' chapter.");
            var indexDir = args[0];
            var dataDir = args[1];
            Console.WriteLine("Directory for new Lucene index: [{0}]", indexDir);
            Console.WriteLine("Directory with .txt files to index: [{0}]", indexDir);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var indexer = new Indexer(indexDir);
                var numFilesIndexed = indexer.Index(dataDir);
                stopwatch.Stop();
                Console.WriteLine("Indexing {0} files took {1} ms.", numFilesIndexed, stopwatch.Elapsed);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}