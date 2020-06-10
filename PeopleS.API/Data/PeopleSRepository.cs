using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Value> GetValue(int id)
        {
            return await _context.Values.FirstOrDefaultAsync( x => x.Id == id);
        }

        public async Task<IEnumerable<Value>> GetValues()
        {
            return await _context.Values.ToListAsync();
        }

        public async Task<int> SaveAll()
        {
            return await _context.SaveChangesAsync();
        }
    }
}