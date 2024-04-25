using System.Net;
using System.Net.Http.Json;

namespace Blog.Api.IntegrationTests
{
    [Collection(nameof(SharedTestCollection))]
    public class BlogTests : IAsyncLifetime
    {
        private readonly HttpClient _httpClient = default!;

        private Func<Task> _resetDatabase = default!;

        public BlogTests(CustomWebApplicationFactory customWebApplicationFactory)
        {
            _httpClient = customWebApplicationFactory.HttpClient;
            _resetDatabase = customWebApplicationFactory.ResetDatabaseAsync;
        }


        [Fact]
        public async Task OurTestCase()
        {
            //Arrange
            var blogPost = new BlogPost
            {
                Title = "Test Title",
                Content = "Test Content"
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync(requestUri: "/api/blog", blogPost);
            response.EnsureSuccessStatusCode();

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        public async Task DisposeAsync() => await _resetDatabase();

        public Task InitializeAsync() => Task.CompletedTask;
    }
}
