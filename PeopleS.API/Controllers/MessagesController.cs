using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using PeopleS.API.Data;
using PeopleS.API.Dtos;
using PeopleS.API.Helpers;
using PeopleS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PeopleS.API.Controllers
{
    [Authorize]
    [Route("users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IPeopleSRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IPeopleSRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageCreationDto messageForCreationDto)
        {
            var sender = await _repo.GetUser(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

            if(recipient == null)
                return BadRequest("Could't find recipient");

            var message = _mapper.Map<Message>(messageForCreationDto);

            _repo.Add(message);

            if (await _repo.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {userId, id = message.Id}, messageToReturn); 
            }

            
            throw new Exception("Creating the message failed on save");
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            var sender = await _repo.GetUser(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if( messageFromRepo == null ) return BadRequest("Message don't exist");

            var messageForReturn = _mapper.Map<MessageReturnDto>(messageFromRepo);

            return Ok(messageForReturn);
        }

        [HttpGet("thread")]
        public async Task<IActionResult> GetMessageThread(int userId, [FromQuery] ThreadParams threadParams)
        {
            var sender = await _repo.GetUser(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            if (sender.Id == threadParams.SecondUserId)
                return BadRequest("User cannot text to himself");

            var messagesFromRepo = await _repo.GetMessageThread(userId, threadParams);

            if( messagesFromRepo == null) return NoContent();

            var messagesToSend = _mapper.Map<MessageReturnDto[]>(messagesFromRepo);

            Response.AddPagination( messagesFromRepo.CurrentPage, 
                        messagesFromRepo.PageSize, 
                        messagesFromRepo.TotalCount, 
                        messagesFromRepo.TotalPages);

            return Ok(messagesToSend);
        }

        [HttpPut("threadRead/{secondUserId}")]
        public async Task<IActionResult> MarkThreadAsRead(int userId, int secondUserId)
        {
            var sender = await _repo.GetUser(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            if (sender.Id == secondUserId)
                return BadRequest("User cannot text to himself");

            if( ! await _repo.MarkThreadAsRead(userId, secondUserId) ) return BadRequest("Thread doesn't exist or is already read");

            return Ok();
        }

        [HttpPut("deleteMessage/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int userId, int messageId)
        {
            var sender = await _repo.GetUser(userId);

            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(messageId);

            if(messageFromRepo == null) 
                return BadRequest("Message don't exist");

            if(messageFromRepo.SenderId != userId && messageFromRepo.RecipientId != userId)
                return Unauthorized();

            if(userId == messageFromRepo.SenderId) 
                messageFromRepo.SenderDeleted = true;

            if(userId == messageFromRepo.RecipientId) 
                messageFromRepo.RecipientDeleted = true;

            if(messageFromRepo.RecipientDeleted && messageFromRepo.SenderDeleted) _repo.Delete(messageFromRepo);

            if( ! await _repo.SaveAll() ) return StatusCode(500);

            return Ok();
        }
    }
}