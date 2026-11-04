# Fahrzeugverwaltung

Übungsprojekt (.NET 10): Verwaltung von Fahrzeugstammdaten aus der Zulassungsbescheinigung Teil I
(Halter, Kennzeichen, VIN, Erstzulassung, HU, Hersteller, Typ, HSN und TSN).

## Aufbau

| Ordner | Inhalt |
|---|---|
| `src/FahrzeugverwaltungVPower.Core` | Geschäftslogik: Entitäten, Commands und Queries, Handler, Validatoren |
| `src/FahrzeugverwaltungVPower.Infrastructure` | Datenbankkontext, Migrationen, Identity, Token-Erzeugung, Admin-Seeder |
| `src/FahrzeugverwaltungVPower.API` | REST-Controller, Anmeldung und Berechtigungen, Swagger |
| `src/FahrzeugverwaltungVPower.UI` | Razor-Klassenbibliothek mit allen Seiten, Komponenten und Diensten der Oberfläche (MudBlazor). Sie ist von der Art des Hosts unabhängig und läuft sowohl im Blazor-Server-Host als auch in der MAUI-App |
| `src/FahrzeugverwaltungVPower.Blazor` | Blazor-Server-Host der Oberfläche: Start, Konfiguration und Speicherort der Anmeldung |
| `src/FahrzeugverwaltungVPower.MauiBlazor` | MAUI-Blazor-Hybrid-App (Windows, Android, iOS, Mac Catalyst): zeigt dieselbe Oberfläche, speichert die Anmeldung im sicheren Speicher des Geräts |
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

Die Oberfläche läuft in einem zweiten Prozess und ruft die API serverseitig auf. Die Adresse der API steht in
`src/FahrzeugverwaltungVPower.Blazor/appsettings.json` unter `Api:BaseUrl` (Standard `https://localhost:7139/`).
Zuerst die API starten, dann in einem zweiten Terminal:

```bash
dotnet run --project src/FahrzeugverwaltungVPower.Blazor --launch-profile https
```

Danach ist die Oberfläche unter <https://localhost:7094> erreichbar. Die Anmeldung erfolgt mit den Zugangsdaten
des Administrators (siehe oben).

### MAUI-App

Die App zeigt dieselben Seiten wie die Web-Oberfläche, weil alles in `FahrzeugverwaltungVPower.UI` liegt. Der Host
liefert nur den Speicherort der Anmeldung (sicherer Speicher des Geräts, `SecureStorageTokenStore`) und die Adresse
der API (`ApiEndpoints.cs`). Voraussetzung ist die MAUI-Workload (in Visual Studio: "Entwicklung mit .NET Multi-Platform App UI").
Start in Visual Studio mit `FahrzeugverwaltungVPower.MauiBlazor` als Startprojekt und Ziel "Windows-Computer" oder
einem Android-Emulator, oder für Windows auf der Kommandozeile:

```bash
dotnet run --project src/FahrzeugverwaltungVPower.MauiBlazor -f net10.0-windows10.0.19041.0
```

**API erreichen:** "localhost" ist auf einem Emulator oder Gerät nicht der Rechner mit der API. Außerdem vertrauen
sie dem HTTPS-Entwicklerzertifikat nicht. Deshalb spricht die App die API in der Entwicklung per HTTP auf Port 5112 an
(Profil `http`):

```bash
dotnet run --project src/FahrzeugverwaltungVPower.API --launch-profile http
```

| Läuft auf | Adresse der API |
|---|---|
| Windows, Mac, iOS-Simulator | `http://localhost:5112/` |
| Android-Emulator | `http://10.0.2.2:5112/` (so erreicht der Emulator den Rechner) |
| Echtes Gerät | `http://<IP-Adresse des Rechners>:5112/`, einzutragen in `PhysicalDeviceUrl` in `ApiEndpoints.cs` |

Für ein **echtes Gerät** muss die API im Netzwerk erreichbar sein, und Gerät und Rechner müssen im selben WLAN sein:

```bash
dotnet run --project src/FahrzeugverwaltungVPower.API --launch-profile http --urls http://0.0.0.0:5112
```

Die IP-Adresse des Rechners zeigt `ipconfig`. Die Windows-Firewall muss Port 5112 im privaten Netzwerk zulassen,
beim ersten Start fragt Windows danach.

