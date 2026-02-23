using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hastane_Otomasyon.Models
{
    [Table("doctor")]
    public class Doctor : User
    {

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }

        public int DepartmentID { get; set; }



        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;


        [Phone]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public override string GetFullName() => $"{FirstName} {LastName}".Trim();

        public override string GetRoleName() => "Doctor";
        public override int GetRoleId() => 2;
    }
}