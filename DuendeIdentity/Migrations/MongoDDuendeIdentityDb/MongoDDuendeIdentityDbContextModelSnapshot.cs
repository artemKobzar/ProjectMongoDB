﻿// <auto-generated />
using System;
using DuendeIdentity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DuendeIdentity.Migrations.MongoDDuendeIdentityDb
{
    [DbContext(typeof(MongoDDuendeIdentityDbContext))]
    partial class MongoDDuendeIdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DuendeIdentity.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "9b54b97c-03ec-4037-90a6-0c9cf32e368e",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "26f34d70-7327-44b6-a988-0accf548d80b",
                            Email = "qwerty@gmail.com",
                            EmailConfirmed = true,
                            FirstName = "Soul",
                            LastName = "Goodman",
                            LockoutEnabled = false,
                            NormalizedEmail = "QWERTY@GMAIL.COM",
                            NormalizedUserName = "LAWYER",
                            PasswordHash = "AQAAAAIAAYagAAAAEEoGbMcNa8T3w6i/z0RWQnO8yqYgeSkbAO7t3uQF6DGkui4KkIqD9ymBBcRJi55U+Q==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "a02de16b-c1c9-4c82-a674-b6ef2259f3cf",
                            TwoFactorEnabled = false,
                            UserName = "lawyer"
                        },
                        new
                        {
                            Id = "b219c2ab-3e98-4de2-8076-542b2aeff3bf",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "e7154f8a-f4dd-4061-8828-8afd24e0931b",
                            Email = "12345@gmail.com",
                            EmailConfirmed = true,
                            FirstName = "Hector",
                            LastName = "Salamanca",
                            LockoutEnabled = false,
                            NormalizedEmail = "12345@GMAIL.COM",
                            NormalizedUserName = "MAFIA",
                            PasswordHash = "AQAAAAIAAYagAAAAEBv1Sobi51oaaadfe/PS6jfBFmrEusBEYUcP2Tkmv9AtA/frVePcVSp/9wY9vz37nw==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "8359a42a-4478-423a-a0e4-270a705f613a",
                            TwoFactorEnabled = false,
                            UserName = "mafia"
                        },
                        new
                        {
                            Id = "с516c2ab-3e98-4de2-8076-542b2aeff1da",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "e52ae288-1c09-401f-9f1e-9429dbaab422",
                            Email = "wick@gmail.com",
                            EmailConfirmed = true,
                            FirstName = "John",
                            LastName = "Wick",
                            LockoutEnabled = false,
                            NormalizedEmail = "WICK@GMAIL.COM",
                            NormalizedUserName = "ASSASSIN",
                            PasswordHash = "AQAAAAIAAYagAAAAEAX74bZJz7DVcgtczZQA4BWDoob92RYXTFxs8INNZcs6kOPMKMT9CnjM3J1emlwnig==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "9e99ade1-e7b6-4c1b-a47a-1f0ab4f59d9e",
                            TwoFactorEnabled = false,
                            UserName = "assassin"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "fb16ef9d-baf5-4c1e-9d3e-5323ead086ed",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = "835dfaaa-3ffc-403d-b2c3-117caa95c23d",
                            Name = "User",
                            NormalizedName = "USER"
                        },
                        new
                        {
                            Id = "rb16ec9d-ba44-4c1e-9d3e-5323ead087aw",
                            Name = "Participant",
                            NormalizedName = "PARTICIPANT"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ClaimType = "role",
                            ClaimValue = "Admin",
                            UserId = "b219c2ab-3e98-4de2-8076-542b2aeff3bf"
                        },
                        new
                        {
                            Id = 2,
                            ClaimType = "role",
                            ClaimValue = "User",
                            UserId = "9b54b97c-03ec-4037-90a6-0c9cf32e368e"
                        },
                        new
                        {
                            Id = 3,
                            ClaimType = "role",
                            ClaimValue = "Participant",
                            UserId = "с516c2ab-3e98-4de2-8076-542b2aeff1da"
                        },
                        new
                        {
                            Id = 4,
                            ClaimType = "name",
                            ClaimValue = "12345@gmail.com",
                            UserId = "b219c2ab-3e98-4de2-8076-542b2aeff3bf"
                        },
                        new
                        {
                            Id = 5,
                            ClaimType = "name",
                            ClaimValue = "qwerty@gmail.com",
                            UserId = "9b54b97c-03ec-4037-90a6-0c9cf32e368e"
                        },
                        new
                        {
                            Id = 6,
                            ClaimType = "name",
                            ClaimValue = "wick@gmail.com",
                            UserId = "с516c2ab-3e98-4de2-8076-542b2aeff1da"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "9b54b97c-03ec-4037-90a6-0c9cf32e368e",
                            RoleId = "fb16ef9d-baf5-4c1e-9d3e-5323ead086ed"
                        },
                        new
                        {
                            UserId = "b219c2ab-3e98-4de2-8076-542b2aeff3bf",
                            RoleId = "835dfaaa-3ffc-403d-b2c3-117caa95c23d"
                        },
                        new
                        {
                            UserId = "с516c2ab-3e98-4de2-8076-542b2aeff1da",
                            RoleId = "rb16ec9d-ba44-4c1e-9d3e-5323ead087aw"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("DuendeIdentity.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("DuendeIdentity.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DuendeIdentity.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("DuendeIdentity.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
