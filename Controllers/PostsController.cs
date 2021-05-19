using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVC_Blog.Data;
using MVC_Blog.Models;
using MVC_Blog.Services;
using X.PagedList;

namespace MVC_Blog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly BasicSlugService _slugService;
        private readonly SearchService _searchService;

        public PostsController(ApplicationDbContext context, IFileService fileService, IConfiguration configuration, BasicSlugService slugService, SearchService searchService)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
            _slugService = slugService;
            _searchService = searchService;
        }

        // GET: Posts with BlogPostIndex = n AND page = n
        public async Task<ActionResult> BlogPostIndex(int? id, int? page = 1)
        {
            if(id == null)
            {
                return NotFound();
            }

            var blog = _context.Blogs.Find(id);
            ViewData["HeaderText"] = blog.Name;
            ViewData["SubText"] = blog.Description;
            ViewData["HeaderImage"] = _fileService.DecodeImage(blog.BlogImage, blog.ContentType);

            //var pageNumber = page ?? 1;
            var pageSize = 5;

            // var blogPosts = await _context.Posts.Where(p => p.BlogId == id).ToListAsync();
            var blogPosts = await _context.Posts.Where(p => p.BlogId == id && p.PublishState.Equals(Enums.PublishState.ProductionReady))
                                                .OrderByDescending(b => b.Created)
                                                .ToPagedListAsync(page, pageSize);

            // Show everything for Admin/Moderators
            if (User.IsInRole("Administrator") || User.IsInRole("Moderator"))
            {
                blogPosts = await _context.Posts.Where(p => p.BlogId == id)
                                                .OrderByDescending(b => b.Created)
                                                .ToPagedListAsync(page, pageSize);
            }

            // use an existing view to show the data
            return View(blogPosts);
        }


        // GET: Posts
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index()
        {
            // Default Post Page Image
            var DefaultImage = await _fileService.EncodeFileAsync(_configuration["DefaultPostPageImage"]);
            var DefaultContentType = _configuration["DefaultPostPageImage"].Split('.')[1];


            ViewData["HeaderText"] = "Post Index";
            ViewData["SubText"] = "Get Your Daily Posts";
            ViewData["HeaderImage"] = _fileService.DecodeImage(DefaultImage, DefaultContentType);

            var applicationDbContext = _context.Posts.Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5 (COMMENT OUT)
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts
        //        .Include(p => p.Blog)
        //        .Include(p => p.Comments)
        //        .ThenInclude(c => c.Author)
        //        .FirstOrDefaultAsync(m => m.Id == id);
            
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewData["HeaderText"] = post.Title;
        //    ViewData["SubText"] = post.Abstract;
        //    ViewData["AuthorText"] = $"Created by Josue Cedeno on {post.Created.ToString("MMM dd, yyyy")}";
        //    ViewData["HeaderImage"] = _fileService.DecodeImage(post.ImageData, post.ContentType);

        //    return View(post);
        //}

        // GET: Posts/Details/slug
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Slug == slug && m.PublishState.Equals(Enums.PublishState.ProductionReady));

            // Show everything for Admin/Moderators
            if (User.IsInRole("Administrator") || User.IsInRole("Moderator"))
            {
                post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            }

            if (post == null)
            {
                return NotFound();
            }

            ViewData["HeaderText"] = post.Title;
            ViewData["SubText"] = post.Abstract;
            ViewData["AuthorText"] = $"Created by Josue Cedeno on {post.Created.ToString("MMM dd, yyyy")}";
            ViewData["HeaderImage"] = _fileService.DecodeImage(post.ImageData, post.ContentType);

            return View(post);
        }


        // GET: Posts/Create

        //public IActionResult Create()
        //{
        //    ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description");
        //   return View();
        //}
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create(int? blogId)
        {

            var post = new Post();

            if (blogId is null)
            {
                ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description");
            }
            else            
            {
                post.BlogId = (int)blogId;
            };
            
            return View(post);
        }


        // Search Index
        public async Task<IActionResult> SearchIndex(string searchString, int? page = 1)
        {
            ViewData["SearchString"] = searchString;
            
            // Step 1: Results from search string
            var posts = _searchService.SearchContent(searchString);
            var pageSize = 2;

            return View(await posts.ToPagedListAsync(page, pageSize));

        }


        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,PublishState,ImageFile")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;
                
                post.ImageData = (await _fileService.EncodeFileAsync(post.ImageFile)) ??
                                  await _fileService.EncodeFileAsync(_configuration["DefaultPostImage"]);

                post.ContentType = (_fileService.ContentType(post.ImageFile) ??
                                    _configuration["DefaultPostImage"].Split('.')[1]);

                // Need Slug (built from the title)
                var slug = _slugService.UrlFriendly(post.Title);
                if (!_slugService.IsUnique(slug))
                {
                    // Custom Error
                    // Model error and inform the user
                    ModelState.AddModelError("Title","Please provide a unique title");
                    // ModelState.AddModelError("", "Error: Title not unique");

                    return View(post);
                }
                
                post.Slug = slug;


                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("BlogPostIndex", new { id = post.BlogId});
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImageData,ContentType,BlogId,Created,Slug,Title,Abstract,Content,ImageFile,PublishState")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Slug
                    var newSlug = _slugService.UrlFriendly(post.Title);
                    if (post.Slug != newSlug)
                    {
                        if (!_slugService.IsUnique(newSlug))
                        {
                            ModelState.AddModelError("Title", "Please provide a unique title");
                            return View(post);
                        }

                        post.Slug = newSlug;
                    }

                    // Image
                    if (post.ImageFile is not null)
                    {
                        post.ImageData = await _fileService.EncodeFileAsync(post.ImageFile);
                        post.ContentType = _fileService.ContentType(post.ImageFile);
                    }

                    // Updated
                    post.Updated = DateTime.Now;


                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("BlogPostIndex", "Posts", new { id = post.BlogId });
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
