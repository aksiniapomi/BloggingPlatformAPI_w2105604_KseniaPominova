//Test case for UserService - testing for Registration/Login 

using System.Threading.Tasks;
using Xunit;
using Moq;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<ApplicationDbContext> _mockDbContext;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                      .UseInMemoryDatabase(databaseName: "TestDb").Options;

        var dbContext = new ApplicationDbContext(options);
        _mockDbContext = new Mock<ApplicationDbContext>(options);
        _userService = new UserService(dbContext);
    }

    [Fact]
    public async Task RegisterUser_ShouldCreateUser()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashedpassword" };

        // Act
        await _userService.RegisterUser(user);

        // Assert
        Assert.True(_mockDbContext.Object.Users.Any(u => u.Email == "test@example.com"));
    }

    [Fact]
    public async Task AuthenticateUser_ShouldReturnUserIfValid()
    {
        // Arrange
        var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashedpassword" };
        _mockDbContext.Object.Users.Add(user);
        await _mockDbContext.Object.SaveChangesAsync();

        // Act
        var result = await _userService.AuthenticateUser("test@example.com", "hashedpassword");

        // Assert
        Assert.NotNull(result);
    }
}