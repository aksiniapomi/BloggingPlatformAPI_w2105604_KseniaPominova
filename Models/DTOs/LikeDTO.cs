using System.ComponentModel.DataAnnotations;

namespace GothamPostBlogAPI.Models.DTOs
{
    public class LikeDTO
    {
        [Required]
        public int BlogPostId { get; set; }
        //Users can only like a post, not set `UserId`, or LikeId
    }
}