using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuendeIdentity.Data
{
    public class RoleSeeding : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {

            builder.HasData(
                new IdentityRole
                {
                    Id = "fb16ef9d-baf5-4c1e-9d3e-5323ead086ed",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "835dfaaa-3ffc-403d-b2c3-117caa95c23d",
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = "rb16ec9d-ba44-4c1e-9d3e-5323ead087aw",
                    Name = "Participant",
                    NormalizedName = "PARTICIPANT"
                });
        }
    }
}
