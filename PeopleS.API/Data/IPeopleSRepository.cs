using System.Collections.Generic;
using System.Threading.Tasks;
using PeopleS.API.Models;

namespace PeopleS.API.Data
{
    public interface IPeopleSRepository
    {
        public void Add<T>(T entity) where T: class;
        public void Delete<T>(T entity) where T: class;
        public Task<int> SaveAll();
        public Task<Value> GetValue(int id);
        public Task<IEnumerable<Value>> GetValues();
    }
}