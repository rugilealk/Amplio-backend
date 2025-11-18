using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using PSI.Data;
using PSI.Models;

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

    //[Fact]
    //public async Task UploadSongs_ValidFile_ReturnsOkAndSeeds()
    //{
    //    var dir = Path.Combine(Directory.GetCurrentDirectory(), "DummyData");
    //    Directory.CreateDirectory(dir);
    //    var filePath = Path.Combine(dir, "songs.json");
    //    var json = "[" +
    //               "{\n" +
    //               "  \"Title\": \"Song One\",\n" +
    //               "  \"Artist\": \"Artist A\",\n" +
    //               "  \"Genres\": [\"Rock\"],\n" +
    //               "  \"Link\": \"https://youtu.be/VIDEO1\",\n" +
    //               "  \"Album\": { \"Name\": \"AlbumX\", \"Artist\": \"Artist A\", \"ReleaseYear\": 2001 }\n" +
    //               "}]";
    //    File.WriteAllText(filePath, json);

    //    var response = await _client.PostAsync("/songs/upload", null);
    //    response.StatusCode.Should().Be(HttpStatusCode.OK);

    //    using var scope = _factory.Services.CreateScope();
    //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //    db.Songs.Count().Should().Be(1);
    //    db.Albums.Count().Should().Be(1);
    //}

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
