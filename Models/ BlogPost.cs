//The BlogPost model represents the blog posts that users create 
//A user can create multiple blog posts(One-to-Many with User)
//A blog posts can have multiple comments (1:M with Comment)
//A blog post can have multiple likes (1:M with Like)
//A blog post belongs to one category(One Category can have multiple blog posts; 1:M relationship)

using System; //for the DateTime in the core .Net functionality 
using System.Collections.Generic; //namespace for working with collections to store the comments and likes 
using System.ComponentModel.DataAnnotations; //data validation 
using GothamPostBlogAPI.Models; //own Models namespace to use custome models in the BlogPost.cs 

namespace GothamPostBlogAPI.Models
{
    public class BlogPost
    {
        [Key]
        public int BlogPostId { get; set; }  //Primary Key, unique identifier for the BlogPost

        [Required, MaxLength(255)]
        public string Title { get; set; }  //Blog post title

        [Required]
        public string Content { get; set; }  //Main blog post content

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;  //Timestamp for when the post was created

        //Foreign Keys from User and Category models 
        public int UserId { get; set; }  //FK to User (author of the post)
        public User User { get; set; }

        public int CategoryId { get; set; }  //FK to Category
        public Category Category { get; set; }

        //A List of comments belonging to the post, EF will use it to load all coments belonging to the post 
        public List<Comment> Comments { get; set; } = new();  //A blog post can have multiple comments
                                                              //A List of likes belonging to the post 
        public List<Like> Likes { get; set; } = new();  //A blog post can have multiple likes
    }
}