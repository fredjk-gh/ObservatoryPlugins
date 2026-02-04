namespace com.github.fredjk_gh.ObservatoryStatScanner
{
    internal class RecordsCSVFormatChangedException(string message) : Exception(message)
    { }

    internal class RecordsCSVParseException(string message, Exception? innerException) : Exception(message, innerException)
    { }
}
