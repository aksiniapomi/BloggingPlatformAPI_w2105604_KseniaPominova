using System.ComponentModel.DataAnnotations;

namespace GothamPostBlogAPI.Models.DTOs
{
    public class UpdateUserDTO
    {
        [MaxLength(100)]
        public required string Username { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [MinLength(6)]
        public required string Password { get; set; }
    }
}