using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVC_Blog.Data;
using MVC_Blog.Models;
using MVC_Blog.Services;

namespace MVC_Blog.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;

        public BlogsController(ApplicationDbContext context, IFileService fileService, IConfiguration configuration)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
        }

        // GET: Blogs
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        // [Authorize]
        // [Authorize(Roles = "Administrator")]
        // [Authorize(Roles = "Moderator")]
        // [Authorize(Roles = "Administrator,Moderator")]
        // [AllowAnonymous] - open to everyone

        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Name,Description")] Blog blog, IFormFile BlogImage)
        {
            if (ModelState.IsValid)
            {
                // Add Default Image (and ContentType)
                blog.BlogImage = (await _fileService.EncodeFileAsync(BlogImage)) ??
                                  await _fileService.EncodeFileAsync(_configuration["DefaultBlogImage"]);

                blog.ContentType = (_fileService.ContentType(BlogImage)) ??
                                    _configuration["DefaultBlogImage"].Split('.')[1];


                // Add date programmatically (only need Created)
                blog.Created = DateTime.Now;

                

                _context.Add(blog);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
                //return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blogs/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Created,Updated,BlogImage,ContentType")] Blog blog, IFormFile newBlogImage)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(newBlogImage is not null)
                    {
                        blog.BlogImage = await _fileService.EncodeFileAsync(newBlogImage);
                        blog.ContentType = _fileService.ContentType(newBlogImage);
                    }

                    // Edit Update DateTime
                    blog.Updated = DateTime.Now;

                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
