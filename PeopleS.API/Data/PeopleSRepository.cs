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
    }
}