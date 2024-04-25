using System.ComponentModel.DataAnnotations;

namespace Blog.Api
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;

    }
}
