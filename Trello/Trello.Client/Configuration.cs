namespace Trello.Client
{
    public class Configuration
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Url { get; set; }
        public required IEnumerable<string> Names { get; set; }
        public required int Timeout { get; set; }
        public required bool Parallel { get; set; }
    }
}
