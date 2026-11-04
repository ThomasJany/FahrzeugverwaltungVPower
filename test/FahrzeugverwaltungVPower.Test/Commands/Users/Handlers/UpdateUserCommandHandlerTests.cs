using FahrzeugverwaltungVPower.Core.Commands.Users;
using FahrzeugverwaltungVPower.Core.Commands.Users.Handlers;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using FahrzeugverwaltungVPower.Test.Helpers;
using FluentValidation;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Users.Handlers;

[TestClass]
public class UpdateUserCommandHandlerTests
{
    // ---------- Positivfälle ----------

    [TestMethod]
    public async Task Handle_WithExistingId_UpdatesDataAndRolesAndReturnsTrue()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer, RoleNames.Creator]);
        UpdateUserCommandHandler handler = new(identity.UserManager);
        UpdateUserCommand command = UserTestData.UpdateCommand(user.Id) with
        {
            Email = $" {UserTestData.NewEmail} ",
            FirstName = " Maximilian ",
            LastName = " Musterfrau ",
            // Die doppelte Rolle muss ohne Fehler zusammengefasst werden.
            Roles = [RoleNames.Editor, RoleNames.Creator, RoleNames.Creator]
        };

        bool updated = await handler.Handle(command, CancellationToken.None);

        updated.ShouldBeTrue();
        ApplicationUser? saved = await identity.UserManager.FindByEmailAsync(UserTestData.NewEmail);
        saved.ShouldNotBeNull();
        saved.Id.ShouldBe(user.Id);
        saved.UserName.ShouldBe(UserTestData.NewEmail);
        saved.FirstName.ShouldBe("Maximilian");
        saved.LastName.ShouldBe("Musterfrau");
        (await identity.UserManager.GetRolesAsync(saved)).ShouldBe([RoleNames.Creator, RoleNames.Editor], ignoreOrder: true);
        (await identity.UserManager.FindByEmailAsync(TestIdentity.Email)).ShouldBeNull();
        (await identity.UserManager.CheckPasswordAsync(saved, TestIdentity.Password)).ShouldBeTrue();
    }

    [TestMethod]
    public async Task Handle_WithNewPassword_ChangesPasswordAndSecurityStamp()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        string? securityStampBefore = user.SecurityStamp;
        UpdateUserCommandHandler handler = new(identity.UserManager);

        bool updated = await handler.Handle(
            UserTestData.UpdateCommand(user.Id) with { NewPassword = UserTestData.NewPassword },
            CancellationToken.None);

        updated.ShouldBeTrue();
        (await identity.UserManager.CheckPasswordAsync(user, UserTestData.NewPassword)).ShouldBeTrue();
        (await identity.UserManager.CheckPasswordAsync(user, TestIdentity.Password)).ShouldBeFalse();
        user.SecurityStamp.ShouldNotBe(securityStampBefore);
    }

    // ---------- Negativfälle ----------

    [TestMethod]
    public async Task Handle_WithUnknownId_ReturnsFalseAndChangesNothing()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        UpdateUserCommandHandler handler = new(identity.UserManager);

        bool updated = await handler.Handle(UserTestData.UpdateCommand(Guid.NewGuid()), CancellationToken.None);

        updated.ShouldBeFalse();
        ApplicationUser? unchanged = await identity.UserManager.FindByIdAsync(user.Id.ToString());
        unchanged.ShouldNotBeNull();
        unchanged.FirstName.ShouldBe("Max");
        unchanged.Email.ShouldBe(TestIdentity.Email);
    }

    [TestMethod]
    public async Task Handle_WithWeakNewPassword_ThrowsAndChangesNothing()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        UpdateUserCommandHandler handler = new(identity.UserManager);

        ValidationException exception = await Should.ThrowAsync<ValidationException>(
            () => handler.Handle(UserTestData.UpdateCommand(user.Id) with { NewPassword = "abc" }, CancellationToken.None));

        exception.Errors.ShouldContain(e => e.PropertyName == "Password");

        // Das Passwort wird vor allen anderen Änderungen geprüft, der Benutzer bleibt unverändert.
        ApplicationUser? unchanged = await identity.UserManager.FindByIdAsync(user.Id.ToString());
        unchanged.ShouldNotBeNull();
        unchanged.FirstName.ShouldBe("Max");
        unchanged.Email.ShouldBe(TestIdentity.Email);
        (await identity.UserManager.CheckPasswordAsync(unchanged, TestIdentity.Password)).ShouldBeTrue();
        (await identity.UserManager.GetRolesAsync(unchanged)).ShouldBe([RoleNames.Viewer]);
    }

    [TestMethod]
    public async Task Handle_WithEmailOfOtherUser_ThrowsWithEmailError()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        await identity.CreateUserAsync([RoleNames.Viewer], TestIdentity.OtherEmail);
        UpdateUserCommandHandler handler = new(identity.UserManager);

        ValidationException exception = await Should.ThrowAsync<ValidationException>(
            () => handler.Handle(UserTestData.UpdateCommand(user.Id) with { Email = TestIdentity.OtherEmail }, CancellationToken.None));

        exception.Errors.ShouldContain(e => e.PropertyName == "Email");
    }
}
