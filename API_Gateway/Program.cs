using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidAudience = builder.Configuration["Token:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

app.UseAuthentication();
// app.UseAuthorization();

await app.UseOcelot();
app.UseHttpsRedirection();

app.MapGet("/GetToken", () =>
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    // Token'da yer alacak bilgiler
    var claims = new[]
    {
        new Claim("Role", "Admin"),

    };

    // Token oluşturma ayarları
    var token = new JwtSecurityToken(
        issuer: builder.Configuration["Token:Issuer"],
        audience: builder.Configuration["Token:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(10),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
});

app.Run();