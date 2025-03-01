//Test case for BlogPostService - Testing Blog Post Creation

using System.Threading.Tasks;
using Xunit;
using Moq;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Add this

public class BlogPostServiceTests
{
    private readonly BlogPostService _blogPostService;
    private readonly Mock<ApplicationDbContext> _mockDbContext;
    private readonly Mock<ILogger<BlogPostService>> _mockLogger; // Mock Logger

    public BlogPostServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                      .UseInMemoryDatabase(databaseName: "TestDb").Options;

        var dbContext = new ApplicationDbContext(options);
        _mockDbContext = new Mock<ApplicationDbContext>(options);
        _mockLogger = new Mock<ILogger<BlogPostService>>(); // Create mock logger

        _blogPostService = new BlogPostService(dbContext, _mockLogger.Object); // Pass logger
    }

    [Fact]
    public async Task CreatePost_ShouldAddPostToDatabase()
    {
        // Arrange
        var blogPost = new BlogPost { Title = "Test Post", Content = "This is a test post", UserId = 1 };

        // Act
        await _blogPostService.CreatePost(blogPost);

        // Assert
        Assert.NotNull(await _mockDbContext.Object.BlogPosts.FindAsync(blogPost.BlogPostId));
    }
}