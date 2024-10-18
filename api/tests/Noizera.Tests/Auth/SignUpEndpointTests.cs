using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Noizera.Shared.Contracts.Errors;
using static Noizera.Api.Endpoints.Auth.SignUpPublicEndpoint;

namespace Noizera.Tests.Auth;

public sealed class SignUpEndpointTests(CustomWebApplicationFactory factory) : BaseTest(factory)
{
    private const string url = "/api/auth/sign-up";

    [Theory]
    [InlineData("test@test.com")]
    [InlineData("test1@gmail.eu")]
    public async Task Post_ReturnsOkEmptyResponse_WhenValidEmail(string email)
    {
        var request = new Request(email);

        var response = await _httpClient.PostAsync(url, TestHelper.ToJsonRequest(request));
        var message = await response.Content.ReadAsStringAsync();

        var entity = await _db.VerificationCodes.FirstOrDefaultAsync(x => x.Key == email);

        response.IsSuccessStatusCode.Should().BeTrue();
        message.Should().BeEmpty();
        entity.Should().NotBeNull();
    }

    [Theory]
    [InlineData("testtest.com")]
    public async Task Post_ReturnsBadRequestResponse_Validation_WhenEmailIsInvalid(string email)
    {
        var request = new Request(email);

        var response = await _httpClient.PostAsync(url, TestHelper.ToJsonRequest(request));
        var problemDetails = await TestHelper.ToProblemDetails(response);

        response.StatusCode.Should().Be(response.StatusCode);
        problemDetails.Should().NotBeNull();
        problemDetails?.Title.Should().Be(ErrorType.Validation.ToString());
    }

    [Theory]
    [InlineData("info@noizera")]
    public async Task Post_ReturnsBadRequestResponse_BusinessRule_WhenEmailIsInvalid(string email)
    {
        var request = new Request(email);

        var response = await _httpClient.PostAsync(url, TestHelper.ToJsonRequest(request));
        var problemDetails = await TestHelper.ToProblemDetails(response);

        response.StatusCode.Should().Be(response.StatusCode);
        problemDetails.Should().NotBeNull();
        problemDetails?.Title.Should().Be(ErrorType.BusinessRule.ToString());
    }
}
