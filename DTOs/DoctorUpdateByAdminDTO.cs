namespace Hastane_Otomasyon.DTOs
{
    public class DoctorUpdateByAdminDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int DepartmentID { get; set; }
    }
}