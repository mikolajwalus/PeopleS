using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleS.API.Data;
using PeopleS.API.Dtos;
using PeopleS.API.Helpers;

namespace PeopleS.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IPeopleSRepository _repo;
        private readonly IMapper _mapper;
        private readonly IAuthRepository _auth;
        public UsersController(IPeopleSRepository repo, IMapper mapper, IAuthRepository auth)
        {
            _auth = auth;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            // TODO Add if statement with check for other user with "friends" relationship check
            var userId = int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != id) return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            var userForReturn = _mapper.Map<UserDetailedDto>(userFromRepo);

            return Ok(userForReturn);
        }

        [HttpPut("{id}/updateInfo")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserMainInfoDto user)
        {
            var userId = int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != id) return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(user, userFromRepo);

            if (await _repo.SaveAll()) return NoContent();

            return BadRequest();

        }

        [HttpPut("{id}/changeEmail")]
        public async Task<IActionResult> ChangeEmail(int id, [FromBody] UserEmailDto email)
        {
            var userId = int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != id) return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            userFromRepo.Email = email.Email;

            if (await _repo.SaveAll()) return NoContent();

            return BadRequest();
        }

        [HttpPut("{id}/changePassword")]
        public async Task<IActionResult> ChangeEmail(int id, [FromBody]PasswordChangeDto passwordObject)
        {
            var userId = int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != id) return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            if( ! await _auth.ChangePassword(userFromRepo.Id, passwordObject.OldPassword, passwordObject.Password) ) 
                return Unauthorized();

            return NoContent();
        }

        [HttpPut("{id}/changeBirthDate")]
        public async Task<IActionResult> ChangeBirthDate(int id, [FromBody]UserDateDto dateObject)
        {
            var userId = int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != id) return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            userFromRepo.DateOfBirth = dateObject.DateOfBirth;

            if (await _repo.SaveAll()) return NoContent();

            return BadRequest();
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetPosts([FromQuery]PostParams postParams)
        {
            var userId = int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != postParams.SenderId) return Unauthorized();

            var posts = await _repo.GetUserPosts(postParams);

            var response = _mapper.Map<PostDto[]>(posts);

            Response.AddPagination(posts.CurrentPage, posts.PageSize, posts.TotalCount, posts.TotalPages);

            return Ok(response);
        }
    }
}