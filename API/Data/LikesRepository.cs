using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
                        
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.userName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (likesParams.predicate == "liked")
            {
                likes = likes.Where(like => like.sourceUserId == likesParams.userId);
                users = likes.Select(like => like.likedUser);
            }

            else if (likesParams.predicate == "likedBy")
            {
                likes = likes.Where(like => like.likedUserId == likesParams.userId);
                users = likes.Select(like => like.sourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                userName = user.userName,
                knownAs = user.knownAs,
                age = user.dateOfBirth.CalculateAge(),
                photoUrl = user.Photos.FirstOrDefault(p => p.isMain).url,
                city = user.city,
                id = user.id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers,likesParams.pageNumber, likesParams.PageSize);
        
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.likedUsers)
                .FirstOrDefaultAsync(x => x.id == userId);
        }
    }
}