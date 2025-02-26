using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models; //Import User model
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net; //BCrypt library required for password hasing 

namespace GothamPostBlogAPI.Services
{
    public class AuthService //user authentication 
    {

        private readonly ApplicationDbContext _context; //allow database access 
        private readonly IConfiguration _configuration; //read secret keys from appsettings.json (for JWT tokens)

        public AuthService(ApplicationDbContext context, IConfiguration configuration) //constructor injecting dependencies ApplicationDbContext and IConfiguration (config settings)
        {
            _context = context;
            _configuration = configuration;
        }

        // Hashes the password before storing it
        public string HashPassword(string password) //take plain text password and hash using BCrypt 
        {
            return BCrypt.Net.BCrypt.HashPassword(password);  //reutrn hashed password stored in the database
                                                              //each time the same password is hashed, a different result is generated (because of salting)
        }

        //Verifies the password during login
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword); //take plain password, compare against stored hashed password (return true if match - correct password; false - no match)
        }
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("ThisIsA32CharacterLongSuperSecretKey!"); //Secret key

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
