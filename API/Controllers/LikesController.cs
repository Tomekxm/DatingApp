using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string userName)
        {
            var _sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUsernameAsync(userName);
            var sourceUser = await _likesRepository.GetUserWithLikes(_sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.userName == userName) return BadRequest("You cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(_sourceUserId, likedUser.id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                sourceUserId = _sourceUserId,
                likedUserId = likedUser.id,
            };

            sourceUser.likedUsers.Add(userLike);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes ([FromQuery]LikesParams likesParams)
        {
            likesParams.userId = User.GetUserId();
            var users = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.currentPage, users.pageSize, users.totalCount, users.totalCount);

            return Ok(users);
        }

    }

}