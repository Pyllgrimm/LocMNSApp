using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LocMNSApp.Data;
using LocMNSApp.Models;
using LocMNSApp.DTOs;
using Microsoft.AspNetCore.Identity;
using NuGet.Versioning;
using Microsoft.AspNetCore.Authorization;
namespace LocMNSApp.Controllers
{
    [Authorize]
    public class LocationController : Controller
    {
        private readonly LocMNSAppDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly SignInManager<Utilisateur> _signInManager;

        public LocationController(LocMNSAppDbContext context, UserManager<Utilisateur> userManager, SignInManager<Utilisateur> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Admin, Collaborateur")]
        public async Task<IActionResult> Index()
        {
            int locationEnAttente = 0;


            var locations = await (from l in _context.Locations
                            join s in _context.StatueLocations
                            on l.Status.Id equals s.Id
                            join u in _context.Utilisateurs
                            on l.Demandeur.Id equals u.Id
                            join m in _context.Materiels
                            on l.MaterielDemande.Id equals m.Id
                            where l.ArchivateAt == null
                            select new Location
                            {
                                Id = l.Id,
                                DateDebut = l.DateDebut,
                                Duree = l.Duree,
                                DateDemande = l.DateDemande,
                                DateRetourPrevue = l.DateRetourPrevue,
                                MontantTotal = l.MontantTotal,
                                Status = l.Status,
                                Demandeur = l.Demandeur,
                                MaterielDemande = l.MaterielDemande,
                            }).ToListAsync();

            foreach (var location in locations)
            {
                if (location.Status.StatusName == null)
                    return View(locations);

                locationEnAttente++;
            }

            ViewData["LocationEnAttente"] = locationEnAttente;

            return View(locations);
        }

        public async Task<IActionResult> UserLocations()
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            

            var mesLocations = await (from l in _context.Locations
                                   join s in _context.StatueLocations
                                   on l.Status.Id equals s.Id
                                   join u in _context.Utilisateurs
                                   on l.Demandeur.Id equals u.Id
                                   join m in _context.Materiels
                                   on l.MaterielDemande.Id equals m.Id
                                   where l.Demandeur.Id == userId && l.ArchivateAt == null
                                   select new Location
                                   {
                                       
                                       DateDebut = l.DateDebut,
                                       Duree = l.Duree,
                                       DateDemande = l.DateDemande,
                                       DateRetourPrevue = l.DateRetourPrevue,
                                       MontantTotal = l.MontantTotal,
                                       Status = l.Status,
                                       MaterielDemande = l.MaterielDemande,
                                   }).ToListAsync();

            return View(mesLocations);
        }




        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int id, LocationDto locationDto)
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            var materiel = _context.Materiels.Find(id);
            if (!ModelState.IsValid)
                return View(locationDto);

            Location location = new Location()
            {
                DateDemande = DateTime.Now,
                MaterielDemande = materiel,
                DateDebut = locationDto.DateDebut,
                Duree =locationDto.Duree,
                MontantTotal = locationDto.Duree * materiel.PrixParJour ,
                DateRetourPrevue = locationDto.DateDebut + TimeSpan.FromDays(locationDto.Duree),
                Status = _context.StatueLocations.FirstOrDefault(),
                Demandeur = _userManager.FindByIdAsync(userId).Result,
            };

            /*materiel.Disponibilitee = "Indisponible";*/
            ViewData["Duree"] = location.Duree;
            ViewData["MontantTotale"] = location.MontantTotal;

            location.Demandeur.Locations.Add(location);
            _context.Locations.Add(location);
            
            _context.SaveChanges();
            TempData["success"] = "Location créée avec succès";

            return RedirectToAction("Index", "Home");
            
        }

        // GET: Location/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Location/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Location location)
        {
            if (id != location.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["DemandeurID"] = location.Demandeur.Id;
            return View(location);
        }

        [Authorize(Roles ="Admin")]
        public IActionResult Delete(int? id)
        {
            var location = _context.Locations.Find(id);
                
            if (location == null)
            {
                return RedirectToAction("Index");
            }

            _context.Locations.Remove(location);
            _context.SaveChanges(true);
            TempData["success"] = "Location archivée avec succès";

            return RedirectToAction("Index");
        }

        

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }
    }
}
