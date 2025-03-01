//Imports
using GothamPostBlogAPI.Data; //Allow access to ApplicationDbContext.cs to handle database operations (project files)
using GothamPostBlogAPI.Services; //Business logic services import (project files)
using Microsoft.AspNetCore.Authentication.JwtBearer; //Enables JWT authentication to verify users with JWT tokens (security)
using Microsoft.AspNetCore.Authorization; //Allows to control API endpoints by defining access levels 
using Microsoft.AspNetCore.Builder; //Configuration middleware (authentication, routing, Swagger; core API)
using Microsoft.AspNetCore.Hosting; //Allows API to run as a web service 
using Microsoft.AspNetCore.Mvc; //Handles API Controllers (how requests and responses are managed; core API)
using Microsoft.EntityFrameworkCore; //Enable database connectivity using EF Core 
using Microsoft.Extensions.Configuration; //Loads app settings from appsettings.json (core API)
using Microsoft.Extensions.DependencyInjection; //Register services (DbContext, AuthService, etc; core API)
using Microsoft.Extensions.Hosting; //Manages API lifecycle (start, stop, or restart; core API)
using Microsoft.IdentityModel.Tokens; //Verifies JWT validity 
using Microsoft.OpenApi.Models; //Enables Swagger for API documentation and testing directly from a browser
using System.Text; //Converts secret keys into bytes for token signing 
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging; //ILogger 

var builder = WebApplication.CreateBuilder(args);

//Load Configuration (Ensure `appsettings.json` contains JWT Secret & DB Connection)
var configuration = builder.Configuration;

//Register Database Context (use SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"))); //Usage of EF Core to let the API communicate with SQLite to store and retrieve data

//Add Authentication using JWT Bearer Token
//JWT JSON Web Token securely authenticates users in web application 
//Once user logs in, API generates JWT and send it to the client; JWT is included in the Authorization header in future requests 
var jwtSecretKey = configuration["Jwt:SecretKey"];
if (string.IsNullOrEmpty(jwtSecretKey) || jwtSecretKey.Length < 32)
{
    throw new InvalidOperationException("JWT SecretKey is too short! It must be at least 32 characters.");
} //If the JWT:Secret Key is missing, the app will throw an exception instead of null value 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //Enable JWT authentication 
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, //Ensure the token belongs to this API (created by this API)
            ValidateAudience = true, //Ensure the token is for this API
            ValidateLifetime = true, //Ensure the token hasn't expired 
            ValidateIssuerSigningKey = true, //Verify the token signature (signed with the secret key)
            ValidIssuer = builder.Configuration["Jwt:Issuer"], //From appsettings.json (who created the token)
            ValidAudience = builder.Configuration["Jwt:Audience"], //From appsettings.json (who should use the token)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])), //Secret key for signing 
            ClockSkew = TimeSpan.Zero // Ensures token expiration is precise
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

//Register Services for Dependency Injection
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BlogPostService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<LikeService>();

//Add Controllers
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Configure Swagger for API Documentation; automatic API docs generation and testing 
builder.Services.AddEndpointsApiExplorer(); //Enable Swagger for API testing
builder.Services.AddSwaggerGen(options => //Adds Swagger UI
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gotham Post Blog API",
        Version = "v1",
        Description = "API for managing users, blog posts, comments, and likes.",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

//Register logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); //Adds logging to the console

var app = builder.Build();

// Apply pending migrations automatically at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();  //Applies migrations automatically
}

//Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GothamPostBlog API v1"); //Where to find the JSON documentation for API 
        c.RoutePrefix = "swagger"; // Swagger URL: http://localhost:5113/swagger -Interface to test all API endpoints 
    });
}

// Global Error Handling Middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var errorMessage = new { error = "An unexpected error occurred." };
        await context.Response.WriteAsJsonAsync(errorMessage);
    });
});

// Enable routing
app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

//Logging in process
//POST /api/auth/login 
//API generates the JWT with the User's ID and Role 
//API sends the JWT back to client 
//Client stores the JWT locally/ sends Authorization headers 
//Future requests include the JWT 
//API will verify the JWT before allowing access
