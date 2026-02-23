using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hastane_Otomasyon.Models
{
    [Table("prescription")]
    public class Prescription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int PrescriptionID { get; set; }

        public DateTime Date { get; set; }
        public required string Diagnosis {  get; set; }
        public required string Description { get; set; }


        public int AppointmentID { get; set; }
        public required Appointment Appointment { get; set; }


        public required ICollection<PrescriptionMedicine> prescription_Medicines { get; set; }

    }
}
