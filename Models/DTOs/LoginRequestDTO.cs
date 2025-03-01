//Login Request DTO (Data Transfer Objects) - class that defines what data the API expects from a request 
//DTOs 1.Prevents the client from sending unwanted data
//2.Ensure only necessary fields are passed in a request
//3.Keep models separate from request data (easier to modify later; decoupling)
//Instead of the full User model, only Email and Password used for login; prevents unncessary data sent in the API request - safer and more efficient 

using System.ComponentModel.DataAnnotations;

namespace GothamPostBlogAPI.Models.DTOs
{
    public class LoginRequestDTO
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}