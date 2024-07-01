﻿// <auto-generated />
using System;
using ClayAccessControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ClayAccessControl.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240630130153_AddDoorStatus")]
    partial class AddDoorStatus
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("ClayAccessControl.Core.Entities.Door", b =>
                {
                    b.Property<int>("DoorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DoorName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("OfficeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RequiredAccessLevel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("DoorId");

                    b.HasIndex("OfficeId");

                    b.ToTable("Doors");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("DoorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("EventId");

                    b.HasIndex("DoorId");

                    b.HasIndex("UserId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.Office", b =>
                {
                    b.Property<int>("OfficeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("OfficeName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("OfficeId");

                    b.ToTable("Offices");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccessLevel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("OfficeId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.HasIndex("OfficeId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.UserDoorAccess", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DoorId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasAccess")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId", "DoorId");

                    b.HasIndex("DoorId");

                    b.ToTable("UserDoorAccesses");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.Door", b =>
                {
                    b.HasOne("ClayAccessControl.Core.Entities.Office", "Office")
                        .WithMany("Doors")
                        .HasForeignKey("OfficeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Office");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.Event", b =>
                {
                    b.HasOne("ClayAccessControl.Core.Entities.Door", "Door")
                        .WithMany("Events")
                        .HasForeignKey("DoorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ClayAccessControl.Core.Entities.User", "User")
                        .WithMany("Events")
                        .HasForeignKey("UserId");

                    b.Navigation("Door");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.User", b =>
                {
                    b.HasOne("ClayAccessControl.Core.Entities.Office", "Office")
                        .WithMany()
                        .HasForeignKey("OfficeId");

                    b.Navigation("Office");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.UserDoorAccess", b =>
                {
                    b.HasOne("ClayAccessControl.Core.Entities.Door", "Door")
                        .WithMany("UserDoorAccesses")
                        .HasForeignKey("DoorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ClayAccessControl.Core.Entities.User", "User")
                        .WithMany("UserDoorAccesses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Door");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.UserRole", b =>
                {
                    b.HasOne("ClayAccessControl.Core.Entities.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ClayAccessControl.Core.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.Door", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("UserDoorAccesses");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.Office", b =>
                {
                    b.Navigation("Doors");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("ClayAccessControl.Core.Entities.User", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("UserDoorAccesses");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}