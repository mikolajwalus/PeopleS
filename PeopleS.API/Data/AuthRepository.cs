using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PeopleS.API.Models;

namespace PeopleS.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string email, string password)
        {
            User user = await _context.Users.Where(p => p.Email == email.ToLower()).FirstOrDefaultAsync();

            if(user == null) return null;

            if (VerifyPassword( password, user.PasswordHash, user.PasswordSalt)) return user;

            return null;
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                hmac.Key = passwordSalt;

                return hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).SequenceEqual(passwordHash);
            }
        }

        public async Task<User> Register(string email, string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            User user = new User()
            {
                Email = email.ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
            };

            _context.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string email)
        {
            var user = await _context.Users.Where(p => p.Email == email).FirstOrDefaultAsync();

            if (user == null) return false;

            return true;
        }
    }
}