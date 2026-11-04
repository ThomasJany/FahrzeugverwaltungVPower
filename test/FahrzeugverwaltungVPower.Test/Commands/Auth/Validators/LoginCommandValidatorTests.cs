using FahrzeugverwaltungVPower.Core.Commands.Auth;
using FahrzeugverwaltungVPower.Core.Commands.Auth.Validators;
using FahrzeugverwaltungVPower.Test.Helpers;
using FluentValidation.Results;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Auth.Validators;

[TestClass]
public class LoginCommandValidatorTests
{
    // ---------- Positivfall ----------

    [TestMethod]
    public async Task Validate_WithValidCommand_IsValid()
    {
        ValidationResult result = await ValidateAsync(new LoginCommand(TestIdentity.Email, TestIdentity.Password));

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    // ---------- E-Mail ----------

    [TestMethod]
    public async Task Validate_EmailEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(new LoginCommand("", TestIdentity.Password));

        ShouldHaveError(result, nameof(LoginCommand.Email), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_EmailTooLong_IsInvalid()
    {
        string email = new string('a', 260) + "@mustermann.de";

        ValidationResult result = await ValidateAsync(new LoginCommand(email, TestIdentity.Password));

        ShouldHaveError(result, nameof(LoginCommand.Email), ValidationErrorCodes.MaxLength);
    }

    [TestMethod]
    public async Task Validate_EmailWithoutValidFormat_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(new LoginCommand("keine-email", TestIdentity.Password));

        ShouldHaveError(result, nameof(LoginCommand.Email), ValidationErrorCodes.EmailAddress);
    }

    // ---------- Passwort ----------

    [TestMethod]
    public async Task Validate_PasswordEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(new LoginCommand(TestIdentity.Email, ""));

        ShouldHaveError(result, nameof(LoginCommand.Password), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_PasswordTooLong_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(new LoginCommand(TestIdentity.Email, new string('a', 129)));

        ShouldHaveError(result, nameof(LoginCommand.Password), ValidationErrorCodes.MaxLength);
    }

    // ---------- Hilfsmethoden ----------

    private static async Task<ValidationResult> ValidateAsync(LoginCommand command) =>
        await new LoginCommandValidator().ValidateAsync(command);

    private static void ShouldHaveError(ValidationResult result, string propertyName, string errorCode)
    {
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == propertyName && e.ErrorCode == errorCode);
    }
}
