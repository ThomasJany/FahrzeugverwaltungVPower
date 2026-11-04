# Fahrzeugverwaltung

Übungsprojekt (.NET 10): Verwaltung von Fahrzeugstammdaten aus der Zulassungsbescheinigung Teil I
(Halter, Kennzeichen, VIN, Erstzulassung, HU, Hersteller, Typ, HSN und TSN).

## Aufbau

| Ordner | Inhalt |
|---|---|
| `src/FahrzeugverwaltungVPower.Core` | Geschäftslogik: Entitäten, Commands und Queries, Handler, Validatoren |
| `src/FahrzeugverwaltungVPower.Infrastructure` | Datenbankkontext, Migrationen, Identity, Token-Erzeugung, Admin-Seeder |
| `src/FahrzeugverwaltungVPower.API` | REST-Controller, Anmeldung und Berechtigungen, Swagger |
| `src/FahrzeugverwaltungVPower.Blazor` | Web-Oberfläche |
| `test/FahrzeugverwaltungVPower.Test` | Unit-Tests |

## Starten

Voraussetzungen:

- Windows (SQL Server LocalDB gibt es nur unter Windows)
- .NET 10 SDK
- SQL Server LocalDB (Teil von Visual Studio, einzeln über SQL Server Express installierbar)
- HTTPS-Entwicklerzertifikat, einmalig als vertrauenswürdig einstufen:

  ```bash
  dotnet dev-certs https --trust
  ```

Ohne LocalDB (oder ohne Windows) kann in `src/FahrzeugverwaltungVPower.API/appsettings.json` unter
`ConnectionStrings:Default` ein anderer SQL Server eingetragen werden. Das Konto braucht dort das Recht,
Datenbanken und Tabellen anzulegen.

```bash
dotnet run --project src/FahrzeugverwaltungVPower.API --launch-profile https
```

Beim Start wird die Datenbank `FahrzeugverwaltungVPower_Jany` angelegt bzw. aktualisiert und der Standard-Administrator
angelegt. Dessen E-Mail und Passwort stehen in `src/FahrzeugverwaltungVPower.API/appsettings.json`
im Abschnitt `Seed`. Die Werte gelten nur beim Anlegen, spätere Änderungen in der Datei haben keine
Wirkung, wenn der Administrator schon in der Datenbank steht.

Der Datenbankname steht im Connection String (`ConnectionStrings:Default` in derselben Datei) und ist projektspezifisch gewählt, damit sich mehrere Projekte auf demselben Rechner nicht dieselbe Datenbank teilen. Wer das Projekt ausführt, kann den Namen dort ändern.

Tests: `dotnet test test/FahrzeugverwaltungVPower.Test`

## Mit Swagger testen

Swagger: <https://localhost:7139/swagger>. Alle Endpunkte außer dem Login verlangen eine Anmeldung,
ohne Token antwortet die API mit `401`.

1. `POST /api/auth/Login` mit E-Mail und Passwort des Administrators aufrufen.
2. In der Antwort den Wert von `token` kopieren (ohne Anführungszeichen).
3. Oben rechts auf **Authorize** klicken, das Token einfügen und bestätigen. Das Wort `Bearer` ergänzt
   Swagger selbst.

Das Token gilt `Jwt:ExpiresMinutes` Minuten (Standard 60), danach ist ein neuer Login nötig. Nach dem
Neuladen der Seite muss es erneut eingetragen werden. `401` heißt nicht angemeldet oder Token abgelaufen,
`403` heißt angemeldet, aber die Rolle reicht nicht. Nach 5 falschen Passwörtern ist das Konto 5 Minuten
gesperrt.

Die Adressen enthalten den Namen der Aktion, zum Beispiel `api/auth/Login`, `api/users/GetAllUsers`
(nur Admin) und `api/vehicles/GetAllVehicles`. Fahrzeuge und Benutzer werden über ihre technische Id
angesprochen (zum Beispiel `api/vehicles/GetVehicleById/{id}`). Die VIN eines Fahrzeugs ist dagegen
änderbar.

## Rollen

| Rolle | Rechte |
|---|---|
| Viewer | Fahrzeuge ansehen |
| Editor | ansehen und bearbeiten |
| Creator | ansehen, bearbeiten, anlegen und löschen |
| Admin | alle Fahrzeugaktionen und die Benutzerverwaltung |

