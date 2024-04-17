using System.ComponentModel.DataAnnotations;

namespace LocMNSApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string? Nom { get; set; }
        [Required]
        public string? Prenom { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Ne correspond pas")]
        [Display(Name = "Comfirmez Password")]
        [DataType(DataType.Password)]
        public string? ComfirmPassword { get; set; }
    }
}
