using IdentityService4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService4.Data
{
    public class UserSeeding : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            builder.HasData(
                new ApplicationUser
                {
                    Id = "9b54b97c-03ec-4037-90a6-0c9cf32e368e",
                    Email = "qwerty@gmail.com",
                    NormalizedEmail = "QWERTY@GMAIL.COM",
                    FirstName = "Soul",
                    LastName = "Goodman",
                    UserName = "lawyer",
                    NormalizedUserName = "LAWYER",
                    PasswordHash = hasher.HashPassword(null, "12345"),
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = "b219c2ab-3e98-4de2-8076-542b2aeff3bf",
                    Email = "12345@gmail.com",
                    NormalizedEmail = "12345@GMAIL.COM",
                    FirstName = "Hector",
                    LastName = "Salamanca",
                    UserName = "mafia",
                    NormalizedUserName = "MAFIA",
                    PasswordHash = hasher.HashPassword(null, "qwerty"),
                    EmailConfirmed = true
                });
        }
    }
}
