using Hastane_Otomasyon.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hastane_Otomasyon.Models
{
    [Table("patient")]
    public class Patient : User
    {

        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required, StringLength(11)]
        public required string NationalID{ get; set; }

        [Column(TypeName = "date")]
        public DateTime BirthDate { get; set; }

        [Required]
        public required string Address { get; set; }

        [Required]
        public required string PhoneNumber { get; set; }

        [Required]
        public required string BloodType { get; set; }
        

        [Required]
        public required string Gender { get; set; }

        public  ICollection<Appointment> Appointments { get; set; }

        public override string GetFullName()
        {
            return $"{FirstName} {LastName}".Trim();
        }
        public override string GetRoleName() => "Patient";
        public override int GetRoleId() => 3;
    }
}