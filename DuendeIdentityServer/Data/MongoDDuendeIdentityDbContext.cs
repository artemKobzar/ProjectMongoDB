using DuendeIdentityServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuendeIdentityServer.Data
{
    public class MongoDDuendeIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public MongoDDuendeIdentityDbContext(DbContextOptions<MongoDDuendeIdentityDbContext> options): base(options)
        {          
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new RoleSeeding());
            builder.ApplyConfiguration(new UserSeeding());
            builder.ApplyConfiguration(new UserRoleSeeding());
        }
    }
}
