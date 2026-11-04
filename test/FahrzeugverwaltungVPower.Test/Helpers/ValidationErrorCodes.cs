namespace FahrzeugverwaltungVPower.Test.Helpers;

/// <summary>
/// Fehlercodes der FluentValidation-Regeln, wie sie die Validator-Tests erwarten.
/// </summary>
internal static class ValidationErrorCodes
{
    /// <summary>Code der Regel NotEmpty.</summary>
    public const string NotEmpty = "NotEmptyValidator";

    /// <summary>Code der Regel Matches.</summary>
    public const string RegularExpression = "RegularExpressionValidator";

    /// <summary>Code der Regel MaximumLength.</summary>
    public const string MaxLength = "MaximumLengthValidator";

    /// <summary>Code der Regel Must.</summary>
    public const string Predicate = "PredicateValidator";

    /// <summary>Code der Regel MustAsync.</summary>
    public const string AsyncPredicate = "AsyncPredicateValidator";

    /// <summary>Eigener Code für ein nicht vorhandenes Fahrzeug, bewusst als Literal und nicht aus Core übernommen.</summary>
    public const string NotFound = "NotFound";
}
