//Category model stores different categories posts belong to 
//A Category can contain multiple blog posts(One-to-Many relationship with BlogPost)
//A blog post belongs only to one category 

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GothamPostBlogAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }  //Primary Key, unique identifier

        [Required, MaxLength(100)]
        public string Name { get; set; }  //Category name

        //List stores multiple blog posts; one category can have multiple blog posts 
        public List<BlogPost> BlogPosts { get; set; } = new(); // Navigation Property; =new(); initialises the List, null be default
    }
}


