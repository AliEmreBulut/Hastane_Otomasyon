using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hastane_Otomasyon.Models
{
    [Table("department")]
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int DepartmentID { get; set; }

        [Required]
        [StringLength(100)]
        public required string DepartmentName { get; set; }

        [JsonIgnore]    
        public ICollection<Doctor> Doctors { get; set; }

    }
}
