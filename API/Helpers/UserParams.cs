namespace API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int pageNumber { get; set; } = 1;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string currentUserName { get; set; }
        public string gender { get; set; }
        public int minAge { get; set; } = 18;
        public int maxAge { get; set; } = 150;
        public string orderBy { get; set; } = "lastActive";
    }
}