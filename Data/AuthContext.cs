using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APIMusicaAuth_SerafinParedesAlejandro.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace APIMusicaAuth_SerafinParedesAlejandro.Data
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext(DbContextOptions<AuthContext> options)
             : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // para evitar a la hora de añadir la migración el error de KEY
            modelBuilder.Entity<Auth>().HasNoKey();
            List<IdentityRole> roles = new List<IdentityRole>{
                new IdentityRole{
                Name = "User",
                NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Name = "Administrador",
                    NormalizedName = "ADMINISTRADOR"
                },
                 new IdentityRole
                {
                    Name = "Manager",
                    NormalizedName = "  MANAGER"
                },
             };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
            List<IdentityUser> users = new List<IdentityUser>
             {
                new IdentityUser
                {
                UserName = "usuarionormal",
                NormalizedUserName = "USUARIONORMAL" 
                },
                new IdentityUser
                {
                    UserName = "holasoyAdmin",
                    NormalizedUserName = "HOLASOYADMIN"
                },
                 new IdentityUser
                {
                    UserName = "managerMan",
                    NormalizedUserName = "MANAGERMAN"
                },
             };

            modelBuilder.Entity<IdentityUser>().HasData(users);
            var passwordHasher = new PasswordHasher<IdentityUser>();
            users[0].PasswordHash = passwordHasher.HashPassword(users[0],"1234");
            users[1].PasswordHash = passwordHasher.HashPassword(users[1], "1234");
            users[2].PasswordHash = passwordHasher.HashPassword(users[2], "1234");

            List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>{
                new IdentityUserRole<string>
                {
                    UserId= users[0].Id,
                    RoleId= roles[0].Id
                },
                new IdentityUserRole<string>
                {
                    UserId= users[1].Id,
                    RoleId= roles[1].Id
                },
             };
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(userRoles);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<APIMusicaAuth_SerafinParedesAlejandro.Models.Auth> Auth { get; set; } = default!;
    }
}
