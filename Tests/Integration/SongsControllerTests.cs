using FluentAssertions;
using System.Net;


namespace Tests.Integration;

public class SongsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public SongsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllSongs_EmptyInitially_Ok()
    {
        var response = await _client.GetAsync("/songs");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSongById_NotFound()
    {
        var response = await _client.GetAsync($"/songs/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UploadSongs_FileMissing_ReturnsNotFound()
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "DummyData");
        var filePath = Path.Combine(dir, "songs.json");
        if (File.Exists(filePath)) File.Delete(filePath);

        var response = await _client.PostAsync("/songs/upload", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
