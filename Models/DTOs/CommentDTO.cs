using System.ComponentModel.DataAnnotations;

namespace GothamPostBlogAPI.Models.DTOs
{
    public class CommentDTO
    {
        [Required]
        public string CommentContent { get; set; }

        [Required]
        public int BlogPostId { get; set; }
    }
}
