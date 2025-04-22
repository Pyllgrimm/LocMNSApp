using LocMNSApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LocMNSApp.ViewModels
{
    public class LocationViewModel
    {
        [Required]
        public DateTime DateDebut { get; set; }
        [Required]
        public int Duree { get; set; }
        [Required]
        public DateTime DateDemande { get; set; }
        [Required]
        public DateTime DateRetourPrevue { get; set; }

        public StatusLocation? Status { get; set; }

        [Required]
        [Precision(16, 0)]
        public decimal MontantTotal { get; set; }

      
    }
}
