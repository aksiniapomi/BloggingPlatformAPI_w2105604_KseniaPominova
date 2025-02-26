using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models; // Import User model
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net; // BCrypt library required for password hashing
using Microsoft.Extensions.Configuration; // Required for accessing appsettings.json values

namespace GothamPostBlogAPI.Services
{
    public class AuthService // User authentication service
    {
        private readonly ApplicationDbContext _context; // Allow database access
        private readonly IConfiguration _configuration; // Read secret keys from appsettings.json (for JWT tokens)

        public AuthService(ApplicationDbContext context, IConfiguration configuration) // Constructor injecting dependencies
        {
            _context = context;
            _configuration = configuration;
        }

        // Hashes the password before storing it
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password); // Securely hash the password
        }

        // Verifies the password during login
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword); // Compare plain password with stored hash
        }

        // Generates a JWT token for authentication
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["Jwt:SecretKey"];

            // Ensure the secret key is valid (must be at least 32 characters long)
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            {
                throw new InvalidOperationException("JWT SecretKey is invalid or too short. It must be at least 32 characters.");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Store User ID
                    new Claim(ClaimTypes.Role, user.Role.ToString()), // Store Role (Admin/User)
                }),

                Expires = DateTime.UtcNow.AddHours(2), // Token expires in 2 hours
                Issuer = _configuration["Jwt:Issuer"] ?? "GothamPostBlogAPI",
                Audience = _configuration["Jwt:Audience"] ?? "GothamPostBlogAPI",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}