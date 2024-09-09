using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DuendeIdentity.Data
{
    public class ClaimSeediing : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder.HasData(
                new IdentityUserClaim<string>
                {
                    Id = 1,
                    ClaimType = "role",
                    ClaimValue = "Admin",
                    UserId = "b219c2ab-3e98-4de2-8076-542b2aeff3bf"
                },
                new IdentityUserClaim<string>
                {
                    Id = 2,
                    ClaimType = "role",
                    ClaimValue = "User",
                    UserId = "9b54b97c-03ec-4037-90a6-0c9cf32e368e"
                },
                new IdentityUserClaim<string>
                {
                    Id = 3,
                    ClaimType = "role",
                    ClaimValue = "Participant",
                    UserId = "с516c2ab-3e98-4de2-8076-542b2aeff1da"
                },
                new IdentityUserClaim<string>
                {
                    Id = 4,
                    ClaimType = "name",
                    ClaimValue = "12345@gmail.com",
                    UserId = "b219c2ab-3e98-4de2-8076-542b2aeff3bf"
                },
                new IdentityUserClaim<string>
                {
                    Id = 5,
                    ClaimType = "name",
                    ClaimValue = "qwerty@gmail.com",
                    UserId = "9b54b97c-03ec-4037-90a6-0c9cf32e368e"
                },
                new IdentityUserClaim<string>
                {
                    Id = 6,
                    ClaimType = "name",
                    ClaimValue = "wick@gmail.com",
                    UserId = "с516c2ab-3e98-4de2-8076-542b2aeff1da"
                });
        }
    }
}
