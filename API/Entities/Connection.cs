namespace API.Entities
{
    public class Connection
    {
        public Connection(string connectionId, string userName)
        {
            this.connectionId = connectionId;
            this.userName = userName;
        }

        public string connectionId { get; set; }
        public string userName { get; set; }
    }
}