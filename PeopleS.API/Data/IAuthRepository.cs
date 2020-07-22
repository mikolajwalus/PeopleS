using System.Threading.Tasks;
using PeopleS.API.Models;

namespace PeopleS.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(string email, string password);
        Task<User> Login(string email, string password);
        Task<bool> UserExists(string email);
        Task<bool> ChangePassword(int id, string oldPassword, string newPassword);
    }
}