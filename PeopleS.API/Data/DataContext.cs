using Microsoft.EntityFrameworkCore;
using PeopleS.API.Models;

namespace PeopleS.API.Data
{
    public class DataContext :DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<Post> Posts { get; set; }
    }
}