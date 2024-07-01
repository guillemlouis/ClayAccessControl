using ClayAccessControl.Core.Entities;
using ClayAccessControl.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClayAccessControl.Infrastructure.Data{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;

        public DatabaseSeeder(ApplicationDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task SeedAsync()
        {
            // Seed Roles
            await SeedRolesAsync();

            // Seed Office
            var office = await SeedOfficeAsync();

            // Seed Users
            await SeedUsersAsync(office);

            // Seed Doors
            await SeedDoorsAsync(office);

            // Seed initial UserDoorAccess
            await SeedUserDoorAccessAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (!await _context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { RoleName = "Admin", AccessLevel = 5, Description = "Full access to all systems" },
                    new Role { RoleName = "Manager", AccessLevel = 3, Description = "Access to most areas and management functions" },
                    new Role { RoleName = "Employee", AccessLevel = 1, Description = "Basic access to common areas" }
                };
                await _context.Roles.AddRangeAsync(roles);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<Office> SeedOfficeAsync()
        {
            if (!await _context.Offices.AnyAsync())
            {
                var office = new Office
                {
                    OfficeName = "Clay Headquarters",
                    Address = "123 Clay Street, Amsterdam",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.Offices.AddAsync(office);
                await _context.SaveChangesAsync();
                return office;
            }
            return await _context.Offices.FirstAsync();
        }

        private async Task SeedUsersAsync(Office office)
        {
            if (!await _context.Users.AnyAsync())
            {
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
                var managerRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Manager");
                var employeeRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Employee");

                var users = new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        PasswordHash = _passwordService.HashPassword("password"),
                        Email = "admin@clay.com",
                        FirstName = "Admin",
                        LastName = "User",
                        IsActive = true,
                        OfficeId = office.OfficeId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        UserRoles = new List<UserRole> { new UserRole { Role = adminRole ?? throw new InvalidOperationException("adminRole not found")} }
                    },
                    new User
                    {
                        Username = "manager",
                        PasswordHash = _passwordService.HashPassword("password"),
                        Email = "manager@clay.com",
                        FirstName = "Manager",
                        LastName = "User",
                        IsActive = true,
                        OfficeId = office.OfficeId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        UserRoles = new List<UserRole> { new UserRole { Role = managerRole ?? throw new InvalidOperationException("managerRole not found")} }
                    },
                    new User
                    {
                        Username = "employee1",
                        PasswordHash = _passwordService.HashPassword("password"),
                        Email = "employee1@clay.com",
                        FirstName = "Employee",
                        LastName = "One",
                        IsActive = true,
                        OfficeId = office.OfficeId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        UserRoles = new List<UserRole> { new UserRole { Role = employeeRole ?? throw new InvalidOperationException("employeeRole not found") } }
                    },
                    new User
                    {
                        Username = "employee2",
                        PasswordHash = _passwordService.HashPassword("password"),
                        Email = "employee2@clay.com",
                        FirstName = "Employee",
                        LastName = "Two",
                        IsActive = true,
                        OfficeId = office.OfficeId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        UserRoles = new List<UserRole> { new UserRole { Role = employeeRole ?? throw new InvalidOperationException("employeeRole not found")} }
                    }
                };
                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedDoorsAsync(Office office)
        {
            if (!await _context.Doors.AnyAsync())
            {
                var doors = new List<Door>
                {
                    new Door
                    {
                        DoorName = "Main Entrance",
                        RequiredAccessLevel = 1, // All employees can access
                        Status = DoorStatusEnum.Locked,
                        OfficeId = office.OfficeId
                    },
                    new Door
                    {
                        DoorName = "Storage Room",
                        RequiredAccessLevel = 3, // Only managers and admins can access
                        Status = DoorStatusEnum.Locked,
                        OfficeId = office.OfficeId
                    }
                };
                await _context.Doors.AddRangeAsync(doors);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedUserDoorAccessAsync()
        {
            if (!await _context.UserDoorAccesses.AnyAsync())
            {
                var users = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
                var doors = await _context.Doors.ToListAsync();

                var userDoorAccesses = new List<UserDoorAccess>();

                foreach (var user in users)
                {
                    foreach (var door in doors)
                    {
                        var highestUserAccessLevel = user.UserRoles.Max(ur => ur.Role.AccessLevel);
                        userDoorAccesses.Add(new UserDoorAccess
                        {
                            UserId = user.UserId,
                            DoorId = door.DoorId,
                            HasAccess = highestUserAccessLevel >= door.RequiredAccessLevel
                        });
                    }
                }

                await _context.UserDoorAccesses.AddRangeAsync(userDoorAccesses);
                await _context.SaveChangesAsync();
            }
        }
    }
}