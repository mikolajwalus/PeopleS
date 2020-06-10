using System.Threading.Tasks;
using PeopleS.API.Models;

namespace PeopleS.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(string name, string password);
        Task<User> Login(string name, string password);
        Task<bool> UserExists(string name);
    }
}