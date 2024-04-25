using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly ILogger<BlogController> _logger;
        private readonly ApplicationDbContext _db;

        public BlogController(ILogger<BlogController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public async Task<IEnumerable<BlogPost>> Get()
        {
            return await _db.Posts.ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post(BlogPost blogPost)
        {
            _db.Posts.Add(blogPost);
            await _db.SaveChangesAsync();

            return Created("", blogPost);
        }
    }
}
