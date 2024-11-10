using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Domain.Users;
using Noizera.Shared.Persistence.SQL;
using System.Net.Http.Headers;

namespace Noizera.Tests;
public abstract class BaseTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient _httpClient = factory.CreateClient();
    protected IServiceProvider _services = null!;
    private AsyncServiceScope _scope;
    protected AppDbContext _db = null!;

    public async Task InitializeAsync()
    {
        _scope = factory.Services.CreateAsyncScope();
        _services = _scope.ServiceProvider;
        _db = _services.GetRequiredService<AppDbContext>();
        await SeedDataAsync();
    }

    public async Task DisposeAsync() => await _scope.DisposeAsync();

    protected User TestArtist { get; private set; } = null!;
    protected User LoggedArtist { get; private set; } = null!;

    private async Task SeedDataAsync()
    {
        var userChecker = _services.GetRequiredService<IUserUniquenessChecker>();
        var profileChecker = _services.GetRequiredService<IProfileUniquenessChecker>();
        var passwordHelper = _services.GetRequiredService<IPasswordHelper>();
        var hashGenerator = _services.GetRequiredService<IHashGenerator>();

        var hasTestArtist = await _db.Users.AnyAsync(x => x.Email == "test-artist@test.com");
        if (!hasTestArtist)
        {
            TestArtist = await User.CreateAsync(
                "test-artist@test.com",
                "testpassword",
                "User",
                "testartist",
                userChecker,
                profileChecker,
                passwordHelper,
                hashGenerator,
                default);
            TestArtist.Profile!.SetProfileType(ProfileType.Artist);
            await _db.Users.AddAsync(TestArtist);
            await _db.SaveChangesAsync();
        }
        else if (TestArtist is null)
        {
            TestArtist = await _db.Users
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(x => x.Email == "test-artist@test.com")!;
        }

        var hasLoggedArtist = await _db.Users.AnyAsync(x => x.Email == "test-artist-logged@test.com");
        if (!hasLoggedArtist)
        {
            LoggedArtist = await User.CreateAsync(
                "test-artist-logged@test.com",
                "testpassword",
                "User",
                "testartistlogged",
                userChecker,
                profileChecker,
                passwordHelper,
                hashGenerator,
                default);
            LoggedArtist.Profile!.SetProfileType(ProfileType.Artist);
            await _db.Users.AddAsync(LoggedArtist);
            await _db.SaveChangesAsync();
        }
        else if (LoggedArtist is null)
        {
            LoggedArtist = await _db.Users
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(x => x.Email == "test-artist-logged@test.com")!;
        }

        var tokenGenerator = _services.GetRequiredService<IJwtTokenService>();
        var accessToken = tokenGenerator.GenerateAccessToken(LoggedArtist!);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}
