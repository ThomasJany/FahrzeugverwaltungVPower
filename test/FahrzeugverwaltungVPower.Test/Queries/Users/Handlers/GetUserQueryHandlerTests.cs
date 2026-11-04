using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Queries.Users;
using FahrzeugverwaltungVPower.Core.Queries.Users.Handlers;
using FahrzeugverwaltungVPower.Core.Security;
using FahrzeugverwaltungVPower.Test.Helpers;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Queries.Users.Handlers;

[TestClass]
public class GetUserQueryHandlerTests
{
    [TestMethod]
    public async Task Handle_WithExistingId_ReturnsUserWithRoles()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer, RoleNames.Creator]);
        await identity.CreateUserAsync([RoleNames.Admin], TestIdentity.OtherEmail, "Erika", "Musterfrau");
        GetUserQueryHandler handler = new(identity.UserManager);

        UserDto? result = await handler.Handle(new GetUserQuery(user.Id), CancellationToken.None);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(user.Id);
        result.Email.ShouldBe(TestIdentity.Email);
        result.FirstName.ShouldBe("Max");
        result.LastName.ShouldBe("Mustermann");
        result.Roles.ShouldBe([RoleNames.Creator, RoleNames.Viewer]);
    }

    [TestMethod]
    public async Task Handle_WithUnknownId_ReturnsNull()
    {
        using TestIdentity identity = new();
        await identity.CreateUserAsync([RoleNames.Viewer]);
        GetUserQueryHandler handler = new(identity.UserManager);

        UserDto? result = await handler.Handle(new GetUserQuery(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeNull();
    }
}
