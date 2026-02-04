namespace com.github.fredjk_gh.PluginCommon.Data.Spansh
{
    public class SpanshException(string msg, Exception innerEx = null) : Exception(msg, innerEx)
    { }
}
