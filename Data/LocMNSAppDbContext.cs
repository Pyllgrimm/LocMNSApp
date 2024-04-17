using LocMNSApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LocMNSApp.Data
{
    public class LocMNSAppDbContext : IdentityDbContext<Utilisateur>
    {
        public LocMNSAppDbContext(DbContextOptions<LocMNSAppDbContext> options) : base(options)
        {
            
        }
        
        
        public DbSet<Materiel> Materiels { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
    }
}
