using System.ComponentModel.DataAnnotations; //namespace provides data validation attributes to use in DTOs and models and enforce rules e.g. required fields, length restrictions 
using GothamPostBlogAPI.Models;

namespace GothamPostBlogAPI.Models.DTOs
{
    public class UserDTO
    {
        [Required, MaxLength(100)] //mandatory, cannot be null; length not greater than n; .DataAnnotations enforce this 
        public required string Username { get; set; }

        [Required, EmailAddress] //ensure the emaill address format is valid 
        public required string Email { get; set; }

        [Required, MinLength(6)] //ensure password is at least 6 characters 
        public required string Password { get; set; }
        public UserRole? Role { get; set; }
    }
}
