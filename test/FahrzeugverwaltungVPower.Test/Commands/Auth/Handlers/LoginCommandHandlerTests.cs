using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Commands.Auth;
using FahrzeugverwaltungVPower.Core.Commands.Auth.Handlers;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using FahrzeugverwaltungVPower.Test.Helpers;
using NSubstitute;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Auth.Handlers;

[TestClass]
public class LoginCommandHandlerTests
{
    private const string WrongPassword = "FalschesPasswort1";
    private static readonly DateTimeOffset TokenExpiresAt = new(2030, 1, 1, 12, 0, 0, TimeSpan.Zero);

    // ---------- Positivfälle ----------

    [TestMethod]
    public async Task Handle_WithValidCredentials_ReturnsTokenAndUserData()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Creator, RoleNames.Editor]);
        IJwtTokenService tokenService = CreateTokenService();
        LoginCommandHandler handler = new(identity.UserManager, tokenService);

        // Groß- und Kleinschreibung sowie Leerzeichen der E-Mail spielen keine Rolle.
        LoginResult? result = await handler.Handle(new LoginCommand(" MAX@Mustermann.DE ", TestIdentity.Password), CancellationToken.None);

        result.ShouldNotBeNull();
        result.Token.ShouldBe("jwt-token");
        result.ExpiresAt.ShouldBe(TokenExpiresAt);
        result.Email.ShouldBe(TestIdentity.Email);
        result.FirstName.ShouldBe("Max");
        result.LastName.ShouldBe("Mustermann");
        result.Roles.ShouldBe([RoleNames.Creator, RoleNames.Editor], ignoreOrder: true);
        tokenService.Received(1).CreateToken(Arg.Is<ApplicationUser>(u => u.Id == user.Id), Arg.Any<IEnumerable<string>>());
    }

    [TestMethod]
    public async Task Handle_WithValidCredentialsAfterFailedAttempts_ResetsFailedAttempts()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        LoginCommandHandler handler = new(identity.UserManager, CreateTokenService());
        await handler.Handle(new LoginCommand(TestIdentity.Email, WrongPassword), CancellationToken.None);
        await handler.Handle(new LoginCommand(TestIdentity.Email, WrongPassword), CancellationToken.None);

        LoginResult? result = await handler.Handle(new LoginCommand(TestIdentity.Email, TestIdentity.Password), CancellationToken.None);

        result.ShouldNotBeNull();
        (await identity.UserManager.GetAccessFailedCountAsync(user)).ShouldBe(0);
    }

    // ---------- Negativfälle ----------

    [TestMethod]
    public async Task Handle_WithWrongPassword_ReturnsNullAndCountsFailedAttempt()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        IJwtTokenService tokenService = CreateTokenService();
        LoginCommandHandler handler = new(identity.UserManager, tokenService);

        LoginResult? result = await handler.Handle(new LoginCommand(TestIdentity.Email, WrongPassword), CancellationToken.None);

        result.ShouldBeNull();
        (await identity.UserManager.GetAccessFailedCountAsync(user)).ShouldBe(1);
        tokenService.DidNotReceive().CreateToken(Arg.Any<ApplicationUser>(), Arg.Any<IEnumerable<string>>());
    }

    [TestMethod]
    public async Task Handle_WithUnknownEmail_ReturnsNull()
    {
        using TestIdentity identity = new();
        await identity.CreateUserAsync([RoleNames.Viewer]);
        IJwtTokenService tokenService = CreateTokenService();
        LoginCommandHandler handler = new(identity.UserManager, tokenService);

        LoginResult? result = await handler.Handle(new LoginCommand("unbekannt@mustermann.de", TestIdentity.Password), CancellationToken.None);

        result.ShouldBeNull();
        tokenService.DidNotReceive().CreateToken(Arg.Any<ApplicationUser>(), Arg.Any<IEnumerable<string>>());
    }

    [TestMethod]
    public async Task Handle_WithLockedOutUser_ReturnsNullEvenWithCorrectPassword()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        await identity.UserManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(5));
        IJwtTokenService tokenService = CreateTokenService();
        LoginCommandHandler handler = new(identity.UserManager, tokenService);

        LoginResult? result = await handler.Handle(new LoginCommand(TestIdentity.Email, TestIdentity.Password), CancellationToken.None);

        result.ShouldBeNull();
        tokenService.DidNotReceive().CreateToken(Arg.Any<ApplicationUser>(), Arg.Any<IEnumerable<string>>());
    }

    [TestMethod]
    public async Task Handle_AfterTooManyFailedAttempts_LocksAccount()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        LoginCommandHandler handler = new(identity.UserManager, CreateTokenService());

        // Fünf Fehlversuche entsprechen MaxFailedAccessAttempts der Identity-Einstellungen.
        for (int attempt = 0; attempt < 5; attempt++)
        {
            await handler.Handle(new LoginCommand(TestIdentity.Email, WrongPassword), CancellationToken.None);
        }

        (await identity.UserManager.IsLockedOutAsync(user)).ShouldBeTrue();

        LoginResult? result = await handler.Handle(new LoginCommand(TestIdentity.Email, TestIdentity.Password), CancellationToken.None);

        result.ShouldBeNull();
    }

    // ---------- Hilfsmethoden ----------

    private static IJwtTokenService CreateTokenService()
    {
        IJwtTokenService tokenService = Substitute.For<IJwtTokenService>();
        tokenService.CreateToken(Arg.Any<ApplicationUser>(), Arg.Any<IEnumerable<string>>())
                    .Returns(new AccessToken("jwt-token", TokenExpiresAt));

        return tokenService;
    }
}
