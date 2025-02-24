using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GothamPostBlogAPI.Models; // Import User model
using Microsoft.IdentityModel.Tokens;

namespace GothamPostBlogAPI.Services
{
    public class AuthService
    {

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("YourSuperSecretKey123!"); //Secret key

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.UserId.ToString()), //Stores User ID
            new Claim(ClaimTypes.Role, user.Role.ToString()) //stores Role (Admin/User)
        }),
                Expires = DateTime.UtcNow.AddHours(2), //Token expires in 2 hours
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
