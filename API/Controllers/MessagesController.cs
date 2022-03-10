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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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

            var _sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var _recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.recipientUsername);

            if (_recipient == null) return NotFound();

            var message = new Message
            {
                sender = _sender,
                recipient = _recipient,
                senderUsername = _sender.UserName,
                recipientUsername = _recipient.UserName,
                content = createMessageDto.content
            };

            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.username = User.GetUsername();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.currentPage, messages.pageSize, messages.totalCount, messages.totalPages);

            return messages;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            if (message.senderUsername != username && message.recipientUsername != username) return Unauthorized();

            if (message.sender.UserName == username) message.senderDeleted = true;

            if (message.recipient.UserName == username) message.recipientDeleted = true;

            if (message.senderDeleted && message.recipientDeleted) _unitOfWork.MessageRepository.DeleteMessage(message);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}