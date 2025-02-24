using GothamPostBlogAPI.Data;
using GothamPostBlogAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; //Enables JWT authentication 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens; //Verifies JWT validity 
using Microsoft.OpenApi.Models;
using System.Text; //Converts secret keys into bytes for token signing 

var builder = WebApplication.CreateBuilder(args);

//Load Configuration (Ensure `appsettings.json` contains JWT Secret & DB Connection)
var configuration = builder.Configuration;

//Register Database Context (use SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

//Add Authentication using JWT Bearer Token
//JWT JSON Web Token securely autheticates users in web application 
//Once user logs in, API generates JWT and send it to the client; JWT is included in the Authorisation header in future requests 
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])) //Secret key for signing 
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Gotham Post Blog API", Version = "v1" });
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
    app.UseSwaggerUI();
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