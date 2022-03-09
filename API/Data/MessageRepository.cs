using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.connectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(u => u.sender)
                .Include(u => u.recipient)
                .SingleOrDefaultAsync(x => x.id == id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.messageSent)
                .AsQueryable();

            query = messageParams.container switch
            {
                "Inbox" => query.Where(u => u.recipient.UserName == messageParams.username && u.recipientDeleted == false),
                "Outbox" => query.Where(u => u.sender.UserName == messageParams.username && u.senderDeleted == false),
                _ => query.Where(u => u.recipient.UserName ==
                 messageParams.username && u.recipientDeleted == false && u.dateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.pageNumber, messageParams.PageSize);
        }

           public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, 
            string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(u => u.sender).ThenInclude(p => p.Photos)
                .Include(u => u.recipient).ThenInclude(p => p.Photos)
                .Where(m => m.recipient.UserName == currentUsername && m.recipientDeleted == false
                        && m.sender.UserName == recipientUsername
                        || m.recipient.UserName == recipientUsername
                        && m.sender.UserName == currentUsername && m.senderDeleted == false
                )
                .OrderBy(m => m.messageSent)
                .ToListAsync();
            var unreadMessages = messages.Where(m => m.dateRead == null 
                && m.recipient.UserName == currentUsername).ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.dateRead = DateTime.Now;
                    message.dateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}