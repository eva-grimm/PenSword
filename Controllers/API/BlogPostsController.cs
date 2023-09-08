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

        /// <summary>
        /// This endpoint will return the specified number of mostly recently published blog posts.
        /// The count parameter indicates how many of the recent posts to return.
        /// The maximum number of blog posts allowed per request is 10.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet("{count:int}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts(int count)
        {
            if (count > 10) count = 10;
            IEnumerable<BlogPost> blogPosts = (await _blogService.GetPublishedBlogPostsAsync()).Take(count);

            return Ok(blogPosts);
        }
    }
}
