using Microsoft.AspNetCore.Identity;

namespace FahrzeugverwaltungVPower.Core.Entities
{
    /// <summary>
    /// Repräsentiert einen Benutzer innerhalb der Anwendung.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Vorname des Benutzers.
        /// </summary>
        public required string FirstName { get; set; }

        /// <summary>
        /// Nachname des Benutzers.
        /// </summary>
        public required string LastName { get; set; }
    }
}
