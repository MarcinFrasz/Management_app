using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Management_application.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Kod")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Zapamiętaj tą przeglądarkę?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email jest wymagany.")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage ="Błędny email lub hasło.")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj mnie?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage="Email jest wymagany.")]
        [EmailAddress(ErrorMessage ="Błędny format Email.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Imię"),Required(ErrorMessage ="Imię jest wymagane.")]
        [RegularExpression(@"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]*$", ErrorMessage = "Imię może zawierać tylko [A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ].")]
        public string Name { get; set; }
        [Display(Name = "Nazwisko"), Required(ErrorMessage ="Nazwisko jest wymagane")]
        [RegularExpression(@"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]*$", ErrorMessage = "Nazwisko może zawierać tylko [A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ].")]
        public string Surname { get; set; }
        [RegularExpression(@"^[0-9]{6}$",ErrorMessage="Proszę wprowadzić indeks w poprawnym formacie")]
        [Display(Name = "Nr_indeksu")]
        public string Index_number { get; set; }

        [Required(ErrorMessage ="Hasło jest wymagane.")]
        [StringLength(100, ErrorMessage = "{0} musi składać się z conajmniej {2} znaków.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdz hasło")]
        [Compare("Password", ErrorMessage = "Hasło i potwierdzenie hasła się nie zgadzają.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musi składać się z conajmniej {2} znaków.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdz hasło")]
        [Compare("Password", ErrorMessage = "Hasło i potwierdzenie hasła się nie zgadzają.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
