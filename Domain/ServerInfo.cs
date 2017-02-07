namespace Domain
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public Region Region { get; set; }
        public int PlayersCount { get; set; }
        public int PlayersMax { get; set; }
        public int MinutesToNextLevelChange { get; set; }
    }
}