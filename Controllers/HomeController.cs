using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MVC_Blog.Data;
using MVC_Blog.Models;
using MVC_Blog.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IFileService fileService, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            // Default Home Page Image
            var DefaultImage = await _fileService.EncodeFileAsync(_configuration["DefaultHomePageImage"]);
            var DefaultContentType = _configuration["DefaultHomePageImage"].Split('.')[1];


            ViewData["HeaderText"] = "Josue's Blog";
            ViewData["SubText"] = "Helping Coders One Line At A Time";
            ViewData["HeaderImage"] = _fileService.DecodeImage(DefaultImage, DefaultContentType);
            
            var allBlogs = await _context.Blogs.ToListAsync();
            return View(allBlogs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
