namespace API.Helpers
{
    public class MessageParams : PaginationParams
    {
        public string username { get; set; }
        public string container { get; set; } = "Unread";
    }
}