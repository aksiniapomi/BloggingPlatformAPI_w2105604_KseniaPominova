//Imports
using GothamPostBlogAPI.Data; //Allow access to ApplicationDbContext.cs to handle database operations (project files)
using GothamPostBlogAPI.Services; //Business logic services import (project files)
using Microsoft.AspNetCore.Authentication.JwtBearer; //Enables JWT authentication to verify users with JWT tokens (security)
using Microsoft.AspNetCore.Authorization; //allows to control to API endpoints by defining access levels 
using Microsoft.AspNetCore.Builder; //Configuration middleware (authetication, routing, Swagger; core API)
using Microsoft.AspNetCore.Hosting; //Allows API to run as a web serivce 
using Microsoft.AspNetCore.Mvc; //Handles API Controllers (how requests and responses are managed; core API)
using Microsoft.EntityFrameworkCore; //Enable database connectivity using EF Core 
using Microsoft.Extensions.Configuration; //Loads app settings from appsettings.json (core API)
using Microsoft.Extensions.DependencyInjection; //Register services (DbContext, AuthService, etc; core API)
using Microsoft.Extensions.Hosting; //Manages API lifecycle (start,stop,or restart; core API)
using Microsoft.IdentityModel.Tokens; //Verifies JWT validity 
using Microsoft.OpenApi.Models; //Enables Swagger for API documentation and testing directly from a browser
using System.Text; //Converts secret keys into bytes for token signing 

var builder = WebApplication.CreateBuilder(args);

//Load Configuration (Ensure `appsettings.json` contains JWT Secret & DB Connection)
var configuration = builder.Configuration;

//Register Database Context (use SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"))); //Usage of EF Core to let the API communicate with SQLite to store and retrieve data

//Add Authentication using JWT Bearer Token
//JWT JSON Web Token securely autheticates users in web application 
//Once user logs in, API generates JWT and send it to the client; JWT is included in the Authorisation header in future requests 
var jwtSecretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing"); //if the JWT:Secret Key is missing, the app will throw an exception instead of null value 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //enable JWT authentication 
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, //Ensure the token belongs to this API (created by this API)
            ValidateAudience = true, //Ensure the token is for this API
            ValidateLifetime = true, //Ensure the token hasn't expired 
            ValidateIssuerSigningKey = true, //Verify the token signature (signed with the secret key)
            ValidIssuer = configuration["Jwt:Issuer"], //From appsettings.json (who created the token)
            ValidAudience = configuration["Jwt:Audience"], //From appsettings.json (who should use the token)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)) //Secret key for signing 
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
builder.Services.AddControllers();

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

var app = builder.Build();

//Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GothamPostBlog API v1"); //where to find the JSON documentation for API 
        c.RoutePrefix = "swagger"; // Swagger URL: http://localhost:5113/swagger -interface to test all API endpoints 
    });
}

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
//API will verify the JWT before allowing the access 