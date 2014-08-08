using System;
using System.Collections.Generic;
using SystemDirectory = System.IO.Directory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneHelpers
{
    public class TestEnvironment
    {
        public static string DefaultLuceneTestIndexDirectoryPath = @"c:\tmp\lucene_test_index";


        public string TestIndexDirectory { get; set; }

        public TestEnvironment()
            : this(DefaultLuceneTestIndexDirectoryPath)
        {
        }

        public TestEnvironment(string testIndexDirectoryPath)
        {
            TestIndexDirectory = testIndexDirectoryPath;
            EnsureDirectoryExists(TestIndexDirectory);
        }

        public static void EnsureDirectoryExists(string directory)
        {
            if (!SystemDirectory.Exists(directory))
            {
                SystemDirectory.CreateDirectory(directory);
            }
        }

    }
}
