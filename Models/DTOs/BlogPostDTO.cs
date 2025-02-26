//DTOs prevent users from sending unnecessary files 
//Improve security (e.g. prevent the user from setting the role)
//Ensure validation (of the email format, fields filled)

using System.ComponentModel.DataAnnotations;

namespace GothamPostBlogAPI.Models.DTOs
{
    public class BlogPostDTO
    {
        [Required, MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int CategoryId { get; set; }  //Prevent users from setting `UserId`
    }
}