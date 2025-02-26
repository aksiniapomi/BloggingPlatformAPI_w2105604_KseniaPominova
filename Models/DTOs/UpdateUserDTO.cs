using System.ComponentModel.DataAnnotations;

namespace GothamPostBlogAPI.Models.DTOs
{
    public class UpdateUserDTO
    {
        [MaxLength(100)]
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MinLength(6)]
        public string Password { get; set; }
    }
}