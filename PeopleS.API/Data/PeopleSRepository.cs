using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PeopleS.API.Dtos;
using PeopleS.API.Helpers;
using PeopleS.API.Models;

namespace PeopleS.API.Data
{
    public class PeopleSRepository : IPeopleSRepository
    {
        private readonly DataContext _context;
        public PeopleSRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            return await _context.Users.FirstOrDefaultAsync( x => x.Id == id);
        }

        public async Task<Value> GetValue(int id)
        {
            return await _context.Values.FirstOrDefaultAsync( x => x.Id == id);
        }

        public async Task<IEnumerable<Value>> GetValues()
        {
            return await _context.Values.ToListAsync();
        }

        public async Task<PagedList<Post>> GetUserPosts( PostParams postParams )
        {
            var posts = _context.Posts
                .Where( x => x.UserId == postParams.SenderId)
                .OrderByDescending( x => x.DateOfCreation)
                .Include(x => x.User);
            return await PagedList<Post>.CreateAsync(posts, postParams.PageNumber);
        }

        public async Task<PagedList<User>> SearchUser(UserParams userParams)
        {
            string trimmedString = userParams.SearchedString.Replace(" ","").ToLower();
            var users = _context.Users
                .Where( x => x.Name.ToLower().Contains(trimmedString) ||
                    x.Surname.ToLower().Contains(trimmedString) ||
                    string.Concat(x.Name.ToLower(), x.Surname.ToLower()).Contains(trimmedString))
                .Include(x => x.FriendsRecieved)
                .Include(x => x.FriendsRequested);
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber);
        }

        public async Task<bool> SaveAll()
        {
            if( await _context.SaveChangesAsync() > 0)return true;

            return false;
        }

        public async Task<IEnumerable<Friendship>> GetUserFriendships(int id)
        {
            return await _context.Friendships
                .Where( x => (x.Reciever.Id == id || x.Requestor.Id == id) && x.Status == 1)
                .Include(x => x.Requestor)
                .Include(x => x.Reciever)
                .ToListAsync();
        }

        /// <summary>
        /// Returns  the value representation of friendship status from requestor view
        /// </summary>
        /// <returns>
        /// Returns:
        /// 0 - invited
        /// 1 - accepted
        /// 2 - blocked
        /// 3 - none
        /// 4 - yourself (not exist in database)
        /// 5 - waiting for acceptation
        /// </returns>
        public async Task<int> GetFriendshipStatus(int recieverId, int requestorId)
        {
            if(recieverId == requestorId) return 4;

            var status = await _context.Friendships
                .Where(x => ( x.RecieverId == recieverId && x.RequestorId == requestorId ) ||
                            ( x.RecieverId == requestorId && x.RequestorId == recieverId ) )
                .FirstOrDefaultAsync();

            if( status == null) return 3;

            if( status.RecieverId == requestorId && status.RequestorId == recieverId && status.Status == 0) return 5;

            return status.Status;
        }
        public async Task<int> CreateFriendship(int recieverId, int requestorId)
        {
            var friendshipFromRepo = await _context.Friendships
                .Where( x => (x.RecieverId == recieverId && x.RequestorId == requestorId)
                        || (x.RecieverId == requestorId && x.RequestorId == recieverId))
                .FirstOrDefaultAsync();

            if(friendshipFromRepo == null)
            {
                var recieverFromRepo = _context.Users.Where(x => x.Id == recieverId).FirstOrDefault();
                var requestorFromRepo = _context.Users.Where(x => x.Id == requestorId).FirstOrDefault();

                _context.Add<Friendship>(new Friendship{
                    Status = 0,
                    Requestor = requestorFromRepo,
                    RequestorId = requestorId,
                    Reciever = recieverFromRepo,
                    RecieverId = recieverId
                });

                await SaveAll();

                return 5;
            }

            if( friendshipFromRepo.RecieverId == recieverId && friendshipFromRepo.RequestorId == requestorId ) return 5;

            var friendship = _context.Friendships
                .Where(x => x.RecieverId == requestorId && x.RequestorId == recieverId)
                .FirstOrDefault();

            friendship.Status = 1;

            await SaveAll();

            return 1;
        }

        public Task<PagedList<User>> GetUserFriends(FriendParams friendParams)
        {
            var friends = _context.Users
                .Where( x => 
                    x.FriendsRequested.Any( z => z.RecieverId == friendParams.SenderId && z.Status == 1) 
                    || x.FriendsRecieved.Any( z => z.RequestorId == friendParams.SenderId && z.Status == 1))
                .OrderBy( x => x.Surname);

            return PagedList<User>.CreateAsync(friends, friendParams.PageNumber, friendParams.PageSize);
        }

        public Task<PagedList<User>> GetInvitedUsers(FriendParams friendParams)
        {
            var invitedFriends = _context.Users
                .Where( x => x.FriendsRecieved.Any( z => z.RequestorId == friendParams.SenderId && z.Status == 0))
                .OrderBy( x => x.Surname);

            return PagedList<User>.CreateAsync(invitedFriends, friendParams.PageNumber, friendParams.PageSize);
        }

        public Task<PagedList<User>> GetUserInvitations(FriendParams friendParams)
        {
            var invitedFriends = _context.Users
                .Where( x => x.FriendsRequested.Any( z => z.RecieverId == friendParams.SenderId && z.Status == 0))
                .OrderBy( x => x.Surname);

            return PagedList<User>.CreateAsync(invitedFriends, friendParams.PageNumber, friendParams.PageSize);
        }
        
        public async Task<bool> CreateMessage(Message message)
        {
            var thread = _context.Threads
                    .Where(x => x.ThreadParticipants.Where(y => y.ParticipantId == message.SenderId).FirstOrDefault() != null && 
                        x.ThreadParticipants.Where(y => y.ParticipantId == message.RecipientId).FirstOrDefault() != null)
                    .Include(x => x.Messages)
                    .Include(x => x.ThreadParticipants)
                    .FirstOrDefault();
            if( thread == null )
            {
                var newThread = new Thread(){
                    Messages = new Collection<Message>(),
                    ThreadParticipants = new Collection<ThreadParticipant>(),
                    LastModified = DateTime.Now
                };

                newThread.Messages.Add(message);

                var participantOne = new ThreadParticipant() {
                    ParticipantId = message.SenderId,
                    Status = true
                };

                var participantTwo = new ThreadParticipant() {
                    ParticipantId = message.RecipientId,
                    Status = false
                };

                newThread.ThreadParticipants.Add(participantOne);
                newThread.ThreadParticipants.Add(participantTwo);                

                Add(newThread);

                return await SaveAll();
            }

            thread.Messages.Add(message);

            thread.LastModified = DateTime.Now;

            var recipient = thread.ThreadParticipants.Where(x => x.ParticipantId == message.RecipientId).FirstOrDefault();
            recipient.Status = false;

            var sender = thread.ThreadParticipants.Where(x => x.ParticipantId == message.SenderId).FirstOrDefault();
            sender.Status = true;

            return await SaveAll();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<PagedList<Message>> GetMessageThread(int requestorId, ThreadParams threadParams)
        {
            var messagesThread = _context.Messages.Where( x => 
                    (x.RecipientId == requestorId && x.SenderId == threadParams.SecondUserId) ||
                    (x.RecipientId == threadParams.SecondUserId && x.SenderId == requestorId))
                .Include( x => x.Sender)
                .Include( x => x.Recipient)
                .OrderByDescending(x => x.MessageSent);

            var massage = await messagesThread.ToListAsync();

            return await PagedList<Message>.CreateAsync(messagesThread, threadParams.PageNumber, threadParams.PageSize);
        }

        public async Task<bool> MarkThreadAsRead(int requestorId, int secondUserId)
        {
                var messagesThread = await _context.Messages
                    .Where( x => (x.RecipientId == requestorId && x.SenderId == secondUserId))
                    .Where(x => !x.IsRead)
                    .ToListAsync();

                if(messagesThread.Count == 0) return false;

                foreach (Message message in messagesThread)
                {
                    message.DateRead = DateTime.Now;
                    message.IsRead = true;
                }

                var requestor = await _context.ThreadParticipants
                    .Where( x => x.ThreadId == messagesThread.FirstOrDefault().ThreadId && x.ParticipantId == requestorId )
                    .FirstOrDefaultAsync();

                requestor.Status = true;

                if( await SaveAll() ) return true;

                return false;
        }


        public async Task<PagedList<ThreadDto>> GetThreadList(int requestorId, ThreadListParams listParams)
        {
            
            var threads = await _context.Threads
                .Where(x => x.ThreadParticipants.Where(y => y.ParticipantId == requestorId).FirstOrDefault() != null )
                .OrderByDescending(x => x.LastModified)
                .Include(x => x.ThreadParticipants)
                .Skip((listParams.PageNumber - 1) * listParams.PageSize )
                .Take(listParams.PageSize)
                .ToListAsync();

            var threadDtos = new List<ThreadDto>();

            var threadsNumber = await _context.Threads
                .Where(x => x.ThreadParticipants.Where(y => y.ParticipantId == requestorId).FirstOrDefault() != null )
                .OrderByDescending(x => x.LastModified)
                .CountAsync();

            if( threadsNumber == 0) return null;

            foreach (Thread thread in threads)
            {
                var user = await _context.ThreadParticipants
                    .Where(x => x.ThreadId == thread.Id)
                    .Where(x => x.ParticipantId != requestorId)
                    .Include(x => x.Participant)
                    .FirstOrDefaultAsync();

                 var message = await _context.Messages
                    .Where(x => x.ThreadId == thread.Id)
                    .OrderByDescending(x => x.MessageSent)
                    .FirstOrDefaultAsync();

                var lastMesageIsMine = true;
                if( message.SenderId != requestorId ) lastMesageIsMine = false;

                var isRead = true;
                if( !message.IsRead ) isRead = false;

                var threadDto = new ThreadDto() {
                    UserOneId = requestorId,
                    UserTwoId = user.ParticipantId,
                    UserTwoName = user.Participant.Name,
                    UserTwoSurname = user.Participant.Surname,
                    UserTwoPhotoUrl = user.Participant.PhotoUrl,
                    LastModified = thread.LastModified,
                    Content = message.Content,
                    LastMessageIsMine = lastMesageIsMine,
                    IsRead = isRead
                };

                threadDtos.Add(threadDto);
            }
            

            
             return new PagedList<ThreadDto>(threadDtos, threadsNumber, listParams.PageNumber, listParams.PageSize);
        }

        public async Task<Thread> GetThread(int id)
        {
            return await _context.Threads
                .Where(x => x.Id == id)
                .Include(x => x.Messages)
                .Include(x => x.ThreadParticipants)
                .FirstOrDefaultAsync()
            ;
        }

        public async Task<PagedList<Post>> GetUserDashboard(PostParams postParams)
        {
            var posts = _context.Posts
                .Where(x => x.User.FriendsRecieved
                    .Any( y => y.RequestorId == postParams.SenderId && y.Status == 1) || 
                x.User.FriendsRequested
                    .Any( y => y.RecieverId == postParams.SenderId && y.Status == 1))
                .OrderByDescending(x => x.DateOfCreation)
                .Include(x => x.User);

            return await PagedList<Post>.CreateAsync(posts, postParams.PageNumber, postParams.PageSize);
        }
    }
}