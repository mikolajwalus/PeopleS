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
            var posts = _context.Posts.Where( x => x.UserId == postParams.SenderId).OrderByDescending( x => x.DateOfCreation);

            return await PagedList<Post>.CreateAsync(posts, postParams.PageNumber);
        }

        public async Task<bool> SaveAll()
        {
            if( await _context.SaveChangesAsync() > 0)return true;

            return false;
        }
    }
}