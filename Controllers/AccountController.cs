using LocMNSApp.Models;
using LocMNSApp.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.SeSouvenirDeMoi, false);

                if (result.Succeeded)
                {
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
                    DateEnregistrement = DateTime.Now
                };



                if (utilisateur.Email.Equals($"{utilisateur.Prenom}.{utilisateur.Nom}@metznumericschool.fr"))
                {
                    var result = await _userManager.CreateAsync(utilisateur, model.Password!);

                    await _userManager.AddToRoleAsync(utilisateur, "Admin");

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(utilisateur, false);

                        return RedirectToAction("Login", "Account");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else if (utilisateur.Email.Equals($"{utilisateur.Prenom}.{utilisateur.Nom}@formateursifa.fr"))
                {

                    var result = await _userManager.CreateAsync(utilisateur, model.Password!);

                    await _userManager.AddToRoleAsync(utilisateur, "Formateur");

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(utilisateur, false);

                        return RedirectToAction("Login", "Account");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else if (utilisateur.Email.Equals($"{utilisateur.Prenom}.{utilisateur.Nom}@stagiairesmns.fr") || utilisateur.Email.Equals($"{utilisateur.Prenom}.{utilisateur.Nom}@stagiairesifa.fr"))
                {
                    var result = await _userManager.CreateAsync(utilisateur, model.Password!);
                    await _userManager.AddToRoleAsync(utilisateur, "Stagiaire");

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(utilisateur, false);

                        return RedirectToAction("Login", "Account");
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
