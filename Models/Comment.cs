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
        public string CommentContent { get; set; }  //Comment text

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;  //DateTime when the comment was created

        //Foreign Keys
        public int UserId { get; set; }  //FK to User
        public User User { get; set; } //Retrieve the comment author's details

        public int BlogPostId { get; set; }  //FK to BlogPost
        public BlogPost BlogPost { get; set; } //Access to the blog post  the comment is related to 
    }
}