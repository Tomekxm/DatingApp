using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();
            if (createMessageDto.recipientUsername == null || createMessageDto.content == null) {
                return BadRequest("Message must have its recipient and content");
            }

            if (username == createMessageDto.recipientUsername.ToLower())
            {
                return BadRequest("You cannot send messages to yourself");
            }

            var _sender = await _userRepository.GetUserByUsernameAsync(username);
            var _recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.recipientUsername);

            if (_recipient == null) return NotFound();

            var message = new Message
            {
                sender = _sender,
                recipient = _recipient,
                senderUsername = _sender.UserName,
                recipientUsername = _recipient.UserName,
                content = createMessageDto.content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.username = User.GetUsername();

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.currentPage, messages.pageSize, messages.totalCount, messages.totalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread (string username)
        {
            var currentUserName = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUserName, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _messageRepository.GetMessage(id);

            if (message.senderUsername != username && message.recipientUsername != username) return Unauthorized();

            if (message.sender.UserName == username) message.senderDeleted = true;

            if (message.recipient.UserName == username) message.recipientDeleted = true;

            if (message.senderDeleted && message.recipientDeleted) _messageRepository.DeleteMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}