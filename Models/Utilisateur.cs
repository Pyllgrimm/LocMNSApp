
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LocMNSApp.Models
{
    public class Utilisateur : IdentityUser
    {
        
        [StringLength(100)]
        [MaxLength(100)]
        [Required]
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Adresse { get; set; }
        public int CodePostal { get; set; }
        public string? Ville { get; set; }
        public string? Promotion { get; set; }
        public DateTime DateEnregistrement { get; set; }
        public List<Location> Locations { get; set; }
        public string? RoleName { get; set; }
        public DateTime? ArchivateAt { get; set; }

        public Utilisateur()
        {
            Locations = new List<Location>();
        }
    }
}
