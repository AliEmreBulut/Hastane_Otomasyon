using System.ComponentModel.DataAnnotations;

namespace Hastane_Otomasyon.DTOs
{
    public class DoctorCreateDTO
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçersiz email formatı")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Geçersiz telefon numarası formatı")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departman ID zorunludur")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçersiz departman ID")]
        public int DepartmentID { get; set; }

    }
}