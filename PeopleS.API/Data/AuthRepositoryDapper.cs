using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using PeopleS.API.Models;

namespace PeopleS.API.Data
{
    public class AuthRepositoryDapper : IAuthRepository
    {
        private IConfiguration _configuration;
        public AuthRepositoryDapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> ChangePassword(int id, string oldPassword, string newPassword)
        {
            using (IDbConnection cnn = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM USERS WHERE Id = @Id";
                
                var result = await cnn.QueryAsync<User>(query, new {@Id = id});

                var user = result.FirstOrDefault();

                if(user == null) return false;

                if( Login(user.Email, oldPassword) == null) return false;  
                
                byte[] passwordHash;
                byte[] passwordSalt;

                CreatePasswordHash(newPassword, out passwordHash, out passwordSalt); 

                string insert = @"UPDATE Users SET 
                                PasswordHash = @PasswordHash
                                PasswordSalt = @PasswordSalt
                                WHERE Id = @Id";     

                var p = new DynamicParameters();
                p.Add("@Id", user.Id);
                p.Add("@PasswordHash", passwordHash);
                p.Add("@PasswordSalt", passwordSalt);

                await cnn.ExecuteAsync(insert, p);

                return true;      
            }
        }

        public async Task<User> Login(string email, string password)
        {
            using(IDbConnection cnn =  new SqliteConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var p = new DynamicParameters();
                p.Add("@Email", email.ToLower()); 

                string sql = "SELECT * FROM Users WHERE Email = @Email";

                var result = await cnn.QueryAsync<User>(sql, p);

                var user = result.FirstOrDefault();

                if (VerifyPassword( password, user.PasswordHash, user.PasswordSalt)) return user;

                return null;
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

            using(IDbConnection cnn = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var p = new DynamicParameters();
                p.Add("@Email", user.Email);
                p.Add("@PasswordHash", user.PasswordHash);
                p.Add("@PasswordSalt", user.PasswordSalt);

                string sql = @"INSERT INTO Users (Email, PasswordHash, PasswordSalt)
                                VALUES (@Email, @PasswordHash, @PasswordSalt);
                                SELECT Id FROM Users Where Email = @Email;";

                var result = await cnn.QueryAsync<int>(sql, p);

                user.Id = result.FirstOrDefault();
            }
            return user;
        }

        public async Task<bool> UserExists(string email)
        {
            using (IDbConnection cnn = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string sql = "SELECT * FROM Users WHERE Email = @Email";

                var p = new DynamicParameters();
                p.Add("@Email", email.ToLower());

                var result = await cnn.QueryAsync<User>(sql, p);

                if(result.FirstOrDefault() == null) return false;

                return true;
            }
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                hmac.Key = passwordSalt;

                return hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).SequenceEqual(passwordHash);
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}