using Microsoft.EntityFrameworkCore;

namespace LocMNSApp.Models
{
    public class Location
    {
        public int Id { get; set; }
        public DateTime? DateDebut { get; set; }
        public int? Duree { get; set; }
        public DateTime? DateDemande { get; set; }
        public DateTime? DateRetourPrevue { get; set; }
        public DateTime? DateRetourReelle { get; set; }
        [Precision(16, 0)]
        public decimal MontantTotal { get; set; }

        public DateTime? ArchivateAt { get; set; }

        public virtual StatusLocation? Status { get; set; }
        public virtual Utilisateur Demandeur { get; set; }
        public virtual Materiel MaterielDemande { get; set; }

        
    }
}
