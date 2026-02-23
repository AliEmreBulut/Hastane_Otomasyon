using System.ComponentModel.DataAnnotations;

namespace Hastane_Otomasyon.DTOs
{
    public class AppointmentCreateDTO
    {
        [Required(ErrorMessage = "Hasta ID zorunludur")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Doktor ID zorunludur")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Saat zorunludur")]
        public TimeSpan Time { get; set; }
    }
}