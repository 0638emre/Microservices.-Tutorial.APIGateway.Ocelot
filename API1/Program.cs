using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

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


// app.UseAuthentication();
// app.UseAuthorization();


app.Run();