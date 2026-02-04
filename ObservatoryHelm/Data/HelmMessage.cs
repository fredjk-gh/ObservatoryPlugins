namespace com.github.fredjk_gh.ObservatoryHelm.Data
{
    internal class HelmMessage(DateTime ts, string msg, string sender)
    {
        public DateTime Timestamp { get; init; } = ts;
        public string Message { get; init; } = msg;
        public string Sender { get; init; } = sender;

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss K}\t{Sender}\t{Message}";
        }
    }
}
