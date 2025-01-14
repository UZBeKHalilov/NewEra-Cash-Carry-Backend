using Microsoft.EntityFrameworkCore;
using NewEraAPI.Data;
using NewEraAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NewEraDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();


var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>();
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));



// JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new
        SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Secret)),
        ValidateIssuer = false,
        ValidateAudience = false
    };

});


// API Versionning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // Default to version 1.0
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Include version information in the response headers
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Read version from URL segment
});


// Swagger configuration
builder.Services.AddSwaggerGen(
    options =>
    {
        // Swagger hujjatlari (v1 va v2)
        options.SwaggerDoc(
            "v1",
            new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Version = "1.0",
                Title = "E-Commerce API",
                Description = "API for managing an e-commerce platform (v1.0)"
            }
        );
        options.SwaggerDoc(
            "v2",
            new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Version = "2.0",
                Title = "E-Commerce API",
                Description = "API for managing an e-commerce platform (v2.0)"
            }
        );

        // JWT Security Definition
        options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            }
        );

        // Global xavfsizlik talablari
        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
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
                    new string[] {  }
                }
            }
        );
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        //options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API v1.0");
        //options.SwaggerEndpoint("/swagger/v2/swagger.json", "E-Commerce API v2.0");
    });
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
