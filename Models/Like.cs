//A Like represents a User liking a blog post 
//A User can like multiple posts (One-to-Many relationship)
//A Blog Post can be liked by multiple Users (One-to-many relationship) 
//Many posts can be liked by many Users (M:M relationship)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//Import all the models to establish relationships 
using GothamPostBlogAPI.Models;

namespace GothamPostBlogAPI.Models
{
    public class Like
    {
        [Key]
        public int LikeId { get; set; } //Primary Key, unique identifier (number) 

        public int UserId { get; set; } //User is the Foreign Key (which User liked the post)
        public User User { get; set; } //Navigation property, allows full access to the object for Entity Framework core 

        public int BlogPostId { get; set; } //BlogPost is the Foreign Key 
        public BlogPost BlogPost { get; set; }

        public Like(User user, BlogPost blogPost)
        {
            User = user; //likes are always linked to user 
            UserId = user.UserId;
            BlogPost = blogPost; //cannot be null 
        }
    }
}
