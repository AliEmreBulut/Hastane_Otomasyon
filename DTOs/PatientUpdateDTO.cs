namespace Hastane_Otomasyon.DTOs
{
    public class PatientUpdateDTO
    {
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
    }
}