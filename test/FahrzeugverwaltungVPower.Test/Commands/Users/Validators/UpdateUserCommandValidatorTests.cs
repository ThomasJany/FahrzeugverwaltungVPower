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

/// <remarks>
/// Geprüft werden nur die Regeln, die der Update-Validator selbst ergänzt. Die gemeinsamen Regeln
/// (E-Mail-Format, Namen, Rollen) stehen in <see cref="UserCommandValidatorBase{T}"/>.
/// </remarks>
[TestClass]
public class UpdateUserCommandValidatorTests
{
    // ---------- Positivfälle ----------

    [TestMethod]
    public async Task Validate_WithValidCommand_IsValid()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);

        // Der angemeldete Benutzer ist ein anderer, zum Beispiel ein Administrator.
        ValidationResult result = await ValidateAsync(identity, Guid.NewGuid(), UserTestData.UpdateCommand(user.Id));

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [TestMethod]
    public async Task Validate_EmailUnchangedForSameUser_IsValid()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);

        // Der Benutzer behält seine eigene E-Mail-Adresse, das ist kein Duplikat.
        ValidationResult result = await ValidateAsync(
            identity, Guid.NewGuid(), UserTestData.UpdateCommand(user.Id) with { Email = TestIdentity.Email });

        result.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public async Task Validate_OwnRolesUnchanged_IsValid()
    {
        using TestIdentity identity = new();
        ApplicationUser admin = await identity.CreateUserAsync([RoleNames.Admin]);

        // Der Administrator ändert sich selbst, aber nicht seine Rollen.
        ValidationResult result = await ValidateAsync(
            identity, admin.Id, UserTestData.UpdateCommand(admin.Id) with { Roles = [RoleNames.Admin] });

        result.IsValid.ShouldBeTrue();
    }

    // ---------- Negativfälle ----------

    [TestMethod]
    public async Task Validate_UserDoesNotExist_IsInvalid()
    {
        using TestIdentity identity = new();
        await identity.CreateUserAsync([RoleNames.Viewer]);

        ValidationResult result = await ValidateAsync(identity, Guid.NewGuid(), UserTestData.UpdateCommand(Guid.NewGuid()));

        ShouldHaveError(result, nameof(UpdateUserCommand.Id), ValidationErrorCodes.NotFound);
    }

    [TestMethod]
    public async Task Validate_EmailBelongsToOtherUser_IsInvalid()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);
        await identity.CreateUserAsync([RoleNames.Viewer], TestIdentity.OtherEmail);

        ValidationResult result = await ValidateAsync(
            identity, Guid.NewGuid(), UserTestData.UpdateCommand(user.Id) with { Email = TestIdentity.OtherEmail });

        ShouldHaveError(result, nameof(UpdateUserCommand.Email), ValidationErrorCodes.AsyncPredicate);
    }

    [TestMethod]
    public async Task Validate_ChangingOwnRoles_IsInvalid()
    {
        using TestIdentity identity = new();
        ApplicationUser admin = await identity.CreateUserAsync([RoleNames.Admin]);

        ValidationResult result = await ValidateAsync(
            identity, admin.Id, UserTestData.UpdateCommand(admin.Id) with { Roles = [RoleNames.Viewer] });

        ShouldHaveError(result, nameof(UpdateUserCommand.Roles), ValidationErrorCodes.AsyncPredicate);
    }

    [TestMethod]
    public async Task Validate_NewPasswordEmpty_IsInvalid()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);

        ValidationResult result = await ValidateAsync(
            identity, Guid.NewGuid(), UserTestData.UpdateCommand(user.Id) with { NewPassword = "" });

        ShouldHaveError(result, nameof(UpdateUserCommand.NewPassword), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_NewPasswordTooLong_IsInvalid()
    {
        using TestIdentity identity = new();
        ApplicationUser user = await identity.CreateUserAsync([RoleNames.Viewer]);

        ValidationResult result = await ValidateAsync(
            identity, Guid.NewGuid(), UserTestData.UpdateCommand(user.Id) with { NewPassword = new string('a', 129) });

        ShouldHaveError(result, nameof(UpdateUserCommand.NewPassword), ValidationErrorCodes.MaxLength);
    }

    // ---------- Hilfsmethoden ----------

    private static async Task<ValidationResult> ValidateAsync(TestIdentity identity, Guid currentUserId, UpdateUserCommand command)
    {
        ICurrentUser currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(currentUserId);

        return await new UpdateUserCommandValidator(identity.UserManager, currentUser).ValidateAsync(command);
    }

    private static void ShouldHaveError(ValidationResult result, string propertyName, string errorCode)
    {
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == propertyName && e.ErrorCode == errorCode);
    }
}
