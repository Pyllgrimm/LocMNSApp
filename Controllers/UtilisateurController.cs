using LocMNSApp.Data;
using LocMNSApp.DTOs;
using LocMNSApp.Models;
using LocMNSApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LocMNSApp.Controllers
{
    [Authorize]
    public class UtilisateurController : Controller
    {
        private readonly LocMNSAppDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;



        public UtilisateurController(LocMNSAppDbContext context, UserManager<Utilisateur> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult MonCompte()
        {
            var id = _userManager.GetUserAsync(User).Result.Id;

            var utilisateur = _context.Utilisateurs.Find(id);
            ViewData["DateEnregistrement"] = utilisateur.DateEnregistrement.ToString("dd/MM/yyyy");

            return View(utilisateur);
        }

        [HttpPost]
        public IActionResult MonCompte(UtilisateurDto utilisateurDto)
        {
            var id = _userManager.GetUserAsync(User).Result.Id;

            var utilisateur = _context.Utilisateurs.Find(id);

            if (!ModelState.IsValid)
            {
                ViewData["DateEnregistrement"] = utilisateur.DateEnregistrement.ToString("dd/MM/yyyy");

                return View(utilisateurDto);
            }

            utilisateur.Nom = utilisateurDto.Nom;
            utilisateur.Prenom = utilisateurDto.Prenom;
            utilisateur.Adresse = utilisateurDto.Adresse;
            utilisateur.CodePostal = utilisateurDto.CodePostal;
            utilisateur.Ville = utilisateurDto.Ville;
            utilisateur.Promotion = utilisateurDto.Promotion;
            utilisateur.PhoneNumber = utilisateurDto.PhoneNumber;

            _context.SaveChanges();
            TempData["success"] = "Modifié avec succès";

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var utilisateurs = await (from user in _context.Users
                                     join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                                     join role in _context.Roles on userRoles.RoleId equals role.Id
                                     where user.ArchivateAt == null
                                     select new UtilisateurViewModel()
                                     {
                                         Id = user.Id, 
                                         Nom = user.Nom,  
                                         Prenom = user.Prenom,
                                         Adresse = user.Adresse,
                                         CodePostal = user.CodePostal,
                                         PhoneNumber = user.PhoneNumber,
                                         Ville = user.Ville,
                                         Promotion= user.Promotion,
                                         DateEnregistrement = user.DateEnregistrement, 
                                         RoleName = role.Name 
                                     })
                        .ToListAsync(); 
            
            return View(utilisateurs);

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var utilisateur =  _userManager.FindByIdAsync(id).Result;
            
            
            if (utilisateur == null)
                return RedirectToAction("Index");

            UtilisateurDto utilisateurDto = new UtilisateurDto()
            {
                Nom = utilisateur.Nom,
                Prenom = utilisateur.Prenom,
                Adresse = utilisateur.Adresse,
                CodePostal = utilisateur.CodePostal,
                Ville = utilisateur.Ville,
                Promotion = utilisateur.Promotion,
                PhoneNumber = utilisateur.PhoneNumber, 
                RoleName = _userManager.GetRolesAsync(utilisateur).Result.FirstOrDefault()
            };

            ViewData["UtilisateurId"] = utilisateur.Id;
            ViewData["DateEnregistrement"] = utilisateur.DateEnregistrement.ToString("dd/MM/yyyy");

            return View(utilisateurDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, UtilisateurDto utilisateurDto)
        {
            var utilisateur = _userManager.FindByIdAsync(id).Result;
            /*var utilisateurInRole = _context.UserRoles.Where(u=>u.UserId==id).Select(u=>u.RoleId);*/
            utilisateur.RoleName = _userManager.GetRolesAsync(utilisateur).Result.FirstOrDefault();

            if (utilisateur == null)
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
            {
                ViewData["UtilisateurId"] = utilisateur.Id;
                ViewData["DateEnregistrement"] = utilisateur.DateEnregistrement.ToString("dd/MM/yyyy");

                return View(utilisateurDto);
            }

            utilisateur.Nom = utilisateurDto.Nom;
            utilisateur.Prenom = utilisateurDto.Prenom;
            utilisateur.Adresse = utilisateurDto.Adresse;
            utilisateur.CodePostal = utilisateurDto.CodePostal;
            utilisateur.Ville = utilisateurDto.Ville;
            utilisateur.Promotion = utilisateurDto.Promotion;
            utilisateur.PhoneNumber = utilisateurDto.PhoneNumber;

            if (utilisateur.RoleName != utilisateurDto.RoleName)
            {
                 var oldRole = utilisateur.RoleName;
                 await _userManager.RemoveFromRoleAsync(utilisateur, oldRole);
                 await _userManager.AddToRoleAsync(utilisateur, utilisateurDto.RoleName);
                 _userManager.UpdateAsync(utilisateur);
            }
            utilisateur.RoleName = utilisateurDto.RoleName;
            TempData["success"] = "Utilisateur modifié avec succès";

            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            var utilisateur = _context.Utilisateurs.Find(id);

            if (utilisateur == null)
                return RedirectToAction("Index");

            utilisateur.ArchivateAt = DateTime.Now;
            _context.SaveChanges(true);
            TempData["success"] = "Utilisateur archivé avec succès";

            return RedirectToAction("Index", "Utilisateur");
        }

       
    }
}
