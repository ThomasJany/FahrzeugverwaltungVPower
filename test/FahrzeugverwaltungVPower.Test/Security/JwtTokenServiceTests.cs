using System.Security.Claims;
using System.Text;
using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using FahrzeugverwaltungVPower.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Security;

[TestClass]
public class JwtTokenServiceTests
{
    private const string Key = "unit-test-signing-key-with-at-least-32-characters!";
    private const string OtherKey = "a-completely-different-key-with-32-characters!!";
    private const string Issuer = "TestIssuer";
    private const string Audience = "TestAudience";
    private const int ExpiresMinutes = 60;

    // ---------- Positivfall ----------

    [TestMethod]
    public async Task CreateToken_WithUserAndRoles_ReturnsValidTokenWithClaims()
    {
        DateTimeOffset now = CurrentTimeInWholeSeconds();
        JwtTokenService service = CreateService(now);
        ApplicationUser user = CreateUser();

        AccessToken token = service.CreateToken(user, [RoleNames.Admin, RoleNames.Viewer]);

        token.ExpiresAt.ShouldBe(now.AddMinutes(ExpiresMinutes));
        new JsonWebToken(token.Token).ValidTo.ShouldBe(token.ExpiresAt.UtcDateTime, TimeSpan.FromSeconds(1));

        TokenValidationResult result = await ValidateAsync(token.Token, Key);

        result.IsValid.ShouldBeTrue();
        result.ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value.ShouldBe(user.Id.ToString());
        result.ClaimsIdentity.FindFirst(ClaimTypes.Email)?.Value.ShouldBe(user.Email);
        result.ClaimsIdentity.FindAll(ClaimTypes.Role).Select(c => c.Value)
              .ShouldBe([RoleNames.Admin, RoleNames.Viewer], ignoreOrder: true);
    }

    // ---------- Negativfälle ----------

    [TestMethod]
    public async Task CreateToken_ValidatedWithOtherKey_IsInvalid()
    {
        JwtTokenService service = CreateService(CurrentTimeInWholeSeconds());

        AccessToken token = service.CreateToken(CreateUser(), [RoleNames.Viewer]);

        TokenValidationResult result = await ValidateAsync(token.Token, OtherKey);

        result.IsValid.ShouldBeFalse();
    }

    [TestMethod]
    public async Task CreateToken_ValidatedAfterExpiry_IsInvalid()
    {
        // Ausgestellt vor zwei Stunden, bei einer Gültigkeit von 60 Minuten also bereits abgelaufen.
        JwtTokenService service = CreateService(CurrentTimeInWholeSeconds().AddHours(-2));

        AccessToken token = service.CreateToken(CreateUser(), [RoleNames.Viewer]);

        TokenValidationResult result = await ValidateAsync(token.Token, Key);

        result.IsValid.ShouldBeFalse();
        result.Exception.ShouldBeAssignableTo<SecurityTokenInvalidLifetimeException>();
    }

    // ---------- Hilfsmethoden ----------

    private static DateTimeOffset CurrentTimeInWholeSeconds() =>
        DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

    private static JwtTokenService CreateService(DateTimeOffset now)
    {
        TimeProvider timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(now);

        IOptions<JwtOptions> options = Options.Create(new JwtOptions
        {
            Issuer = Issuer,
            Audience = Audience,
            Key = Key,
            ExpiresMinutes = ExpiresMinutes
        });

        return new JwtTokenService(options, timeProvider);
    }

    private static ApplicationUser CreateUser() => new()
    {
        Id = Guid.NewGuid(),
        UserName = "max@mustermann.de",
        Email = "max@mustermann.de",
        FirstName = "Max",
        LastName = "Mustermann"
    };

    private static async Task<TokenValidationResult> ValidateAsync(string token, string key) =>
        await new JsonWebTokenHandler().ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        });
}
