using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Noizera.Infrastructure.Audio;
using Noizera.Infrastructure.CoverImages;
using Noizera.Infrastructure.MusicCollections;
using Noizera.Infrastructure.Profiles;
using Noizera.Infrastructure.Security.Authorization;
using Noizera.Infrastructure.Security.Passwords;
using Noizera.Infrastructure.Security.Policy;
using Noizera.Infrastructure.Security.Tokens;
using Noizera.Infrastructure.Security.UserProviders;
using Noizera.Infrastructure.Security.VerificationCodes;
using Noizera.Infrastructure.Songs;
using Noizera.Infrastructure.Subscriptions;
using Noizera.Infrastructure.Users;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Domain.SecretTokens;
using Noizera.Shared.Domain.Users;
using Noizera.Shared.Domain.VerificationCodes;
using Noizera.Shared.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure;

public static class DI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, [NotNull] WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        _ = services
            .AddHttpContextAccessor()
            .AddSharedInfrastructure(builder)
            .AddServices(configuration)
            .AddAuthentication(configuration)
            .AddAuthorization()
            .AddRepositories(configuration);

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddScoped<IUserUniquenessChecker, UserUniquenessChecker>();
        _ = services.AddScoped<IProfileUniquenessChecker, ProfileUniquenessChecker>();
        _ = services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
        _ = services.AddScoped<IVerificationCodeGenerator, VerificationCodeGenerator>();
        _ = services.AddScoped<IPasswordHelper, PasswordHelper>();

        _ = services.AddScoped<ICoverImageService, CoverImageMongoService>();
        _ = services.AddScoped<ICoverImageUploader, CoverImageUploader>();

        _ = services.AddScoped<ISubscriptionService, SubscriptionService>();

        _ = services.AddScoped<IAudioFileService, AudioFileService>();
        _ = services.AddScoped<IAudioMongoService, AudioMongoService>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddScoped<IUserRepository, UserRepository>();
        _ = services.AddScoped<IProfileRepository, ProfileRepository>();
        _ = services.AddScoped<IProfileRelationRepository, ProfileRelationRepository>();
        _ = services.AddScoped<ISongRepository, SongRepository>();
        _ = services.AddScoped<IAlbumRepository, AlbumRepository>();
        _ = services.AddScoped<IMusicCollectionRepository, MusicCollectionRepository>();
        _ = services.AddScoped<IMusicCollectionCreditRepository, MusicCollectionCreditRepository>();
        _ = services.AddScoped<ISavedMusicCollectionRepository, SavedMusicCollectionRepository>();
        _ = services.AddScoped<IListeningHistoryRepository, ListeningHistoryRepository>();
        _ = services.AddScoped<IStreamInfoRepository, StreamInfoRepository>();
        _ = services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        _ = services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
        _ = services.AddScoped<ITermsRepository, TermsRepository>();
        _ = services.AddScoped<ISecretTokenRepository, SecretTokenRepository>();

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        _ = services.AddScoped<IAuthorizationService, AuthorizationService>();
        _ = services.AddScoped<ICurrentUserProvider, CurrentUserCustomProvider>();
        _ = services.AddSingleton<IPolicyEnforcer, PolicyEnforcer>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
        _ = services.Configure<SecretTokensSettings>(configuration.GetSection(SecretTokensSettings.Section));

        _ = services.AddSingleton<ISecretTokenGenerator, SecretTokenGenerator>();
        _ = services.AddSingleton<IJwtTokenService, JwtTokenService>();

        _ = services
            .ConfigureOptions<JwtBearerTokenValidationConfiguration>()
            .AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        return services;
    }
}
