namespace API.Helpers
{
    public class UserParams : PaginationParams
    {
        public string currentUserName { get; set; }
        public string gender { get; set; }
        public int minAge { get; set; } = 18;
        public int maxAge { get; set; } = 150;
        public string orderBy { get; set; } = "lastActive";
    }
}