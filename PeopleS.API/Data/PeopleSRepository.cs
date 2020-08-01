using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            var users = _context.Users.Where( x => 
                x.Name.ToLower().Contains(trimmedString) ||
                x.Surname.ToLower().Contains(trimmedString) ||
                string.Concat(x.Name.ToLower(), x.Surname.ToLower()).Contains(trimmedString)
            ).Include(x => x.FriendsRecieved)
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
            return await _context.Friendships.Where( x => 
                (x.Reciever.Id == id || x.Requestor.Id == id) && x.Status == 1
            ).Include(x => x.Requestor)
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

                return 0;
            }

            if( friendshipFromRepo.RecieverId == recieverId && friendshipFromRepo.RequestorId == requestorId ) return 0;

            var friendship = _context.Friendships
                .Where(x => x.RecieverId == requestorId && x.RequestorId == recieverId)
                .FirstOrDefault();

            friendship.Status = 1;

            await SaveAll();

            return 1;
        }
    }
}