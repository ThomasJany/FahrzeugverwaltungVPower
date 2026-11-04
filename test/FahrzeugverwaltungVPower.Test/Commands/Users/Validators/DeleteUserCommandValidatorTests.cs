using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Commands.Users;
using FahrzeugverwaltungVPower.Core.Commands.Users.Validators;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using FahrzeugverwaltungVPower.Test.Helpers;
using FluentValidation.Results;
using NSubstitute;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Users.Validators;

[TestClass]
public class DeleteUserCommandValidatorTests
{
    // ---------- Positivfall ----------

    [TestMethod]
    public async Task Validate_WithOtherExistingUser_IsValid()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);

        // Der angemeldete Benutzer ist ein anderer, zum Beispiel ein Administrator.
        ValidationResult result = await ValidateAsync(identity, Guid.NewGuid(), new DeleteUserCommand(user.Id));

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    // ---------- Negativfälle ----------

    [TestMethod]
    public async Task Validate_UserDoesNotExist_IsInvalid()
    {
        using TestIdentity identity = new();
        await identity.CreateUserAsync([RoleNames.Viewer]);

        ValidationResult result = await ValidateAsync(identity, Guid.NewGuid(), new DeleteUserCommand(Guid.NewGuid()));

        ShouldHaveError(result, nameof(DeleteUserCommand.Id), ValidationErrorCodes.NotFound);
        // Wegen CascadeMode.Stop wird bei einem unbekannten Benutzer nur dieser eine Fehler gemeldet.
        result.Errors.Count.ShouldBe(1);
    }

    [TestMethod]
    public async Task Validate_UserIsCurrentUser_IsInvalid()
    {
        using TestIdentity identity = new();
        ApplicationUser admin = await identity.CreateUserAsync([RoleNames.Admin]);

        ValidationResult result = await ValidateAsync(identity, admin.Id, new DeleteUserCommand(admin.Id));

        ShouldHaveError(result, nameof(DeleteUserCommand.Id), ValidationErrorCodes.Predicate);
    }

    // ---------- Hilfsmethoden ----------

    private static async Task<ValidationResult> ValidateAsync(TestIdentity identity, Guid currentUserId, DeleteUserCommand command)
    {
        ICurrentUser currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(currentUserId);

        return await new DeleteUserCommandValidator(identity.UserManager, currentUser).ValidateAsync(command);
    }

    private static void ShouldHaveError(ValidationResult result, string propertyName, string errorCode)
    {
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == propertyName && e.ErrorCode == errorCode);
    }
}
