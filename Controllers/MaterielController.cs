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
            var materiels = _context.Materiels.ToList();
            return View(materiels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(MaterielDto materielDto)
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
            _context.SaveChanges();

            return RedirectToAction("Index", "Materiel");
        }

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
            };

            ViewData["MaterielId"] = materiel.Id;
            ViewData["DateCreation"] = materiel.DateCreation.ToString("dd/MM/yyyy");

            return View(materielDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, MaterielDto materielDto)
        {
            var materiel = _context.Materiels.Find(id);

            if (materiel == null)
                return RedirectToAction("Index", "Materiel");

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

            _context.SaveChanges();

            return RedirectToAction("Index", "Materiel");
        }

        public IActionResult Delete(int id)
        {
            var materiel = _context.Materiels.Find(id);

            if (materiel == null)
                return RedirectToAction("Index", "Materiel");

            _context.Materiels.Remove(materiel);
            _context.SaveChanges(true);

            return RedirectToAction("Index", "Materiel");
        }
    }
}
