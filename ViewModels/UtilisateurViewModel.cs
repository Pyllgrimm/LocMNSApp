
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LocMNSApp.ViewModels
{
    public class UtilisateurViewModel : IdentityUser
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
        public string RoleName { get; set; }
    }
}
