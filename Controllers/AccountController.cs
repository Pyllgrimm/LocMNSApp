using LocMNSApp.DTOs;
using LocMNSApp.Models;
using LocMNSApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocMNSApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Utilisateur> _signInManager;
        private readonly UserManager<Utilisateur> _userManager;
        

        public AccountController(SignInManager<Utilisateur> signInManager, UserManager<Utilisateur> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            
        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.SeSouvenirDeMoi, false);

                if (result.Succeeded)
                {
                    List<Claim> claims = new List<Claim>() 
                    {
                        new Claim(ClaimTypes.NameIdentifier, model.Username),
                        new Claim("OtherProperties", "Example Role")
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = model.SeSouvenirDeMoi
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Login invalide");
                return View(model);
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Utilisateur utilisateur = new()
                {
                    Nom = model.Nom,
                    Prenom = model.Prenom,
                    UserName = model.Email,
                    Email = model.Email,
                    Adresse = model.Adresse,
                    CodePostal = model.CodePostal,
                    Ville = model.Ville,
                    Promotion = model.Promotion,
                    PhoneNumber = model.Telephone,
                    DateEnregistrement = DateTime.Now,
                    Locations = new List<Location>()
                };
            
                if (utilisateur.Email.Equals("admin.admin@gestionlocmns.fr"))
                {
                    var result = await _userManager.CreateAsync(utilisateur, model.Password!);

                    
                    await _userManager.AddToRoleAsync(utilisateur, "Admin");


                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(utilisateur, false);

                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else if (utilisateur.Email.Equals($"{utilisateur.Prenom.ToLower()}.{utilisateur.Nom.ToLower()}@formateursifa.fr") ||
                         utilisateur.Email.Equals($"{utilisateur.Prenom.ToLower()}.{utilisateur.Nom.ToLower()}@metznumericschool.fr") ||
                         utilisateur.Email.Equals($"{utilisateur.Prenom.ToLower()}.{utilisateur.Nom.ToLower()}@stagiairesmns.fr") ||
                         utilisateur.Email.Equals($"{utilisateur.Prenom.ToLower()}.{utilisateur.Nom.ToLower()}@stagiairesifa.fr"))
                {

                    var result = await _userManager.CreateAsync(utilisateur, model.Password!);

                    await _userManager.AddToRoleAsync(utilisateur, "Utilisateur");

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(utilisateur, false);
                        TempData["success"] = "Compte créé avec succès";

                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email non accepté");
                    return View(model);
                }

            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

        


    }
}
