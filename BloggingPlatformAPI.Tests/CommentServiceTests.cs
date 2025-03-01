//Test case for CommentService - Testing Adding Comments to a Post

using System.Threading.Tasks;
using Xunit;
using Moq;
using GothamPostBlogAPI.Services;
using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Models;
using Microsoft.EntityFrameworkCore;

public class CommentServiceTests
{
    private readonly CommentService _commentService;
    private readonly Mock<ApplicationDbContext> _mockDbContext;

    public CommentServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                      .UseInMemoryDatabase(databaseName: "TestDb").Options;

        var dbContext = new ApplicationDbContext(options);
        _mockDbContext = new Mock<ApplicationDbContext>(options);
        _commentService = new CommentService(dbContext);
    }

    [Fact]
    public async Task AddComment_ShouldSaveCommentToDatabase()
    {
        // Arrange
        var comment = new Comment { CommentContent = "Nice post!", BlogPostId = 1, UserId = 2 };

        // Act
        await _commentService.AddComment(comment);

        // Assert
        Assert.True(_mockDbContext.Object.Comments.Any(c => c.CommentContent == "Nice post!"));
    }
}