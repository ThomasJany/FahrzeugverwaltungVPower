using FahrzeugverwaltungVPower.Core.Commands.Users;
using FahrzeugverwaltungVPower.Core.Commands.Users.Validators;
using FahrzeugverwaltungVPower.Core.Security;
using FahrzeugverwaltungVPower.Test.Helpers;
using FluentValidation.Results;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Users.Validators;

/// <remarks>
/// Der Create-Validator erbt die gemeinsamen Regeln von <see cref="UserCommandValidatorBase{T}"/>.
/// Diese werden hier über den Create-Validator geprüft und gelten genauso für das Ändern.
/// </remarks>
[TestClass]
public class CreateUserCommandValidatorTests
{
    // ---------- Positivfall ----------

    [TestMethod]
    public async Task Validate_WithValidCommand_IsValid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand());

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    // ---------- Nur beim Anlegen: E-Mail eindeutig, Passwort ----------

    [TestMethod]
    public async Task Validate_EmailAlreadyExists_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(
            UserTestData.CreateCommand() with { Email = TestIdentity.Email },
            existingUserEmail: TestIdentity.Email);

        ShouldHaveError(result, nameof(CreateUserCommand.Email), ValidationErrorCodes.AsyncPredicate);
    }

    [TestMethod]
    public async Task Validate_PasswordEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { Password = "" });

        ShouldHaveError(result, nameof(CreateUserCommand.Password), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_PasswordTooLong_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { Password = new string('a', 129) });

        ShouldHaveError(result, nameof(CreateUserCommand.Password), ValidationErrorCodes.MaxLength);
    }

    // ---------- Gemeinsame Regeln: E-Mail ----------

    [TestMethod]
    public async Task Validate_EmailEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { Email = "" });

        ShouldHaveError(result, nameof(CreateUserCommand.Email), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_EmailTooLong_IsInvalid()
    {
        string email = new string('a', 260) + "@mustermann.de";

        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { Email = email });

        ShouldHaveError(result, nameof(CreateUserCommand.Email), ValidationErrorCodes.MaxLength);
    }

    [TestMethod]
    public async Task Validate_EmailWithoutValidFormat_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { Email = "keine-email" });

        ShouldHaveError(result, nameof(CreateUserCommand.Email), ValidationErrorCodes.EmailAddress);
    }

    [TestMethod]
    [DataRow("name@host")]
    [DataRow("name@host.")]
    [DataRow("name@host.d")]
    [DataRow("name@.de")]
    [DataRow("na me@beispiel.de")]
    public async Task Validate_EmailWithImplausibleDomain_IsInvalid(string email)
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { Email = email });

        ShouldHaveError(result, nameof(CreateUserCommand.Email), ValidationErrorCodes.EmailAddress);
    }

    // ---------- Gemeinsame Regeln: Namen ----------

    [TestMethod]
    public async Task Validate_FirstNameEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { FirstName = "" });

        ShouldHaveError(result, nameof(CreateUserCommand.FirstName), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_FirstNameTooLong_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { FirstName = new string('A', 101) });

        ShouldHaveError(result, nameof(CreateUserCommand.FirstName), ValidationErrorCodes.MaxLength);
    }

    [TestMethod]
    public async Task Validate_LastNameEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { LastName = "" });

        ShouldHaveError(result, nameof(CreateUserCommand.LastName), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_LastNameTooLong_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { LastName = new string('A', 101) });

        ShouldHaveError(result, nameof(CreateUserCommand.LastName), ValidationErrorCodes.MaxLength);
    }

    // ---------- Gemeinsame Regeln: Rollen ----------

    [TestMethod]
    public async Task Validate_RolesEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { Roles = [] });

        ShouldHaveError(result, nameof(CreateUserCommand.Roles), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_RoleUnknown_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(UserTestData.CreateCommand() with { Roles = [RoleNames.Viewer, "Boss"] });

        // Bei Listen steht die Position des fehlerhaften Eintrags im Feldnamen.
        ShouldHaveError(result, $"{nameof(CreateUserCommand.Roles)}[1]", ValidationErrorCodes.Predicate);
        result.Errors.ShouldNotContain(e => e.PropertyName == $"{nameof(CreateUserCommand.Roles)}[0]");
    }

    // ---------- Hilfsmethoden ----------

    private static async Task<ValidationResult> ValidateAsync(CreateUserCommand command, string? existingUserEmail = null)
    {
        using TestIdentity identity = new();

        if (existingUserEmail is not null)
        {
            await identity.CreateUserAsync([RoleNames.Viewer], existingUserEmail);
        }

        return await new CreateUserCommandValidator(identity.UserManager).ValidateAsync(command);
    }

    private static void ShouldHaveError(ValidationResult result, string propertyName, string errorCode)
    {
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == propertyName && e.ErrorCode == errorCode);
    }
}
