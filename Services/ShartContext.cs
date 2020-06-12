namespace Roshart.Services
{
    sealed class ShartContext
    {
        public string Host { get; }
        public ShartCollection Sharts { get; }

        public ShartContext(string host, ShartCollection sharts)
        {
            Host = host;
            Sharts = sharts;
        }
    }
}
