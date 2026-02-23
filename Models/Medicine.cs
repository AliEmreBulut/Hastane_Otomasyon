using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hastane_Otomasyon.Models
{
    [Table("medicine")]
    public class Medicine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int MedicineID { get; set; }

        [Required]
        [StringLength(50)]
        public required string MedicineName { get; set; }
        public required string Description { get; set; }

        public required ICollection<PrescriptionMedicine > Prescriptions { get; set; }
    }
}