## Verwendete Bibliotheken (Drittanbieter, nicht von Microsoft)

Lizenzangaben aus den NuGet-Metadaten der verwendeten Versionen bzw. aus den Dateien selbst.
Bibliotheken von Microsoft (.NET, ASP.NET Core, Entity Framework Core, MSTest u. a.) sind nicht aufgeführt.

| Bibliothek | Zweck | Lizenz |
|---|---|---|
| MediatR 14.2.0 | Commands, Queries und Pipeline | RPL-1.5 oder kommerzielle Lizenz des Herstellers (kostenlose Community-Lizenz laut Hersteller, siehe Entscheidungen) |
| FluentValidation 12.2.0 | Validierung der Eingaben | Apache-2.0 |
| Swashbuckle.AspNetCore 10.3.1 | Swagger-Dokumentation und -Oberfläche | MIT |
| Swagger UI (JavaScript, in Swashbuckle enthalten) | Oberfläche zum Testen der API | Apache-2.0 |
| Bootstrap 5.3.3 (CSS und JavaScript, Blazor-Vorlage) | Layout der Oberfläche | MIT |
| NSubstitute 6.2.0 (nur Tests) | Mocks | BSD-3-Clause |
| Shouldly 4.3.0 (nur Tests) | Prüfungen in Tests | BSD-3-Clause |

Mittelbar eingebunden (Auszug): Castle.Core 5.1.1 (Apache-2.0, über NSubstitute), DiffEngine 11.3.0 und
EmptyFiles 4.4.0 (MIT, über Shouldly), Humanizer.Core 2.14.1 und Mono.TextTemplating 3.0.0 (MIT, über
die EF-Core-Werkzeuge), Newtonsoft.Json 13.0.4 (MIT), MediatR.Contracts 2.0.1 (Apache-2.0).

## Entscheidungen

Die rechte Spalte nennt die Stelle im Code.

| Entscheidung | Wo im Code |
|---|---|
| MediatR trotz geänderter Lizenz (Community-Lizenz, Lernprojekt) | `API/ServiceCollectionExtensions.cs` |
| Zugangsdaten des Administrators und JWT-Schlüssel in `appsettings.json` statt in Secrets (Projekt soll ohne Einrichtung laufen). In echten Anwendungen gehören sie in User-Secrets, Umgebungsvariablen oder einen Secret-Store | `Infrastructure/Data/AdminSeeder.cs`, `Core/Security/JwtOptions.cs` |
| Migrationen werden bei jedem Start angewendet (in Produktion stattdessen Migration Bundle oder SQL-Skript in der Pipeline) | `API/Program.cs` |
| Selbst ausgestellte JWT ohne Refresh-Token: ausgestellte Tokens lassen sich nicht widerrufen, nach Löschen oder Rollenwechsel eines Benutzers gelten sie bis zum Ablauf | `Infrastructure/Security/JwtTokenService.cs` |
| Anmeldung mit `UserManager` statt `SignInManager`, damit Core frei von ASP.NET bleibt | `Core/Commands/Auth/Handlers/LoginCommandHandler.cs` |
| Rechte der Rollen, Admin als Ergänzung zur Aufgabe | `API/SecurityServiceCollectionExtensions.cs` |
| Fahrzeuge werden über die technische Id angesprochen, weil die VIN änderbar ist (eine Korrektur würde sonst die Adresse des Fahrzeugs ändern) | `API/Controllers/VehicleController.cs` |
| Benutzer werden gelöscht statt deaktiviert, deshalb speichert `CreatedBy` die Id des Benutzers ohne Fremdschlüssel | `Core/Commands/Users/Handlers/DeleteUserCommandHandler.cs`, `Core/Entities/Entity.cs` |
| Swagger steht vor der Anmeldung, damit man sich dort einloggen kann | `API/Program.cs` |
| Eigener Datenbankname (`FahrzeugverwaltungVPower_Jany`), damit mehrere Projekte auf demselben Rechner nicht dieselbe Datenbank benutzen | `API/appsettings.json` (`ConnectionStrings:Default`) |

## Ausblick

- Logging mit Serilog, nur in die Datenbank.
