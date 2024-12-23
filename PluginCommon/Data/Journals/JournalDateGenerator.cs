using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.fredjk_gh.PluginCommon.Data.Journals
{
    public class JournalDateGenerator
    {
        private DateTime _seed;
        private int _counter = 0;

        public JournalDateGenerator(DateTime? seed = null)
        {
            _seed = seed ?? DateTime.Now;
        }

        public DateTime Next()
        {
            return _seed.AddSeconds(_counter++);
        }

        public string NextFormatted()
        {
            return Next().ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ssZ");
        }
    }
}
