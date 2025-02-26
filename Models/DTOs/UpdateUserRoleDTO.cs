//DTO for updating user role (prevents unwanted data from being sent)

using GothamPostBlogAPI.Models;

namespace GothamPostBlogAPI.Models.DTOs
{
    public class UpdateUserRoleDTO
    {
        public UserRole NewRole { get; set; }
    }
}