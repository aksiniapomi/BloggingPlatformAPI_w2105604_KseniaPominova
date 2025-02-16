//The table Users will store Users with different roles (Admin, RegisteredUser, Reader) 
// 	A user can write multiple blog posts
//  A user can write multiple comments
//  A user can like multiple blog posts

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GothamPostBlogAPI.Models; 

namespace GothamPostBlogAPI.Models
{
public enum UserRole //will be stored an enumerated type (0,1,2)
{
    //define the roles available 
    Admin,
    RegisteredUser,
    Reader
}

public class User
{
    [Key]
    public int UserId { get; set; }  // Primary Key, uniquely identifes the user 
    
    //User attributes 
    [Required, MaxLength(100)]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }  // Store hashed passwords

    [Required]
    public UserRole Role { get; set; }  // Enum for role-based access

    // Navigation Properties (Relationships)
    //A User can create multiple blog posts 
    public List<BlogPost> BlogPosts { get; set; } = new();
    //A User can write multiple comments 
    public List<Comment> Comments { get; set; } = new();
    //A User can like multiple blog posts 
    public List<Like> Likes { get; set; } = new();
}

}