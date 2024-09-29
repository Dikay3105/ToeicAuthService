﻿// <auto-generated />
using System;
using AuthService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AuthService.Migrations
{
    [DbContext(typeof(AuthDbContext))]
    [Migration("20240927145940_initDB")]
    partial class initDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("AuthService.Models.EmailConfirm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("ExpiredAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("EmailConfirms");
                });

            modelBuilder.Entity("AuthService.Models.Permission", b =>
                {
                    b.Property<int>("PermissionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("PermissionID"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PermissionName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("PermissionID");

                    b.ToTable("Permissions");

                    b.HasData(
                        new
                        {
                            PermissionID = 1,
                            Description = "Quyền xem người dùng",
                            PermissionName = "ViewUsers"
                        },
                        new
                        {
                            PermissionID = 2,
                            Description = "Quyền chỉnh sửa người dùng",
                            PermissionName = "EditUsers"
                        },
                        new
                        {
                            PermissionID = 3,
                            Description = "Quyền xóa người dùng",
                            PermissionName = "DeleteUsers"
                        });
                });

            modelBuilder.Entity("AuthService.Models.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("ExpiredAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("IssuedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("JwtId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");

                    b.HasData(
                        new
                        {
                            Id = new Guid("c9c235f6-693d-44be-a7b6-9ab238ef5c6a"),
                            ExpiredAt = new DateTime(2024, 10, 27, 21, 59, 40, 312, DateTimeKind.Local).AddTicks(1661),
                            IsRevoked = false,
                            IsUsed = false,
                            IssuedAt = new DateTime(2024, 9, 27, 21, 59, 40, 312, DateTimeKind.Local).AddTicks(1667),
                            JwtId = "test",
                            Token = "refresh_token_1",
                            UserId = 1
                        },
                        new
                        {
                            Id = new Guid("fe0a6332-9d14-428a-98b6-49db7b0aae7b"),
                            ExpiredAt = new DateTime(2024, 10, 27, 21, 59, 40, 312, DateTimeKind.Local).AddTicks(1670),
                            IsRevoked = false,
                            IsUsed = false,
                            IssuedAt = new DateTime(2024, 9, 27, 21, 59, 40, 312, DateTimeKind.Local).AddTicks(1671),
                            JwtId = "test",
                            Token = "refresh_token_2",
                            UserId = 2
                        });
                });

            modelBuilder.Entity("AuthService.Models.ResetPassword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<bool>("Used")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ResetPasswords");
                });

            modelBuilder.Entity("AuthService.Models.Role", b =>
                {
                    b.Property<int>("RoleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("RoleID"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("RoleID");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleID = 1,
                            Description = "Quản trị viên có toàn quyền",
                            RoleName = "Admin"
                        },
                        new
                        {
                            RoleID = 2,
                            Description = "Người dùng thông thường",
                            RoleName = "User"
                        });
                });

            modelBuilder.Entity("AuthService.Models.RolePermission", b =>
                {
                    b.Property<int>("RolePermissionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("RolePermissionID"));

                    b.Property<int>("PermissionID")
                        .HasColumnType("int");

                    b.Property<int>("RoleID")
                        .HasColumnType("int");

                    b.HasKey("RolePermissionID");

                    b.HasIndex("PermissionID");

                    b.HasIndex("RoleID");

                    b.ToTable("RolePermissions");

                    b.HasData(
                        new
                        {
                            RolePermissionID = 1,
                            PermissionID = 1,
                            RoleID = 1
                        },
                        new
                        {
                            RolePermissionID = 2,
                            PermissionID = 2,
                            RoleID = 1
                        },
                        new
                        {
                            RolePermissionID = 3,
                            PermissionID = 3,
                            RoleID = 1
                        },
                        new
                        {
                            RolePermissionID = 4,
                            PermissionID = 1,
                            RoleID = 2
                        });
                });

            modelBuilder.Entity("AuthService.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserID"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("lastPasswordChange")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserID");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserID = 1,
                            CreatedAt = new DateTime(2024, 9, 27, 21, 59, 40, 312, DateTimeKind.Local).AddTicks(1451),
                            Email = "admin@example.com",
                            FirstName = "Admin",
                            LastName = "User",
                            PasswordHash = "hashed_password_1",
                            Salt = "salt1",
                            Username = "admin",
                            lastPasswordChange = "none"
                        },
                        new
                        {
                            UserID = 2,
                            CreatedAt = new DateTime(2024, 9, 27, 21, 59, 40, 312, DateTimeKind.Local).AddTicks(1464),
                            Email = "john.doe@example.com",
                            FirstName = "John",
                            LastName = "Doe",
                            PasswordHash = "hashed_password_2",
                            Salt = "salt2",
                            Username = "john_doe",
                            lastPasswordChange = "none"
                        },
                        new
                        {
                            UserID = 3,
                            CreatedAt = new DateTime(2024, 9, 27, 21, 59, 40, 312, DateTimeKind.Local).AddTicks(1466),
                            Email = "jane.smith@example.com",
                            FirstName = "Jane",
                            LastName = "Smith",
                            PasswordHash = "hashed_password_3",
                            Salt = "salt3",
                            Username = "jane_smith",
                            lastPasswordChange = "none"
                        });
                });

            modelBuilder.Entity("AuthService.Models.UserRole", b =>
                {
                    b.Property<int>("UserRoleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserRoleID"));

                    b.Property<int>("RoleID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("UserRoleID");

                    b.HasIndex("RoleID");

                    b.HasIndex("UserID");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            UserRoleID = 1,
                            RoleID = 1,
                            UserID = 1
                        },
                        new
                        {
                            UserRoleID = 2,
                            RoleID = 2,
                            UserID = 2
                        },
                        new
                        {
                            UserRoleID = 3,
                            RoleID = 2,
                            UserID = 3
                        });
                });

            modelBuilder.Entity("AuthService.Models.RefreshToken", b =>
                {
                    b.HasOne("AuthService.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuthService.Models.ResetPassword", b =>
                {
                    b.HasOne("AuthService.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AuthService.Models.RolePermission", b =>
                {
                    b.HasOne("AuthService.Models.Permission", null)
                        .WithMany()
                        .HasForeignKey("PermissionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AuthService.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AuthService.Models.UserRole", b =>
                {
                    b.HasOne("AuthService.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AuthService.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}