namespace API.Entities
{
    public class UserLike
    {
        public AppUser sourceUser { get; set; }
        public int sourceUserId { get; set; }
        public AppUser likedUser { get; set; }
        public int likedUserId { get; set; }
    }
}