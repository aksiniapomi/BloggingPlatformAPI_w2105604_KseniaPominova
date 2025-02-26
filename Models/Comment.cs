//The Comment model represents comments made on blog posts 
//A user can make multiple comments (One-to-Many relationship with User)
//A blog post can have multiple comments(One-to-Many relationship with BlogPost)
using System;
using System.ComponentModel.DataAnnotations;
using GothamPostBlogAPI.Models;

namespace GothamPostBlogAPI.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }  //Primary Key

        [Required]
        public required string CommentContent { get; set; }  //Comment text; non-nullable property 

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;  //DateTime when the comment was created

        //Foreign Keys
        public int UserId { get; set; }  //FK to User
        public User? User { get; set; } //Retrieve the comment author's details

        public int BlogPostId { get; set; }  //FK to BlogPost
        public BlogPost? BlogPost { get; set; } //Access to the blog post  the comment is related to 

        //Empty Constructor for EF Core
        public Comment() { }
        //Full constrcutor to manually create the objects 
        public Comment(string content, User user, BlogPost blogPost)
        {

            CommentContent = content; //always set 
            User = user; //cannot be null
            UserId = user.UserId;
            BlogPost = blogPost; //cannot be null 
            DateCreated = DateTime.UtcNow; //retrievers the current timestamp  
        }
    }
}