HTTP ohne Verschlüsselung ist nur für die Entwicklung gedacht. Dafür erlauben `Platforms/Android/AndroidManifest.xml`
(`usesCleartextTraffic`) und `Platforms/iOS/Info.plist` (`NSAllowsLocalNetworking`) HTTP, beide mit Hinweis im Kommentar.
Für eine echte Veröffentlichung entfernen und die API per HTTPS mit gültigem Zertifikat betreiben. Eine Alternative
für Geräte ohne gemeinsames WLAN sind Dev Tunnels von Visual Studio, die eine öffentliche HTTPS-Adresse auf die lokale API
legen. Das ist hier nicht ausprobiert.

**Layout auf kleinen Bildschirmen:** Unter 960 px Breite (Handy, schmales Fenster) liegt das Menü als Schublade über
dem Inhalt und ist zunächst geschlossen, in der Kopfzeile bleiben nur Symbole. Das gilt in der Web-Oberfläche genauso.
Unter Android läuft die App Edge-to-Edge: Die Oberfläche reicht bis an den Bildschirmrand, die Statusleiste ist in
der Farbe der Kopfzeile eingefärbt. Die Abstände der Systemleisten und der Tastatur meldet `MainActivity` an die
Seite (`SafeAreaService`, `SafeAreaBridge`), das CSS dazu steht in `wwwroot/app.css`.

Stand: Die App baut für alle vier Ziele (Windows, Android, iOS, Mac Catalyst) und startet unter Windows. Auf einem
Android-Gerät und im Emulator, unter iOS und unter Mac Catalyst wurde sie noch nicht ausgeführt (iOS und Mac
Catalyst brauchen dafür einen Mac).

Tests: `dotnet test test/FahrzeugverwaltungVPower.Test`

## Oberfläche

- **Anmelden:** E-Mail und Passwort. Das Token liegt im Session-Storage des Browsers und gilt nur für diesen Tab.
- **Benutzer (nur Admin):** Tabelle mit Sortierung und Suche, Klick auf eine Zeile öffnet die Detailseite zum
  Bearbeiten. Das Passwort wird zweimal eingegeben. Beim Bearbeiten lässt ein leeres Passwortfeld das bisherige
  Passwort unverändert. Löschen fragt immer in einem Dialog nach. Die Eingaben werden schon beim Tippen geprüft,
  mit 0,5 Sekunden Verzögerung nach dem letzten Tastendruck.
- **Fahrzeuge (alle Rollen):** Tabelle mit Kennzeichen, Halter, Fahrzeug, HU und VIN, sortierbar und mit Suche. Die
  HU ist rot, wenn sie abgelaufen ist, und gelb, wenn sie in den nächsten 3 Monaten fällig wird. Klick auf eine
  Zeile öffnet die Detailseite mit allen Feldern. Anlegen und Löschen dürfen Creator und Admin, Bearbeiten
  zusätzlich der Editor. Der Viewer sieht die Detailseite nur lesend. Löschen fragt immer in einem Dialog nach.
- **Tabellen:** Beide Tabellen sind unabhängig von der Bildschirmgröße in der dichten Darstellung.
- **Prüfung der Eingaben:** Die Felder werden schon beim Tippen geprüft, mit 0,5 Sekunden Verzögerung nach dem
  letzten Tastendruck. Die Regeln der Oberfläche sind eine Kopie der Validatoren in Core (siehe Ausblick). Ob eine
  E-Mail-Adresse oder VIN schon vergeben ist, prüft nur die API beim Speichern.
