using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleS.API.Data;
using PeopleS.API.Dtos;
using PeopleS.API.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;

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
        public async Task<IActionResult> ChangePassword(int id, [FromBody]PasswordChangeDto passwordObject)
        {
            var userId = int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != id) return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            if( ! await _auth.ChangePassword(userFromRepo.Id, passwordObject.OldPassword, passwordObject.Password) ) 
                return Unauthorized();

            return NoContent();
        }

        [HttpPut("{id}/changeBirthDate+")]
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
        public async Task<IActionResult> GetUserProfile([FromQuery]PostParams postParams)
        {
            var userFromRepo = await _repo.GetUser(postParams.SenderId);

            if (userFromRepo == null) return BadRequest("User don't exist");

            var userToReturn = _mapper.Map<UserDetailedDto>(userFromRepo);

            var senderId = int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var status = await _repo.GetFriendshipStatus(userFromRepo.Id, senderId);
            var statusReversed = await _repo.GetFriendshipStatus(senderId, userFromRepo.Id);

            var jsonFormatter = new JsonSerializerSettings();
            jsonFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.Formatting = Formatting.Indented;

            if( (status == 1 || status == 4) || (statusReversed == 1 || statusReversed == 4))
            {
                var postsFromRepo = await _repo.GetUserPosts(postParams);

                var mappedPosts = _mapper.Map<PostDto[]>(postsFromRepo);

                Response.AddPagination( postsFromRepo.CurrentPage, 
                                        postsFromRepo.PageSize, 
                                        postsFromRepo.TotalCount, 
                                        postsFromRepo.TotalPages);

                var objectToReturn = new {
                    posts = mappedPosts,
                    user = userToReturn,
                };

                var response = JsonConvert.SerializeObject(objectToReturn, jsonFormatter);

                return Ok(response);
            }
            else
            {
                var objectToReturn = new {
                    posts = new {},
                    user = userToReturn,
                };

                var response = JsonConvert.SerializeObject(objectToReturn, jsonFormatter);

                return Ok(response);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUser([FromQuery] UserParams userParams) 
        {
            var userId = int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var usersFromRepo = await _repo.SearchUser(userParams);

            var usersForReturn = usersFromRepo.ChangeToSearchDto(userId);

            if( usersForReturn.Where( x => x.Id == userId).FirstOrDefault() != null )
                usersForReturn.Where( x => x.Id == userId).FirstOrDefault().FriendshipStatus = 4;

            Response.AddPagination( usersFromRepo.CurrentPage,
                                    usersFromRepo.PageSize,
                                    usersFromRepo.TotalCount,
                                    usersFromRepo.TotalPages);

            return Ok(usersForReturn);
        }

        [HttpPost("{id}/addFriend/{recieverId}")]
        public async Task<IActionResult> AddFriend( int id, int recieverId )
        {
            var userId = int.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId != id) return Unauthorized();

            var userFromRepo = await _repo.GetUser(recieverId);

            if (userFromRepo == null) return BadRequest("Reciever don't exist");

            var status = await _repo.GetFriendshipStatus(recieverId, id);

            if (status == 0) return BadRequest("Invitation already sent");

            if(status == 1) return BadRequest("User is already a friend");

            if(status == 2) return BadRequest("User is blocked!");

            if (status == 4) return BadRequest("User cannot be friend with himself!");

            var updatedStatus = await _repo.CreateFriendship(recieverId, id);

            var objectToReturn = new {
                    status = updatedStatus
                };


            return Ok(objectToReturn);
        }
    }
}