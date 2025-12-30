namespace com.github.fredjk_gh.ObservatoryArchivist
{
    class DBNotConnectedException : Exception
    {
        public DBNotConnectedException(string name) : base($"Archivist DB '{name}' is not connected") { }
    }
}
