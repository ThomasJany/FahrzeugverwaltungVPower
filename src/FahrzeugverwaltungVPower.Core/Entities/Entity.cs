namespace FahrzeugverwaltungVPower.Core.Entities
{
    /// <summary>
    /// Basisklasse einer Entität.
    /// </summary>
    public abstract record Entity
    {
        /// <summary>
        /// Eindeutiger Identifikator der Entität.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Zeitpunkt der Erstellung der Entität.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Technische Id des Benutzers, der die Entität angelegt hat.
        /// </summary>
        public required Guid CreatedBy { get; set; }

        /// <summary>
        /// Zeitpunkt der letzten Änderung an der Entität.
        /// </summary>
        public DateTimeOffset ModifiedAt { get; set; }
    }
}
