namespace com.github.fredjk_gh.PluginCommon.Data.Journals
{
    public class JournalDateGenerator(DateTime? seed = null)
    {
        private readonly DateTime _seed = seed ?? DateTime.Now;
        private int _counter = 0;

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
