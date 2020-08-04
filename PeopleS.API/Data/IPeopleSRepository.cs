using System.Collections.Generic;
using System.Threading.Tasks;
using PeopleS.API.Helpers;
using PeopleS.API.Models;

namespace PeopleS.API.Data
{
    public interface IPeopleSRepository
    {
        public void Add<T>(T entity) where T: class;
        public void Delete<T>(T entity) where T: class;
        public Task<bool> SaveAll();
        public Task<Value> GetValue(int id);
        public Task<IEnumerable<Value>> GetValues();
        public Task<User> GetUser(int id);
        public Task<PagedList<Post>> GetUserPosts(PostParams postParams);
        public Task<PagedList<User>> SearchUser( UserParams userParams);
        public Task<IEnumerable<Friendship>> GetUserFriendships(int id);
        public Task<int> GetFriendshipStatus(int recieverId, int requestorId);
        public Task<int> CreateFriendship(int recieverId, int requestorId);
        public Task<Message> GetMessage(int id);
        public Task<PagedList<Message>> GetMessageThread(int requestorId, ThreadParams threadParams);
        public Task<bool> MarkThreadAsRead(int requestorId, int secondUserId);
    }
}