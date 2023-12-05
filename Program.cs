using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using APIMusicaAuth_SerafinParedesAlejandro.Data;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<ChinookContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionChinook") ?? throw new InvalidOperationException("Connection string 'ConexionChinook'not found.")));

builder.Services.AddDbContext<UsersContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("UsersDb") ?? 
throw new InvalidOperationException("Connection string 'UsersContext' not found.")));

builder.Services.AddIdentityCore<IdentityUser>(options =>
 options.SignIn.RequireConfirmedEmail = false)
 .AddRoles<IdentityRole>()
 .AddEntityFrameworkStores<UsersContext>();

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.RequireHttpsMetadata = false;
     options.SaveToken = true;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidAudience = builder.Configuration["Jwt:Audience"],
         IssuerSigningKey = new
    SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
     };
 });



// linea añadida para ignorar los ciclos de Json
builder.Services.AddControllers().AddJsonOptions(
    x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

// añadir antes el authent que el authoriz
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
