using System.ComponentModel.DataAnnotations.Schema;

namespace Hastane_Otomasyon.Models
{
    [Table("prescription_medicine")]
    public class PrescriptionMedicine
    {
        public int PrescriptionID { get; set; }
        public required Prescription Prescription { get; set; }

        public int MedicineID { get; set; }
        public required Medicine Medicine { get; set; }

        
        public required string Description { get; set; }
        
    }
}
