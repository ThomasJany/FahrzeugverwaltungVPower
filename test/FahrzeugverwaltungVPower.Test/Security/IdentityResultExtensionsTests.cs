using FahrzeugverwaltungVPower.Core.Security;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Security;

[TestClass]
public class IdentityResultExtensionsTests
{
    [TestMethod]
    public void ThrowIfFailed_WithSuccessfulResult_DoesNotThrow()
    {
        Should.NotThrow(() => IdentityResult.Success.ThrowIfFailed());
    }

    [TestMethod]
    public void ThrowIfFailed_WithPasswordError_ThrowsWithPasswordProperty()
    {
        IdentityResult result = IdentityResult.Failed(
            new IdentityError { Code = "PasswordTooShort", Description = "Passwords must be at least 12 characters." },
            new IdentityError { Code = "PasswordRequiresDigit", Description = "Passwords must have at least one digit." });

        ValidationException exception = Should.Throw<ValidationException>(() => result.ThrowIfFailed());

        exception.Errors.Count().ShouldBe(2);
        exception.Errors.ShouldAllBe(e => e.PropertyName == "Password");
        exception.Errors.Select(e => e.ErrorCode).ShouldBe(["PasswordTooShort", "PasswordRequiresDigit"]);
    }

    [TestMethod]
    public void ThrowIfFailed_WithDuplicateEmailError_ThrowsWithEmailProperty()
    {
        IdentityResult result = IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail", Description = "Email is already taken." });

        ValidationException exception = Should.Throw<ValidationException>(() => result.ThrowIfFailed());

        exception.Errors.Single().PropertyName.ShouldBe("Email");
    }

    [TestMethod]
    public void ThrowIfFailed_WithOtherError_ThrowsWithoutProperty()
    {
        IdentityResult result = IdentityResult.Failed(new IdentityError { Code = "ConcurrencyFailure", Description = "Optimistic concurrency failure." });

        ValidationException exception = Should.Throw<ValidationException>(() => result.ThrowIfFailed());

        exception.Errors.Single().PropertyName.ShouldBeEmpty();
        exception.Errors.Single().ErrorMessage.ShouldBe("Optimistic concurrency failure.");
    }
}
