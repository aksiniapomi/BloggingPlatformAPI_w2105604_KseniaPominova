//The table Users will store Users with different roles (Admin, RegisteredUser, Reader) 
// A user can write multiple blog posts(One-to-Many with BlogPost)
// A user can write multiple comments(One-to-Many with Comment)
// A user can like multiple blog posts (One-to-Many with Like)
// A user must have a role (Admin, RegistertedUser, Reader)
// A user should have an email and hashed password 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GothamPostBlogAPI.Models;

namespace GothamPostBlogAPI.Models
{
    public enum UserRole //will be stored an enumerated type (0,1,2)
    {
        //define the roles/permissions available 
        Admin, //0 has all permissions (can create, update, delete blog posts, comments and users)
        RegisteredUser, //1 cannot delete blog posts; can create, comment and like posts 
        Reader //2 cannot create/delete blog posts/make comments or like posts; can register and read blog posts 
    }

    public class User
    {
        [Key]
        public int UserId { get; set; }  // Primary Key, uniquely identifes the user 

        //User attributes 
        [Required, MaxLength(100)]
        public required string Username { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }  // Store hashed passwords

        [Required]
        public UserRole Role { get; set; }  // Enum for role-based access

        // Navigation Properties (Relationships)
        //A User can create multiple blog posts 
        public List<BlogPost> BlogPosts { get; set; }
        //A User can write multiple comments 
        public List<Comment> Comments { get; set; }
        //A User can like multiple blog posts 
        public List<Like> Likes { get; set; }


        //EF Core requires a parameterless constructor
        public User() { }
        public User(string username, string email, string passwordHash, UserRole role)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Role = role; //ensures that when the user is created it is always assigned a role 
            BlogPosts = new(); //new empty list to store blog posts created by user to avoid null exception 
            Comments = new();
            Likes = new();
        }
    }
}

// var adminUser = new User {
//     Username = "Admin123",
//     Email = "admin@example.com",
//     PasswordHash = "hashed_password",
//     Role = UserRole.Admin
// };

