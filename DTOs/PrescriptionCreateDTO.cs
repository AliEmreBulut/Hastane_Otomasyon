namespace Hastane_Otomasyon.DTOs
{
    public class PrescriptionCreateDTO
    {
        public string Diagnosis { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<PrescriptionMedicineDTO> Prescription_Medicines { get; set; } = new();
    }

    public class PrescriptionMedicineDTO
    {
        public int MedicineID { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}