- **Hell/Dunkel:** Umschalter in der Kopfzeile. Zu Beginn gilt die Einstellung des Systems.
- **TestUser erstellen / Testfahrzeug zufügen:** Die Knöpfe auf der Benutzer- bzw. Fahrzeugseite legen einen
  Datensatz mit zufälligen Daten an. Ein Testbenutzer hat eine zufällige Rollenkombination (nie Admin) und eine noch
  nicht vergebene Adresse `vorname.nachname@example.test`. Das Passwort aller Testbenutzer steht in der Meldung
  nach dem Anlegen. Ein Testfahrzeug hat gültige Daten und eine noch nicht vergebene VIN, die HU ist gemischt
  abgelaufen, bald fällig oder länger gültig. **Beide Knöpfe sind nur für Präsentationszwecke gedacht** und gehören
  in einer echten Anwendung nicht in die Oberfläche.

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
| MudBlazor 9.12.0 | Komponenten und Layout der Oberfläche | MIT |
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
| MudBlazor statt Bootstrap: fertige Tabelle mit Sortierung, Dialoge und Eingabefelder mit Validierung, MIT-Lizenz. Schrift: Systemschrift statt Google Fonts (keine Verbindung zu Drittservern) | `UI/Layout/MainLayout.razor`, `UI/Services/UiServiceCollectionExtensions.cs` |
| Zugangsdaten des Administrators und JWT-Schlüssel in `appsettings.json` statt in Secrets (Projekt soll ohne Einrichtung laufen). In echten Anwendungen gehören sie in User-Secrets, Umgebungsvariablen oder einen Secret-Store | `Infrastructure/Data/AdminSeeder.cs`, `Core/Security/JwtOptions.cs` |
| Migrationen werden bei jedem Start angewendet (in Produktion stattdessen Migration Bundle oder SQL-Skript in der Pipeline) | `API/Program.cs` |
| Selbst ausgestellte JWT ohne Refresh-Token: ausgestellte Tokens lassen sich nicht widerrufen, nach Löschen oder Rollenwechsel eines Benutzers gelten sie bis zum Ablauf | `Infrastructure/Security/JwtTokenService.cs` |
| Anmeldung mit `UserManager` statt `SignInManager`, damit Core frei von ASP.NET bleibt | `Core/Commands/Auth/Handlers/LoginCommandHandler.cs` |
| Rechte der Rollen, Admin als Ergänzung zur Aufgabe | `API/SecurityServiceCollectionExtensions.cs` |
| Benutzer werden gelöscht statt deaktiviert, deshalb speichert `CreatedBy` die Id des Benutzers ohne Fremdschlüssel | `Core/Commands/Users/Handlers/DeleteUserCommandHandler.cs`, `Core/Entities/Entity.cs` |
| Datenbankname `FahrzeugverwaltungVPower_Jany`, damit mehrere Projekte auf demselben Rechner nicht dieselbe Datenbank benutzen | `API/appsettings.json` (`ConnectionStrings:Default`) |

## Ausblick

- Logging mit Serilog, nur in die Datenbank.
- Gemeinsames Projekt `FahrzeugverwaltungVPower.Contracts` für die Prüfregeln: Heute stehen die Regeln für Benutzer
  und Fahrzeuge zweimal im Code, als FluentValidation-Validatoren in Core und als Kopie in der Oberfläche
  (`UI/Shared/UserRules.cs`, `UI/Shared/VehicleRules.cs`). Die Basisklassen der Commands und die synchronen
  Validatoren (`UserCommandValidatorBase`, `VehicleCommandValidatorBase`) könnten in ein kleines Projekt ziehen, das
  nur FluentValidation braucht und von Core und der Oberfläche genutzt wird. Die Prüfungen auf doppelte E-Mail und
  VIN bleiben in Core, weil sie die Datenbank brauchen.
- **Automatisches Einlesen des Fahrzeugscheins** (Aufgabe 6): aus Zeitgründen nicht umgesetzt. Das Konzept steht im nächsten
  Abschnitt.

### Konzept: Fahrzeugschein einlesen

**Ziel:** Ein Foto oder Scan der Zulassungsbescheinigung Teil I füllt das Formular "Neues Fahrzeug" vor. Gespeichert
wird nichts automatisch: Der Benutzer sieht die erkannten Werte im Formular, prüft sie und speichert selbst.

**Entscheidung: Tesseract, lokal.** Ein Fahrzeugschein enthält Name und Anschrift des Halters, also personenbezogene
Daten. Ein Cloud-Dienst würde das Bild zur Verarbeitung an einen Drittanbieter senden. Deshalb wird die Erkennung mit
Tesseract (Open Source, Apache-2.0, kostenlos) in der eigenen API ausgeführt. Das Bild verlässt den Rechner nicht und
wird nach der Erkennung verworfen, es wird nicht gespeichert. Alternativen wären Cloud-Dienste zur Dokumentenerkennung
oder Bildmodelle per API gewesen, sie wurden aus dem genannten Datenschutzgrund nicht gewählt und deshalb nicht
weiter untersucht.

**Ablauf**

1. **Bild beschaffen:** Im Web über die Dateiauswahl. In der MAUI-App zusätzlich über die Gerätekamera. 
   Dafür käme eine Schnittstelle in der Razor-Klassenbibliothek analog zu `ITokenStore`, die der
   jeweilige Host umsetzt. Das Bild wird vor dem Senden verkleinert.
