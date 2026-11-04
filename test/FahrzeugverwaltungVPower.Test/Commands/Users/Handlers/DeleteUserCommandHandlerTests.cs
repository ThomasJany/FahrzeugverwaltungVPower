using FahrzeugverwaltungVPower.Core.Commands.Users;
using FahrzeugverwaltungVPower.Core.Commands.Users.Handlers;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using FahrzeugverwaltungVPower.Test.Helpers;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Users.Handlers;

[TestClass]
public class DeleteUserCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_WithExistingId_DeletesOnlyThatUserAndReturnsTrue()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        ApplicationUser other = await identity.CreateUserAsync([RoleNames.Viewer], TestIdentity.OtherEmail);
        DeleteUserCommandHandler handler = new(identity.UserManager);

        bool deleted = await handler.Handle(new DeleteUserCommand(user.Id), CancellationToken.None);

        deleted.ShouldBeTrue();
        (await identity.UserManager.FindByIdAsync(user.Id.ToString())).ShouldBeNull();
        (await identity.UserManager.FindByIdAsync(other.Id.ToString())).ShouldNotBeNull();
    }

    [TestMethod]
    public async Task Handle_WithUnknownId_ReturnsFalseAndKeepsUsers()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        DeleteUserCommandHandler handler = new(identity.UserManager);

        bool deleted = await handler.Handle(new DeleteUserCommand(Guid.NewGuid()), CancellationToken.None);

        deleted.ShouldBeFalse();
        (await identity.UserManager.FindByIdAsync(user.Id.ToString())).ShouldNotBeNull();
    }
}
