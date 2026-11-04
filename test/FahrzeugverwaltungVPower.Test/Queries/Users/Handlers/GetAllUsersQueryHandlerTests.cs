using FahrzeugverwaltungVPower.Core.Queries.Users;
using FahrzeugverwaltungVPower.Core.Queries.Users.Handlers;
using FahrzeugverwaltungVPower.Core.Security;
using FahrzeugverwaltungVPower.Test.Helpers;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Queries.Users.Handlers;

[TestClass]
public class GetAllUsersQueryHandlerTests
{
    [TestMethod]
    public async Task Handle_WithStoredUsers_ReturnsAllSortedByLastAndFirstNameWithRoles()
    {
        using TestIdentity identity = new();
        await identity.CreateUserAsync([RoleNames.Viewer, RoleNames.Creator], "zoe.meier@mustermann.de", "Zoe", "Meier");
        await identity.CreateUserAsync([RoleNames.Admin], "anna.meier@mustermann.de", "Anna", "Meier");
        await identity.CreateUserAsync([RoleNames.Editor], "bernd.adler@mustermann.de", "Bernd", "Adler");
        GetAllUsersQueryHandler handler = new(identity.UserManager);

        IReadOnlyList<UserDto> result = await handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        // Zuerst nach Nachname, bei gleichem Nachnamen nach Vorname.
        result.Select(u => $"{u.LastName} {u.FirstName}").ShouldBe(["Adler Bernd", "Meier Anna", "Meier Zoe"]);
        result[0].Email.ShouldBe("bernd.adler@mustermann.de");
        result[0].Roles.ShouldBe([RoleNames.Editor]);
        result[1].Roles.ShouldBe([RoleNames.Admin]);
        // Die Rollen eines Benutzers sind alphabetisch sortiert.
        result[2].Roles.ShouldBe([RoleNames.Creator, RoleNames.Viewer]);
    }

    [TestMethod]
    public async Task Handle_WithoutUsers_ReturnsEmptyList()
    {
        using TestIdentity identity = new();
        GetAllUsersQueryHandler handler = new(identity.UserManager);

        IReadOnlyList<UserDto> result = await handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }
}
