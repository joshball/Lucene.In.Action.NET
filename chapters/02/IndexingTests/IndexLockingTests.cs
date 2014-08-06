using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Xunit;

namespace IndexingTests
{
    public class IndexLockingTests
    {

        [Fact]
        public void TestWriteLock()
        {
            var path = Path.Combine(Path.GetTempPath(), "tmpLuceneIndex");

            var dir = FSDirectory.Open(path);
            using (var indexWriter = new IndexWriter(dir, new SimpleAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED))
            {
                try
                {
                    var indexWriterExecption = new IndexWriter(dir, new SimpleAnalyzer(),
                        IndexWriter.MaxFieldLength.UNLIMITED);
                }
                catch (LockObtainFailedException e)
                {
                    Assert.Equal(-2146232800, e.HResult);
                }
            }
        }

    }
}
