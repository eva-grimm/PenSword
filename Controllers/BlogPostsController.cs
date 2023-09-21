using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PenSword.Helpers;
using PenSword.Models;
using PenSword.Services.Interfaces;
using PenSword.Enums;
using X.PagedList;
using System.Text;

namespace PenSword.Controllers
{
    [Authorize]
    public class BlogPostsController : Controller
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly IBlogService _blogService;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public BlogPostsController(UserManager<BlogUser> userManager,
            IBlogService blogService,
            IUserService userService,
            IImageService imageService,
            ICategoryService categoryService,
            ITagService tagService)
        {
            _userManager = userManager;
            _blogService = blogService;
            _userService = userService;
            _imageService = imageService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        // GET: BlogPosts
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? pageNum, string? swalMessage = null)
        {
            ViewData["SwalMessage"] = swalMessage;

            int pageSize = 3;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetPublishedBlogPostsAsync())
                .ToPagedListAsync(page, pageSize);
            ViewData["ActionName"] = nameof(Index);

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
            return View(nameof(Index), blogPosts);
        }

        // GET: BlogPosts/LikedBlogs
        public async Task<IActionResult> LikedBlogs(int? pageNum)
        {
            string? blogUserId = _userManager.GetUserId(User);

            int pageSize = 3;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetLikedBlogPostsAsync(blogUserId))
                .ToPagedListAsync(page, pageSize);
            ViewData["ActionName"] = nameof(LikedBlogs);

            return View(nameof(Index), blogPosts);
        }

        // GET: Filter BlogPosts by Category
        [AllowAnonymous]
        public async Task<IActionResult> FilterByCategory(int? id, int? pageNum = null)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetBlogPostsByCategoryAsync(id))
                .ToPagedListAsync(page, pageSize);
            ViewData["ActionName"] = nameof(FilterByCategory);
            ViewData["SearchString"] = (await _categoryService.GetSingleCategoryAsync(id))?.Name;
            return View(nameof(Index), blogPosts);
        }

        // GET: Filter BlogPosts by Tag
        [AllowAnonymous]
        public async Task<IActionResult> FilterByTag(int? id, int? pageNum = null)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetBlogPostsByTagAsync(id))
                .ToPagedListAsync(page, pageSize);
            ViewData["ActionName"] = nameof(FilterByTag);
            ViewData["SearchString"] = (await _tagService.GetSingleTagAsync(id))?.Name;
            return View(nameof(Index), blogPosts);
        }

        // GET: Search BlogPosts
        [AllowAnonymous]
        public async Task<IActionResult> SearchIndex(string? searchString, int? pageNum)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;
            IPagedList<BlogPost> blogPosts = await _blogService.SearchBlogPosts(searchString)
                .ToPagedListAsync(page, pageSize);

            ViewData["ActionName"] = nameof(SearchIndex);
            ViewData["SearchString"] = searchString;

            return View(nameof(Index), blogPosts);
        }

        // GET: BlogPosts/Details/[slug]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? slug, string? swalMessage = null)
        {
            if (string.IsNullOrEmpty(slug)) return NotFound();
            ViewBag.SwalMessage = swalMessage;

            BlogPost? blogPost = await _blogService.GetSingleBlogPostAsync(slug);

            if (blogPost == null) return NotFound();

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> Create()
        {
            List<Category> categories = (await _categoryService.GetCategoriesAsync()).ToList();
            ViewData["CategoryList"] = new SelectList(categories, "Id", "Name");
            List<Tag> tags = (await _blogService.GetTagsAsync()).ToList();
            ViewData["TagList"] = new MultiSelectList(tags, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = nameof(Policies.AdAuth))]
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
                    ViewData["CategoryId"] = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name");
                    return View(blogPost);
                }
                blogPost.Slug = newSlug;
                blogPost.Created = DateTime.Now;

                blogPost.AuthorId = _userManager.GetUserId(User);

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
            List<Category> categories = (await _categoryService.GetCategoriesAsync()).ToList();
            ViewData["CategoryList"] = new SelectList(categories, "Id", "Name");
            List<Tag> tags = (await _blogService.GetTagsAsync()).ToList();
            ViewData["TagList"] = new MultiSelectList(tags, "Id", "Name");

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            BlogPost? blogPost = await _blogService.GetSingleBlogPostAsync(id); 
            if (blogPost is null) return NotFound();
            if (blogPost.Author != await _userManager.GetUserAsync(User)) return Unauthorized();

            List<Category> categories = (await _categoryService.GetCategoriesAsync()).ToList();
            ViewData["CategoryList"] = new SelectList(categories, "Id", "Name");
            List<Tag> tags = (await _blogService.GetTagsAsync()).ToList();
            ViewData["TagList"] = new MultiSelectList(tags, "Id", "Name");

            StringBuilder tagsAsString = new();
            foreach (Tag tag in tags)
            {
                tagsAsString.Append(tag.ToString());
                tagsAsString.Append(", ");
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsPublished,IsDeleted,ImageFile,ImageData,ImageType,CategoryId,AuthorId")] BlogPost blogPost, string? stringTags)
        {
            if (id != blogPost.Id) return NotFound();
            BlogUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || blogPost.AuthorId != currentUser.Id) return Unauthorized();

            if (ModelState.IsValid)
            {
                try
                {
                    // validate slug before continuing
                    string? newSlug = StringHelper.BlogPostSlug(blogPost.Title);
                    if (!await _blogService.ValidSlugAsync(newSlug, blogPost.Id))
                    {
                        ModelState.AddModelError("Title", "A similar Title/Slug is already in use.");
                        ViewData["CategoryId"] = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name");
                        return View(blogPost);
                    }
                    blogPost.Slug = newSlug;
                    blogPost.Updated = DateTime.Now;

                    if (blogPost.ImageFile != null)
                    {
                        // Convert file to byte array and assign it to image data
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                        // Assign ImageType based on the chosen file
                        blogPost.ImageType = blogPost.ImageFile.ContentType;
                    }

                    await _blogService.UpdateBlogPostAsync(blogPost);

                    if (string.IsNullOrEmpty(stringTags) == false)
                    {
                        IEnumerable<string> tagNames = stringTags.Split(',');
                        await _blogService.AddTagsToBlogPostAsync(tagNames, blogPost.Id);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_blogService.BlogPostExists(blogPost.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            List<Category> categories = (await _categoryService.GetCategoriesAsync()).ToList();
            ViewData["CategoryList"] = new SelectList(categories, "Id", "Name");
            List<Tag> tags = (await _blogService.GetTagsAsync()).ToList();
            ViewData["TagList"] = new MultiSelectList(tags, "Id", "Name");

            return View(blogPost);
        }

        [HttpPost,Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> TogglePublish(int? id)
        {
            if (id == null || id == 0) return NotFound();

            BlogPost? blogPost = await _blogService.GetSingleBlogPostAsync(id);
            if (blogPost == null) return NotFound();

            BlogUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || blogPost.AuthorId != currentUser.Id) return Unauthorized();

            blogPost.IsPublished = !blogPost.IsPublished;

            await _blogService.UpdateBlogPostAsync(blogPost);

            return RedirectToAction(nameof(Details), new { slug = blogPost.Slug });
        }
        
        [HttpPost,Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> ToggleDelete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            BlogPost? blogPost = await _blogService.GetSingleBlogPostAsync(id);
            if (blogPost == null) return NotFound();

            BlogUser? currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || blogPost.AuthorId != currentUser.Id) return Unauthorized();

            blogPost.IsDeleted = !blogPost.IsDeleted;
            // additionally, if a blog should no longer be published
            blogPost.IsPublished = false;

            await _blogService.UpdateBlogPostAsync(blogPost);

            return RedirectToAction(nameof(AuthorArea));
        }

        // POST: Handle user clicking like button
        [HttpPost]
        public async Task<IActionResult> LikeBlogPost(int? blogPostId, string? blogUserId)
        {
            BlogPost? blogPost = await _blogService.GetSingleBlogPostAsync(blogPostId);
            BlogUser? blogUser = await _userService.GetUserByIdAsync(blogUserId);
            if (blogUser == null || blogPost == null) return NotFound();

            await _blogService.UserClickedLikeButtonAsync(blogPostId!.Value, blogUserId!);

            return Json(new
            {
                isLiked = await _userService.DoesUserLikeBlogAsync(blogPostId.Value, blogUserId!),
                count = blogPost.Likes.Count
            });
        }

        [Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> AuthorArea(int? pageNum)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;
            BlogUser? author = await _userManager.GetUserAsync(User);
            if (author == null) return Unauthorized();
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetAllBlogPostsAsync(author.Id))
                .ToPagedListAsync(page, pageSize);
            return View(blogPosts);
        }

        [Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> AuthorAreaPublishedOnly(int? pageNum)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;
            BlogUser? author = await _userManager.GetUserAsync(User);
            if (author == null) return Unauthorized();
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetPublishedBlogPostsAsync(author.Id))
                .ToPagedListAsync(page, pageSize);
            return View(nameof(AuthorArea), blogPosts);
        }

        [Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> AuthorAreaDraftOnly(int? pageNum)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;
            BlogUser? author = await _userManager.GetUserAsync(User);
            if (author == null) return Unauthorized();
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetDraftBlogPostsAsync(author.Id))
                .ToPagedListAsync(page, pageSize);
            return View(nameof(AuthorArea), blogPosts);
        }

        [Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> AuthorAreaDeletedOnly(int? pageNum)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;
            BlogUser? author = await _userManager.GetUserAsync(User);
            if (author == null) return Unauthorized();
            IPagedList<BlogPost> blogPosts = await (await _blogService.GetDeletedBlogPostsAsync(author.Id))
                .ToPagedListAsync(page, pageSize);
            return View(nameof(AuthorArea), blogPosts);
        }
    }
}
