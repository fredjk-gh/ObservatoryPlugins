namespace com.github.fredjk_gh.PluginCommon.Exceptions
{
    public class DBNotConnectedException(string name, Exception inner = null)
        : Exception($"DB '{name}' is not connected", inner)
    { }

    public class DBCorruptedException(string name, Exception inner = null)
        : Exception($"DB '{name}' exists but could not be initialized and may be corrupted.", inner)
    { }
}