2. **API-Endpunkt** (zum Beispiel `POST api/vehicles/ScanRegistration`, Berechtigungen wie beim Anlegen), mit Begrenzung von
   Dateityp und -größe. Die Erkennung läuft ausschließlich dort.
3. **Vorbereiten:** Drehung laut EXIF-Angabe anwenden, dann die Ausrichtung erkennen.
4. **Erkennen:** Tesseract mit den Sprachdaten `deu` und `osd` und einem Modus für verstreuten Text, weil ein
   Formular keinen Fließtext enthält. Ergebnis sind Wörter mit Position und Sicherheit.
5. **Felder zuordnen** (Extraktion, siehe unten).
6. **Prüfen:** Jeder erkannte Wert läuft durch dieselben Regeln wie die Eingabe im Formular (`VehicleRules`). Was
   durchfällt, wird markiert und nicht stillschweigend übernommen.
7. **Antwort:** Die erkannten Felder, die Sicherheit je Feld und Warnungen. Die Oberfläche füllt das Formular vor und
   hebt unsichere Felder hervor. Die Eingabeprüfung im Formular erledigt den Rest.

**Felder zuordnen:** Die Zulassungsbescheinigung hat feste Feldkennungen (A Kennzeichen, B Erstzulassung,
C.1.1 und C.1.2 Halter, D.1 bis D.3 Hersteller, Typ und Modell, E VIN, 2.1 HSN, 2.2 TSN). Zwei Wege ergänzen sich:

- Werte mit eindeutigem Muster werden unabhängig von ihrer Lage gefunden: die VIN (17 Zeichen ohne I, O, Q), das
  Kennzeichen, Datumsangaben (`dd.MM.yyyy`), HSN (4 Ziffern) und TSN.
- Freitexte (Halter, Hersteller, Typ, Modell) werden über ihre Feldkennung gefunden: Die Kennung suchen und den Text
  daneben bzw. darunter nehmen.

Erkennung (Bild zu Wörtern) und Extraktion (Wörter zu Feldern) sind getrennte Klassen. Die Extraktion lässt sich so mit
gespeicherten Beispielausgaben ohne Tesseract in den Unit-Tests prüfen.

**Schräg, quer, auf dem Kopf:** Zuerst die EXIF-Drehung, dann die Ausrichtungserkennung von Tesseract (0, 90, 180, 270
Grad). Als Absicherung kann das Bild in allen vier Drehungen erkannt und die gewählt werden, bei der die meisten Felder
die Prüfregeln erfüllen. Schiefe Aufnahmen (Perspektive) und starke Schräglage wären ein weiterer Ausbauschritt, zum
Beispiel mit einer Bildbibliothek mit freier Lizenz.

**Voraussichtliche Bausteine:** Tesseract (Apache-2.0) mit einem .NET-Wrapper, der .NET 10 und die Betriebssysteme der
API unterstützt (zum Beispiel `Tesseract.CrossPlatform`), und die Sprachdateien `deu` und `osd`, die mit der API
ausgeliefert werden. Version und Lizenz sind vor dem Einbau auf NuGet zu prüfen. Bei der Bildverarbeitung ist auf die
Lizenz zu achten: ImageSharp ab Version 3 steht unter einer Lizenz mit Bedingungen für kommerzielle Nutzung. Eine
Alternative mit MIT-Lizenz wäre SkiaSharp.

**Grenzen:** Tesseract braucht ausreichend große, kontrastreiche Schrift (grob 20 bis 30 Pixel Höhe für Kleinbuchstaben)
und gerade Textzeilen. Der Sicherheitshintergrund des Dokuments, Spiegelungen und Schatten mindern die Trefferquote.
Alte Fahrzeugscheine (vor Teil I und II) haben ein anderes Layout und wären nicht abgedeckt. Ob das HU-Datum zuverlässig
auf dem Dokument steht, müssten Beispieldokumente zeigen, sonst bleibt es ein manuelles Pflichtfeld.

**Nächste Schritte:** Einige Muster- oder geschwärzte Dokumente mit den richtigen Werten sammeln (nicht in das
Repository), mit der Tesseract-Kommandozeile prüfen, was ohne Vorverarbeitung erkannt wird, daraus Vorverarbeitung und
Extraktion ableiten und die Trefferquote je Feld messen. Danach Upload im Web, Endpunkt in der API und zuletzt die Kamera in
der MAUI-App.
