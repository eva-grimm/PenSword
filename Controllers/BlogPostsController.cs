using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PenSword.Data;
using PenSword.Helpers;
using PenSword.Models;
using PenSword.Services.Interfaces;
using X.PagedList;

namespace PenSword.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IBlogService _blogService;
        private readonly IImageService _imageService;

        public BlogPostsController(ApplicationDbContext context,
            UserManager<BlogUser> userManager,
            IBlogService blogService,
            IImageService imageService)
        {
            _context = context;
            _userManager = userManager;
            _blogService = blogService;
            _imageService = imageService;
        }

        // GET: BlogPosts
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? pageNum, string? swalMessage = null)
        {
            ViewData["SwalMessage"] = swalMessage;

            int pageSize = 3;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetBlogPostsAsync())
                .ToPagedListAsync(page, pageSize);
            ViewData["ActionName"] = nameof(Index);

            // Popular Blog Posts for Sidebar widget
            IEnumerable<BlogPost> popularBlogPosts = await _blogService.GetPopularBlogPostsAsync(pageSize);
            ViewData["PopularBlogs"] = popularBlogPosts;
            return View(blogPosts);
        }

        // GET: Popular BlogPosts
        [AllowAnonymous]
        public async Task<IActionResult> PopularBlogs(int? pageNum)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetPopularBlogPostsAsync())
                .ToPagedListAsync(page, pageSize);
            ViewData["ActionName"] = nameof(PopularBlogs);
            return View(blogPosts);
        }

        // GET: BlogPosts/Favorites
        public async Task<IActionResult> FavoriteBlogs(string? blogUserId, int? pageNum)
        {
            if (string.IsNullOrEmpty(blogUserId)) return View();

            int pageSize = 3;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetFavoriteBlogPostsAsync(blogUserId))
                .ToPagedListAsync(page, pageSize);
            ViewData["ActionName"] = nameof(FavoriteBlogs);

            // Popular Blog Posts for Sidebar widget
            IEnumerable<BlogPost> popularBlogPosts = await _blogService.GetPopularBlogPostsAsync(pageSize);
            ViewData["PopularBlogs"] = popularBlogPosts;

            return View(nameof(Index), blogPosts);
        }

        // GET: Filter BlogPosts by Category
        //public async Task<IActionResult> FilterIndex(string? searchString, int? pageNum)
        //{
        //    int pageSize = 3;
        //    int page = pageNum ?? 1;
        //    IPagedList<BlogPost> blogPosts = await _blogService.SearchBlogPosts(searchString)
        //        .ToPagedListAsync(page, pageSize);
        //    ViewData["ActionName"] = nameof(SearchIndex);
        //    ViewData["SearchString"] = searchString;
        //    return View(nameof(Index), blogPosts);
        //}

        // GET: Search BlogPosts
        public async Task<IActionResult> SearchIndex(string? searchString, int? pageNum)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await _blogService.SearchBlogPosts(searchString)
                .ToPagedListAsync(page, pageSize);

            ViewData["ActionName"] = nameof(SearchIndex);
            ViewData["SearchString"] = searchString;

            // Popular Blog Posts for Sidebar widget
            IEnumerable<BlogPost> popularBlogPosts = await _blogService.GetPopularBlogPostsAsync(pageSize);
            ViewData["PopularBlogs"] = popularBlogPosts;

            return View(nameof(Index), blogPosts);
        }

        // GET: BlogPosts/Details/[slug]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? slug)
        {
            if (string.IsNullOrEmpty(slug)) return NotFound();

            BlogPost? blogPost = await _blogService.GetSingleBlogPostAsync(slug);

            if (blogPost == null) return NotFound();

            return View(blogPost);
        }

        // POST: Handle user clicking like button
        [HttpPost]
        public async Task<IActionResult> LikeBlogPost(int? blogPostId, string? blogUserId)
        {
            BlogUser? blogUser = await _context.Users
                    .FirstOrDefaultAsync(b => b.Id == blogUserId);
            BlogPost? blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(b => b.Id == blogPostId);
            if (blogUser == null || blogPost == null) return NotFound();

            await _blogService.UserClickedLikeButtonAsync(blogPostId!, blogUserId!);

            return Json(new
            {
                isLiked = await _blogService.DoesUserLikeBlogAsync(blogPostId!.Value, blogUserId!),
                count = blogPost.UsersWhoLikeThis.Count()
            });
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            List<Category> categories = _context.Categories.ToList();
            ViewData["CategoryList"] = new SelectList(categories, "Id", "Name");
            List<Tag> tags = _context.Tags.ToList();
            ViewData["TagList"] = new MultiSelectList(tags, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Title,Abstract,Content,IsPublished,ImageFile,CategoryId")] BlogPost blogPost, string? stringTags)
        {
            ModelState.Remove("Slug");
            ModelState.Remove("IsDeleted");

            if (ModelState.IsValid)
            {
                // validate slug before continuing
                string? newSlug = StringHelper.BlogPostSlug(blogPost.Title);
                if (!await _blogService.ValidSlugAsync(newSlug, blogPost.Id))
                {
                    ModelState.AddModelError("Title", "A similar Title/Slug is already in use.");
                    ViewData["CategoryId"] = new SelectList(await _blogService.GetCategoriesAsync(), "Id", "Name");
                    return View(blogPost);
                }
                blogPost.Slug = newSlug;
                blogPost.Created = DateTime.Now;

                if (blogPost.ImageFile != null)
                {
                    // Convert file to byte array and assign it to image data
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                    // Assign ImageType based on the chosen file
                    blogPost.ImageType = blogPost.ImageFile.ContentType;
                }

                await _blogService.AddBlogPostAsync(blogPost);

                if (string.IsNullOrEmpty(stringTags) == false)
                {
                    IEnumerable<string> tagNames = stringTags.Split(',');
                    await _blogService.AddTagsToBlogPostAsync(tagNames, blogPost.Id);
                }

                return RedirectToAction(nameof(Index));
            }
            List<Category> categories = _context.Categories.ToList();
            ViewData["CategoryList"] = new SelectList(categories, "Id", "Name");
            List<Tag> tags = _context.Tags.ToList();
            ViewData["TagList"] = new MultiSelectList(tags, "Id", "Name");
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            BlogPost? blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost is null) return NotFound();

            List<Category> categories = _context.Categories.ToList();
            ViewData["CategoryList"] = new SelectList(categories, "Id", "Name");
            List<Tag> tags = _context.Tags.ToList();
            ViewData["CategoryList"] = new MultiSelectList(tags, "Id", "Name");

            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsPublished,IsDeleted,ImageFile,ImageData,ImageType,CategoryId")] BlogPost blogPost)
        {
            if (id != blogPost.Id) return NotFound();

            // TO-DO: Address slug!
            ModelState.Remove("Slug");

            if (ModelState.IsValid)
            {
                try
                {
                    blogPost.Updated = DateTime.Now;

                    // TO-DO: Address Slug!
                    blogPost.Slug = string.Empty;

                    if (blogPost.ImageFile != null)
                    {
                        // Convert file to byte array and assign it to image data
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                        // Assign ImageType based on the chosen file
                        blogPost.ImageType = blogPost.ImageFile.ContentType;
                    }

                    await _blogService.UpdateBlogPostAsync(blogPost);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);

            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BlogPosts == null) return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");

            await _blogService.DeleteBlogPostAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
