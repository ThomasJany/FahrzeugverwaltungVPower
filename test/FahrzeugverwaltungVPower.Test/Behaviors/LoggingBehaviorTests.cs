using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Behaviors;
using FahrzeugverwaltungVPower.Core.Commands.Auth;
using FahrzeugverwaltungVPower.Test.Helpers;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Behaviors;

[TestClass]
public class LoggingBehaviorTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    public sealed record CreateThingCommand : IRequest<Guid>;

    public sealed record DeleteThingCommand(Guid Id) : IRequest<bool>;

    public sealed record GetThingQuery(Guid Id) : IRequest<string?>;

    [TestMethod]
    public async Task Handle_CreateSucceeds_LogsInformationWithActionUserAndCreatedId()
    {
        Guid created = Guid.NewGuid();
        (LoggingBehavior<CreateThingCommand, Guid> behavior, ListLogger<LoggingBehavior<CreateThingCommand, Guid>> logger) = Create<CreateThingCommand, Guid>();

        await behavior.Handle(new CreateThingCommand(), _ => Task.FromResult(created), CancellationToken.None);

        ListLogger<LoggingBehavior<CreateThingCommand, Guid>>.LogEntry entry = logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Information);
        entry.Message.ShouldContain("CreateThing erfolgreich");
        entry.Message.ShouldContain(UserId.ToString());
        entry.Message.ShouldContain(created.ToString());
    }

    [TestMethod]
    public async Task Handle_DeleteSucceeds_LogsIdFromRequest()
    {
        Guid id = Guid.NewGuid();
        (LoggingBehavior<DeleteThingCommand, bool> behavior, ListLogger<LoggingBehavior<DeleteThingCommand, bool>> logger) = Create<DeleteThingCommand, bool>();

        await behavior.Handle(new DeleteThingCommand(id), _ => Task.FromResult(true), CancellationToken.None);

        ListLogger<LoggingBehavior<DeleteThingCommand, bool>>.LogEntry entry = logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Information);
        entry.Message.ShouldContain("DeleteThing erfolgreich");
        entry.Message.ShouldContain(id.ToString());
    }

    [TestMethod]
    public async Task Handle_DeleteReturnsFalse_LogsWarning()
    {
        (LoggingBehavior<DeleteThingCommand, bool> behavior, ListLogger<LoggingBehavior<DeleteThingCommand, bool>> logger) = Create<DeleteThingCommand, bool>();

        await behavior.Handle(new DeleteThingCommand(Guid.NewGuid()), _ => Task.FromResult(false), CancellationToken.None);

        ListLogger<LoggingBehavior<DeleteThingCommand, bool>>.LogEntry entry = logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Warning);
        entry.Message.ShouldContain("DeleteThing ohne Wirkung");
    }

    [TestMethod]
    public async Task Handle_QuerySucceeds_LogsActionWithoutQuerySuffix()
    {
        (LoggingBehavior<GetThingQuery, string?> behavior, ListLogger<LoggingBehavior<GetThingQuery, string?>> logger) = Create<GetThingQuery, string?>();

        await behavior.Handle(new GetThingQuery(Guid.NewGuid()), _ => Task.FromResult<string?>("x"), CancellationToken.None);

        logger.Entries.ShouldHaveSingleItem().Message.ShouldContain("GetThing erfolgreich");
    }

    [TestMethod]
    public async Task Handle_LoginSucceeds_LogsEmailButNotPassword()
    {
        (LoggingBehavior<LoginCommand, LoginResult?> behavior, ListLogger<LoggingBehavior<LoginCommand, LoginResult?>> logger) = Create<LoginCommand, LoginResult?>();
        LoginResult result = new("token", DateTimeOffset.UtcNow.AddHours(1), "a@beispiel.de", "A", "B", ["Viewer"]);

        await behavior.Handle(new LoginCommand("a@beispiel.de", "Geheim-12345"), _ => Task.FromResult<LoginResult?>(result), CancellationToken.None);

        ListLogger<LoggingBehavior<LoginCommand, LoginResult?>>.LogEntry entry = logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Information);
        entry.Message.ShouldContain("Anmeldung erfolgreich");
        entry.Message.ShouldContain("a@beispiel.de");
        entry.Message.ShouldNotContain("Geheim-12345");
        entry.Message.ShouldNotContain("token");
    }

    [TestMethod]
    public async Task Handle_LoginReturnsNull_LogsWarning()
    {
        (LoggingBehavior<LoginCommand, LoginResult?> behavior, ListLogger<LoggingBehavior<LoginCommand, LoginResult?>> logger) = Create<LoginCommand, LoginResult?>();

        await behavior.Handle(new LoginCommand("a@beispiel.de", "falsch"), _ => Task.FromResult<LoginResult?>(null), CancellationToken.None);

        ListLogger<LoggingBehavior<LoginCommand, LoginResult?>>.LogEntry entry = logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Warning);
        entry.Message.ShouldContain("Anmeldung fehlgeschlagen");
        entry.Message.ShouldNotContain("falsch");
    }

    [TestMethod]
    public async Task Handle_ValidationFails_LogsWarningWithoutValuesAndRethrows()
    {
        (LoggingBehavior<CreateThingCommand, Guid> behavior, ListLogger<LoggingBehavior<CreateThingCommand, Guid>> logger) = Create<CreateThingCommand, Guid>();
        ValidationException exception = new([new ValidationFailure("VIN", "Die VIN GEHEIMWERT ist falsch.") { ErrorCode = "RegexValidator" }]);

        await Should.ThrowAsync<ValidationException>(() => behavior.Handle(new CreateThingCommand(), _ => throw exception, CancellationToken.None));

        ListLogger<LoggingBehavior<CreateThingCommand, Guid>>.LogEntry entry = logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Warning);
        entry.Message.ShouldContain("CreateThing abgelehnt");
        entry.Message.ShouldContain("VIN:RegexValidator");
        entry.Message.ShouldNotContain("GEHEIMWERT");
    }

    [TestMethod]
    public async Task Handle_LoginValidationFails_LogsLoginWarning()
    {
        (LoggingBehavior<LoginCommand, LoginResult?> behavior, ListLogger<LoggingBehavior<LoginCommand, LoginResult?>> logger) = Create<LoginCommand, LoginResult?>();

        await Should.ThrowAsync<ValidationException>(() => behavior.Handle(
            new LoginCommand("", "x"), _ => throw new ValidationException([new ValidationFailure("Email", "leer")]), CancellationToken.None));

        logger.Entries.ShouldHaveSingleItem().Message.ShouldContain("Anmeldung fehlgeschlagen");
    }

    [TestMethod]
    public async Task Handle_UnexpectedException_LogsErrorWithExceptionAndRethrows()
    {
        (LoggingBehavior<CreateThingCommand, Guid> behavior, ListLogger<LoggingBehavior<CreateThingCommand, Guid>> logger) = Create<CreateThingCommand, Guid>();
        InvalidOperationException exception = new("kaputt");

        await Should.ThrowAsync<InvalidOperationException>(() => behavior.Handle(new CreateThingCommand(), _ => throw exception, CancellationToken.None));

        ListLogger<LoggingBehavior<CreateThingCommand, Guid>>.LogEntry entry = logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Error);
        entry.Exception.ShouldBeSameAs(exception);
        entry.Message.ShouldContain("CreateThing");
        entry.Message.ShouldContain(UserId.ToString());
    }

    [TestMethod]
    public async Task Handle_Cancelled_LogsNothingAndRethrows()
    {
        (LoggingBehavior<CreateThingCommand, Guid> behavior, ListLogger<LoggingBehavior<CreateThingCommand, Guid>> logger) = Create<CreateThingCommand, Guid>();

        await Should.ThrowAsync<OperationCanceledException>(() => behavior.Handle(new CreateThingCommand(), _ => throw new OperationCanceledException(), CancellationToken.None));

        logger.Entries.ShouldBeEmpty();
    }

    private static (LoggingBehavior<TRequest, TResponse> Behavior, ListLogger<LoggingBehavior<TRequest, TResponse>> Logger) Create<TRequest, TResponse>()
        where TRequest : IRequest<TResponse>
    {
        ListLogger<LoggingBehavior<TRequest, TResponse>> logger = new();
        ICurrentUser currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(UserId);

        return (new LoggingBehavior<TRequest, TResponse>(logger, currentUser), logger);
    }
}
