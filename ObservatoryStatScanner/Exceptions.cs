using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservatoryStatScanner
{
    internal class RecordsCSVFormatChangedException : Exception
    {
        public RecordsCSVFormatChangedException(string message) : base(message)
        { }
    }

    internal class RecordsCSVParseException : Exception
    {
        public RecordsCSVParseException(string message, Exception? innerException) : base(message, innerException)
        { }

    }
}
