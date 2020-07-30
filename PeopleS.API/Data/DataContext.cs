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
        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          modelBuilder.Entity<Friendship>()
              .HasKey(fs => new { fs.RequestorId, fs.RecieverId });

          modelBuilder.Entity<Friendship>()
              .HasOne(fs => fs.Requestor)
              .WithMany(fs => fs.FriendsRequested)
              .HasForeignKey(fs => fs.RequestorId)
              .OnDelete(DeleteBehavior.Restrict);

          modelBuilder.Entity<Friendship>()
              .HasOne(fs => fs.Reciever)
              .WithMany(fs => fs.FriendsRecieved)
              .HasForeignKey(fs => fs.RecieverId)
              .OnDelete(DeleteBehavior.Restrict);
      }
    }
}