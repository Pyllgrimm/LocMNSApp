using LocMNSApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LocMNSApp.DTOs
{
    public class LocationDto
    {
        [Required]
        public DateTime DateDebut { get; set; }
        [Required]
        public int Duree { get; set; }
        /*[Required]
        [Precision(16, 2)]
        public StatusLocation? Status { get; set; }
        public decimal MontantTotal { get; set; }
        [Required]
        public DateTime DateRetourPrevue { get; set; }


        [Required]
        public virtual Utilisateur Demandeur { get; set; }
        [Required]
        public virtual Materiel MaterielDemande { get; set; }*/

       



        
    }
}
