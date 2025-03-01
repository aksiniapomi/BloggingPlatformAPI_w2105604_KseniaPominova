using System.ComponentModel.DataAnnotations;

namespace GothamPostBlogAPI.Models.DTOs
{
    //DTO for Creating a Blog Post (Prevents setting `UserId`)
    public class BlogPostDTO
    {
        [Required, MaxLength(255)]
        public required string Title { get; set; } //Title required, max 255 characters

        [Required]
        public required string Content { get; set; } //Content required

        [Required]
        public int CategoryId { get; set; } //Prevent users from manually setting UserId
    }

    //DTO for Retrieving a Blog Post (Prevents Circular References)
    public class BlogPostResponseDTO
    {
        public int BlogPostId { get; set; } //Unique ID for reference

        public required string Title { get; set; } //Title of the post

        public required string Content { get; set; } //Main post content

        public DateTime DateCreated { get; set; } //When the post was created

        public int UserId { get; set; } //User who wrote the post (ID only)

        public required string Username { get; set; } //To display author without exposing full User object
    }
}