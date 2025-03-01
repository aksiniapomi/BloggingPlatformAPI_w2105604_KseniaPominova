using System.ComponentModel.DataAnnotations;

namespace GothamPostBlogAPI.Models.DTOs
{
    public class CategoryDTO
    {
        [Required, MaxLength(100)] //force valid category names 
        public required string Name { get; set; }
    }
}