using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuendeIdentityServer.Data
{
    public class UserRoleSeeding: IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "fb16ef9d-baf5-4c1e-9d3e-5323ead086ed",
                    UserId = "9b54b97c-03ec-4037-90a6-0c9cf32e368e"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "835dfaaa-3ffc-403d-b2c3-117caa95c23d",
                    UserId = "b219c2ab-3e98-4de2-8076-542b2aeff3bf"
                });
        }
    }
}
