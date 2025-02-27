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
using Microsoft.Extensions.Logging; // Required for logging

namespace GothamPostBlogAPI.Services
{
    public class AuthService // User authentication service
    {
        private readonly ApplicationDbContext _context; // Allow database access
        private readonly IConfiguration _configuration; // Read secret keys from appsettings.json (for JWT tokens)
        private readonly ILogger<AuthService> _logger; // Logger for tracking authentication events

        public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger) // Constructor injecting dependencies
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // Hashes the password before storing it
        public string HashPassword(string password)
        {
            try
            {
                _logger.LogInformation("Hashing a new password."); // Log password hashing attempt
                return BCrypt.Net.BCrypt.HashPassword(password); // Securely hash the password
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while hashing password."); // Log any errors
                throw;
            }
        }

        // Verifies the password during login
        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                _logger.LogInformation("Verifying password. Input: {Password}, Hashed: {HashedPassword}", password, hashedPassword);
                bool result = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                _logger.LogInformation("Password verification result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while verifying password.");
                throw;
            }
        }

        // Generates a JWT token for authentication
        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["Jwt:SecretKey"];

            // Ensure the secret key is valid (must be at least 32 characters long)
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            {
                _logger.LogError("JWT SecretKey is missing or too short."); // Log issue with JWT SecretKey
                throw new InvalidOperationException("JWT SecretKey is invalid or too short. It must be at least 32 characters.");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                _logger.LogInformation("Generating JWT token for user with ID {UserId}", user.UserId); // Log JWT generation attempt

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
                _logger.LogInformation("JWT token successfully generated for user ID {UserId}", user.UserId); // Log successful JWT generation
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating JWT token for user ID {UserId}", user.UserId); // Log any errors
                throw;
            }
        }
    }
}