using FluentAssertions;
using Noizera.Application.CQRS.Profiles.GetArtists;

namespace Noizera.Tests.Profiles;

public sealed class GetArtistsEndpointTests(CustomWebApplicationFactory factory) : BaseTest(factory)
{
    private const string url = "/api/profiles/artists";

    [Theory]
    [InlineData("test")]
    public async Task Post_ReturnsOkFilledResponse_WhenValidText(string text)
    {
        var response = await _httpClient.GetAsync($"{url}?text={text}&userId={LoggedArtist.Id}");
        var result = await TestHelper.To<GetArtistsResponse>(response);

        response.IsSuccessStatusCode.Should().BeTrue();
        result.Should().NotBeNull();
        result!.Artists.Should().Contain(x => x.Name == TestArtist!.Profile!.PublicId);
    }
}
