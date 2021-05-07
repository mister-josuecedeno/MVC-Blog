using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVC_Blog.Data;
using MVC_Blog.Enums;
using MVC_Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Blog.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IFileService _fileService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IConfiguration _configuration;

        public DataService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, IFileService fileService, UserManager<BlogUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _fileService = fileService;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task ManageDataAsync()
        {
            // This is a wrapper method

            // Task 0: Update database programmatically
            // Must be available when moving to Heroku
            await _context.Database.MigrateAsync();

            // Task 1: Seed Roles
            // Creating Roles and entering the into the system AspNetRoles
            await SeedRolesAsync();

            // Task 2: Seed Users
            // Seed a few users in the system (AspNetUsers)
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            // Are there any roles already in the system?
            if (_context.Roles.Any()) 
            { 
                return; 
            }

            // Spin through an enum and do stuff
            foreach(var stringRole in Enum.GetNames(typeof(BlogRole)))
            {
                // create a role in the system for each role
                var identityRole = new IdentityRole(stringRole);
                await _roleManager.CreateAsync(identityRole);
            }

        }

        private async Task SeedUsersAsync()
        {
            if (_context.Users.Any())
            {
                return;
            }

            // ADD Administrator
            
            var adminUser = new BlogUser()
            {
                Email = "jc_admin@mailinator.com",
                UserName = "jc_admin@mailinator.com",
                FirstName = "Josue",
                LastName = "Cedeno",
                PhoneNumber = "555-555-5555",
                EmailConfirmed = true,
                ImageData = await _fileService.EncodeFileAsync("josuecedeno.jpg"),
                ContentType = "jpg"
            };

            await _userManager.CreateAsync(adminUser, _configuration["AdminPassword"]);
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            // ADD Moderator

            var moderatorUser = new BlogUser()
            {
                Email = "jc_moderator@mailinator.com",
                UserName = "jc_moderator@mailinator.com",
                FirstName = "Josue",
                LastName = "Cedeno",
                PhoneNumber = "555-555-5555",
                EmailConfirmed = true,
                ImageData = await _fileService.EncodeFileAsync("josuecedeno.jpg"),
                ContentType = "jpg"
            };

            await _userManager.CreateAsync(moderatorUser, _configuration["ModeratorPassword"]);
            await _userManager.AddToRoleAsync(moderatorUser, BlogRole.Moderator.ToString());
        }
    }
}
