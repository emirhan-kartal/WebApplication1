using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class RegisterModel
    {
        [Display(Name = "Adı Soyadı")]
        [Required(ErrorMessage = "Adı Soyadı Giriniz!")]
        public string FullName { get; set; }




        [Display(Name = "E-Posta Adresi")]
        [Required(ErrorMessage = "E-Posta Adresi Giriniz!")]
        [EmailAddress(ErrorMessage = "Geçerli bir E-Posta Adresi Giriniz!")]
        public string Email { get; set; }



        [Display(Name = "Parola")]
        [Required(ErrorMessage = "Parola Giriniz!")]
        public string Password { get; set; }



        [Display(Name = "Parola Tekrar")]
        [Required(ErrorMessage = "Parola Tekrar Giriniz!")]
        [Compare("Password", ErrorMessage = "Parolalar Eşleşmiyor!")]
        public string PasswordConfirm { get; set; }
    }
}
