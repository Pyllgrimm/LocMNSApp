using LocMNSApp.Data;
using LocMNSApp.DTOs;
using LocMNSApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace LocMNSApp.Controllers
{
    [Authorize]
    public class MaterielController : Controller
    {
        private readonly LocMNSAppDbContext _context;
        public MaterielController(LocMNSAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var materiels = _context.Materiels.Where(m=>m.ArchivateAt==null).ToList();
            return View(materiels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(MaterielDto materielDto)
        {
            if (!ModelState.IsValid)
                return View(materielDto);


            Materiel materiel = new Materiel()
            {
                NomMateriel = materielDto.NomMateriel,
                Marque = materielDto.Marque,
                Categorie = materielDto.Categorie,
                NumeroSerie = materielDto.NumeroSerie,
                Etat = materielDto.Etat,
                PrixParJour = materielDto.PrixParJour,
                DateCreation = DateTime.Now
            };

            _context.Materiels.Add(materiel);
            await _context.SaveChangesAsync();
            TempData["success"] = "Matériel modifié avec succès";

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var materiel = _context.Materiels.Find(id);

            if (materiel == null)
                return RedirectToAction("Index", "Materiel");

            var materielDto = new MaterielDto()
            {
                NomMateriel = materiel.NomMateriel,
                Marque = materiel.Marque,
                Categorie = materiel.Categorie,
                NumeroSerie = materiel.NumeroSerie,
                Etat = materiel.Etat,
                PrixParJour = materiel.PrixParJour,
                Disponibilitee = materiel.Disponibilitee,
            };

            ViewData["MaterielId"] = materiel.Id;
            ViewData["DateCreation"] = materiel.DateCreation.ToString("dd/MM/yyyy");


            return View(materielDto);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(int id, MaterielDto materielDto)
        {
            var materiel = _context.Materiels.Find(id);

            if (materiel == null)
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
            {
                ViewData["MaterielId"] = materiel.Id;
                ViewData["DateCreation"] = materiel.DateCreation.ToString("dd/MM/yyyy");

                return View(materielDto);
            }

            materiel.NomMateriel = materielDto.NomMateriel;
            materiel.Marque = materielDto.Marque;
            materiel.Categorie = materielDto.Categorie;
            materiel.PrixParJour = materielDto.PrixParJour;
            materiel.NumeroSerie = materielDto.NumeroSerie;
            materiel.Etat = materielDto.Etat;
            materiel.Disponibilitee = materielDto.Disponibilitee;

            _context.SaveChanges();

            TempData["success"] = "Matériel modifié avec succès";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var materiel = _context.Materiels.Find(id);

            if (materiel == null)
                return RedirectToAction("Index", "Materiel");

            materiel.ArchivateAt = DateTime.Now;
            _context.SaveChanges(true);
            TempData["success"] = "Matériel archivé avec succès";

            return RedirectToAction("Index", "Materiel");
        }
    }
}
