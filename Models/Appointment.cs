using Hastane_Otomasyon.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hastane_Otomasyon.Models
{
    
    [Table("appointment")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentID { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }

        private string _status = "Onaylandı";

        public string Status
        {
            get
            {
                if (Prescription != null) return "Tamamlandı";

                var appointmentDateTime = Date.Date.Add(Time);

                if (_status != "Cancelled" && _status != "İptal Edildi" && appointmentDateTime < DateTime.Now)
                {
                    return "Tamamlandı";
                }

                return _status;
            }
            set => _status = value;
        }

        public int DoctorID { get; set; }
        public virtual Doctor Doctor { get; set; }

        public int PatientID { get; set; }
        public virtual Patient Patient { get; set; }

        public virtual Prescription? Prescription { get; set; }
    }
}
