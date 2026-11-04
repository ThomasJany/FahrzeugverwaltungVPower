using FahrzeugverwaltungVPower.Core.Commands.Users;
using FahrzeugverwaltungVPower.Core.Commands.Users.Handlers;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using FahrzeugverwaltungVPower.Test.Helpers;
using FluentValidation;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Users.Handlers;

[TestClass]
public class CreateUserCommandHandlerTests
{
    // ---------- Positivfall ----------

    [TestMethod]
    public async Task Handle_WithValidCommand_CreatesUserWithRolesAndReturnsId()
    {
        using TestIdentity identity = new();
        CreateUserCommandHandler handler = new(identity.UserManager);
        CreateUserCommand command = UserTestData.CreateCommand() with
        {
            Email = $" {UserTestData.NewEmail} ",
            FirstName = " Maximilian ",
            LastName = " Musterfrau ",
            // Die doppelte Rolle muss ohne Fehler zusammengefasst werden.
            Roles = [RoleNames.Editor, RoleNames.Creator, RoleNames.Creator]
        };

        Guid id = await handler.Handle(command, CancellationToken.None);

        id.ShouldNotBe(Guid.Empty);
        ApplicationUser? saved = await identity.UserManager.FindByIdAsync(id.ToString());
        saved.ShouldNotBeNull();
        saved.Email.ShouldBe(UserTestData.NewEmail);
        saved.UserName.ShouldBe(UserTestData.NewEmail);
        saved.FirstName.ShouldBe("Maximilian");
        saved.LastName.ShouldBe("Musterfrau");
        saved.EmailConfirmed.ShouldBeTrue();
        (await identity.UserManager.GetRolesAsync(saved)).ShouldBe([RoleNames.Creator, RoleNames.Editor], ignoreOrder: true);

        // Das Passwort gilt, ist aber nicht im Klartext gespeichert.
        (await identity.UserManager.CheckPasswordAsync(saved, UserTestData.NewPassword)).ShouldBeTrue();
        saved.PasswordHash.ShouldNotBeNullOrEmpty();
        saved.PasswordHash.ShouldNotBe(UserTestData.NewPassword);
    }

    // ---------- Negativfälle ----------

    [TestMethod]
    public async Task Handle_WithWeakPassword_ThrowsWithPasswordErrorAndCreatesNoUser()
    {
        using TestIdentity identity = new();
        CreateUserCommandHandler handler = new(identity.UserManager);

        ValidationException exception = await Should.ThrowAsync<ValidationException>(
            () => handler.Handle(UserTestData.CreateCommand() with { Password = "abc" }, CancellationToken.None));

        exception.Errors.ShouldContain(e => e.PropertyName == "Password");
        (await identity.UserManager.FindByEmailAsync(UserTestData.NewEmail)).ShouldBeNull();
    }

    [TestMethod]
    public async Task Handle_WithExistingEmail_ThrowsWithEmailError()
    {
        using TestIdentity identity = new();
        await identity.CreateUserAsync([RoleNames.Viewer], UserTestData.NewEmail);
        CreateUserCommandHandler handler = new(identity.UserManager);

        ValidationException exception = await Should.ThrowAsync<ValidationException>(
            () => handler.Handle(UserTestData.CreateCommand(), CancellationToken.None));

        exception.Errors.ShouldContain(e => e.PropertyName == "Email");
    }
}
