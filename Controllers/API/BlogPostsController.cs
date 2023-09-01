using Microsoft.AspNetCore.Mvc;
using PenSword.Models;
using PenSword.Services.Interfaces;

namespace PenSword.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogPostsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // GET: api/BlogPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {
            IEnumerable<BlogPost> blogPosts = (await _blogService.GetBlogPostsAsync()).Take(4);

            return Ok(blogPosts);
        }
    }
}
