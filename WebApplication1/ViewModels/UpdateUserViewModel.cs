using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class UpdateUserViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Adı Soyadı")]
        [Required(ErrorMessage = "Adı Soyadı Giriniz!")]
        public string FullName { get; set; }

        [Display(Name = "E-Posta Adresi")]
        [Required(ErrorMessage = "E-Posta Adresi Giriniz!")]
        [EmailAddress(ErrorMessage = "Geçerli bir E-Posta Adresi Giriniz!")]
        public  string Email { get; set; }

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "Rol Giriniz!")]
        public string Role { get; set; }
        public bool isSuperAdmin { get; set; }
        public int UploadedFilesCount { get; set; }
    }
}
