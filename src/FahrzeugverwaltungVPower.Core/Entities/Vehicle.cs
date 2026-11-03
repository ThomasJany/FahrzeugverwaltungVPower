namespace FahrzeugverwaltungVPower.Core.Entities
{
    /// <summary>
    /// Repräsentiert einen Fahrzeugschein.
    /// </summary>
    /// <remarks>
    /// Referenz für die Ermittlung der Datentypen:
    /// https://www.kba.de/DE/Themen/ZentraleRegister/ZFZR/Info_behoerden/Regelungen_ZulBescheinigungen/leitfaden_Ausfuellung_Zulassungsbescheinigung_Teil_I_und_II.pdf?__blob=publicationFile&v=4
    /// </remarks>
    public record Vehicle : Entity
    {
        /// <summary>
        /// Kennzeichen, Bis zu 11 Zeichen, normalisiert ohne Leerzeichen. Bindestriche erlaubt.
        /// Feld A
        /// </summary>
        public required string LicensePlate { get; set; }

        /// <summary>
        /// Erstzulassung, nur Datum
        /// Feld B
        /// </summary>
        public required DateOnly FirstRegistrationDate { get; set; }

        /// <summary>
        /// Fahrzeughalter, Nachname oder Firmenname.
        /// Feld C.1.1
        /// </summary>
        public required string Holder { get; set; }

        /// <summary>
        /// Fahrzeughalter, Vorname oder <see langword="null"/> bei Firmen.
        /// Feld C.1.2
        /// </summary>
        public string? HolderFirstName { get; set; }
        
        /// <summary>
        /// Hersteller, 25 Zeichen.
        /// Feld D.1
        /// </summary>  
        public required string Manufacturer { get; set; }

        /// <summary>
        /// Fahrzeugtyp, 25 Zeichen
        /// Feld D.2
        /// </summary>
        public required string TypeDesignation { get; set; }

        /// <summary>
        /// Modellbezeichnung, 25 Zeichen.
        /// Feld D.3
        /// </summary>
        public required string Model { get; set; }

        /// <summary>
        /// Fahrzeug-Identifizierungsnummer, 17-stellig
        /// Feld E
        /// </summary>
        public required string VIN { get; set; }
                
        /// <summary>
        /// HU gültig bis, nur Datum
        /// Feld HU
        /// </summary>
        public required DateOnly InspectionValidUntil { get; set; }
        
        /// <summary>
        /// Hersteller-Schlüsselnummer, 4-stellig
        /// Feld 2.1
        /// </summary>
        public required string HSN { get; set; }
        
        /// <summary>
        /// Typschlüsselnummer 9-stellig
        /// Feld 2.2
        /// </summary>
        public required string TSN { get; set; }
    }
}
