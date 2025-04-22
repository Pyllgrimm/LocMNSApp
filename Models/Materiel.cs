using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LocMNSApp.Models
{
    public class Materiel
    {
        public int Id { get; set; }

        [Required]
        public string NomMateriel { get; set; } = "";
        [Required]
        public string Marque { get; set; } = "";
        [Required]
        public string Categorie { get; set; } = "";
        [Required]
        public int NumeroSerie { get; set; }
        [Required]
        public string Etat { get; set; } = "";
        [Required]
        [Precision(16, 2)]
        public decimal PrixParJour { get; set; }
        public string? Disponibilitee { get; set; } = "Disponible";

        public DateTime DateCreation { get; set; }
        public DateTime? ArchivateAt { get; set; }
    }
}
