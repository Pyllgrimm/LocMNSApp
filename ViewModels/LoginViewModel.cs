using System.ComponentModel.DataAnnotations;

namespace LocMNSApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'identifiant est requis .")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis .")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Se souvenir de moi")]
        public bool SeSouvenirDeMoi { get; set; }
    }
}
