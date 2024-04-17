using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LocMNSApp.DTOs
{
    public class MaterielDto
    {
        [Required, MaxLength(100)]
        public string NomMateriel { get; set; } = "";

        [Required, MaxLength(100)]
        public string Marque { get; set; } = "";

        [Required, MaxLength(100)]
        public string Categorie { get; set; } = "";

        [Required]
        public int NumeroSerie { get; set; }

        [Required, MaxLength(100)]
        public string Etat { get; set; } = "";

        [Required]
        [Precision(16, 2)]
        public decimal PrixParJour { get; set; }
    }
}
