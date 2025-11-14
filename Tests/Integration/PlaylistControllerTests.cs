using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PSI.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace Tests.Integration;

public class PlaylistControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PlaylistControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost")
        });
    }

    private static Guid ExtractIdFromLocation(Uri? location)
    {
        location.Should().NotBeNull();
        var path = location!.IsAbsoluteUri ? location.AbsolutePath : location.ToString();
        var idPart = path.TrimEnd('/').Split('/').Last();
        return Guid.Parse(idPart);
    }

    [Fact]
    public async Task CreatePlaylist_CurrentSongNotSet_ReturnsNoContent()
    {
        var createRequest = new CreatePlaylistRequestDto("MyList", true, null);
        var created = await _client.PostAsJsonAsync("/playlist", createRequest);
        created.StatusCode.Should().Be(HttpStatusCode.Created);
        var id = ExtractIdFromLocation(created.Headers.Location);

        var current = await _client.GetAsync($"/playlist/{id}/current");
        current.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task VisitPlaylist_IncreasesPopularity()
    {
        var createRequest = new CreatePlaylistRequestDto("VisitMe", true, null);
        var created = await _client.PostAsJsonAsync("/playlist", createRequest);
        created.StatusCode.Should().Be(HttpStatusCode.Created);
        var id = ExtractIdFromLocation(created.Headers.Location);

        var visit = await _client.PostAsync($"/playlist/{id}/visit", null);
        visit.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
