//The infromation about all the database tables here 
//Connects the models to the database using EF Core 

//Import all models 
using GothamPostBlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GothamPostBlogAPI.Data
{
    public class ApplicationDbContext : DbContext //inherits from DbContext, main EF Core class for managing database (middleman between the database and models)
    {
        //Constructor which allows to pass database configuration settings 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        //DbSets - Tables in the Database  
        public DbSet<User> Users { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Ensure there are no duplicate likes (one like per post only) for data integrity 
            modelBuilder.Entity<Like>()
            .HasIndex(l => new { l.UserId, l.BlogPostId })
            .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}