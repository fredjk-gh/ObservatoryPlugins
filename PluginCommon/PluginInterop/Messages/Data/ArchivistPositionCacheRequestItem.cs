namespace com.github.fredjk_gh.PluginCommon.PluginInterop.Messages.Data
{
    public class ArchivistPositionCacheRequestItem
    {
        public static ArchivistPositionCacheRequestItem New(string sysName, ulong id64)
        {
            return new()
            {
                SystemName = sysName,
                SystemId64 = id64,
            };
        }

        public string SystemName { get; set; }
        public ulong SystemId64 { get; set; }
    }
}
