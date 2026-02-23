using System.ComponentModel.DataAnnotations;

namespace Hastane_Otomasyon.DTOs
{
    public class PatientRegisterDTO
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad zorunludur")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "TC Kimlik zorunludur")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik 11 haneli olmalıdır")]
        public string NationalID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçersiz email formatı")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Doğum tarihi zorunludur")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Adres zorunludur")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kan grubu zorunludur")]
        public string BloodType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cinsiyet zorunludur")]
        public string Gender { get; set; } = string.Empty;
    }
}