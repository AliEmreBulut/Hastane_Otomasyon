using Hastane_Otomasyon.DTOs;
using Hastane_Otomasyon.Data;
using Hastane_Otomasyon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hastane_Otomasyon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }


        // Doktorun tüm hastalarını etir
        [HttpGet("patients/{doctorId}")]
        public async Task<IActionResult> GetPatientsByDoctor(int doctorId)
        {
            var patients = await _context.Appointment
                .Where(a => a.DoctorID == doctorId)
                .Include(a => a.Patient)
                .Select(a => a.Patient)
                .Distinct()
                .ToListAsync();

            return Ok(patients);
        }

        // Doktorun randevularını getir
        [HttpGet("appointments/{doctorId}")]
        public async Task<IActionResult> GetAppointmentsByDoctor(int doctorId)
        {
            var appointments = await _context.Appointment
                .Where(a => a.DoctorID == doctorId)
                .Include(a => a.Patient)
                .Include(a => a.Prescription)
                    .ThenInclude(p => p.prescription_Medicines) 
                        .ThenInclude(pm => pm.Medicine) 
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.Time)
                .Select(a => new
                {
                    appointmentID = a.AppointmentID,
                    date = a.Date.ToString("yyyy-MM-dd"),
                    time = a.Time.ToString(@"hh\:mm"),
                    status = a.Status,
                    patient = new
                    {
                        userID = a.Patient.UserID,
                        firstName = a.Patient.FirstName,
                        lastName = a.Patient.LastName,
                        nationalID = a.Patient.NationalID,
                        phone = a.Patient.PhoneNumber,
                        bloodType = a.Patient.BloodType
                    },
                    prescription = a.Prescription != null ? new
                    {
                        id = a.Prescription.PrescriptionID,
                        diagnosis = a.Prescription.Diagnosis,
                        description = a.Prescription.Description,
                      
                        medicines = a.Prescription.prescription_Medicines.Select(pm => new
                        {
                            medicineID = pm.MedicineID,
                            name = pm.Medicine != null ? pm.Medicine.MedicineName : "Bilinmeyen İlaç",
                            usage = pm.Description
                        }).ToList()
                    } : null
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // Doktorun belirli bir gün için boş ve dolu saatlerini gör
        [HttpGet("schedule/{doctorId}/{date}")]
        public async Task<IActionResult> GetDoctorSchedule(int doctorId, DateTime date)
        {
            // Dolular
            var doluSaatler = await _context.Appointment
                .Where(a => a.DoctorID == doctorId && a.Date == date.Date)
                .Select(a => a.Time)
                .ToListAsync();

            List<TimeSpan> bosSaatler = new List<TimeSpan>();
            List<TimeSpan> tumSaatler = new List<TimeSpan>();

            // Örnek çalışma saatleri 09:00 - 17:00
            for (int saat = 9; saat < 17; saat++)
            {
                TimeSpan ts = new TimeSpan(saat, 0, 0);
                tumSaatler.Add(ts);
                if (!doluSaatler.Contains(ts))
                    bosSaatler.Add(ts);
            }

            return Ok(new
            {
                Date = date.Date,
                AllHours = tumSaatler,
                BusyHours = doluSaatler,
                FreeHours = bosSaatler
            });
        }

        [HttpPost("AddPrescription/{appointmentId}")]
        public async Task<IActionResult> AddPrescription(int appointmentId, [FromBody] PrescriptionCreateDTO request)
        {
            var appointment = await _context.Appointment
                                            .FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);

            if (appointment == null)
                return NotFound(new { Message = "Randevu bulunamadı." });

            appointment.Status = "Tamamlandı";

            var prescription = new Prescription
            {
                AppointmentID = appointmentId,
                Diagnosis = request.Diagnosis,
                Description = request.Description,
                Date = DateTime.Now,

                Appointment = appointment,
                prescription_Medicines = new List<PrescriptionMedicine>()
            };

            _context.Prescription.Add(prescription);
            await _context.SaveChangesAsync();

            if (request.Prescription_Medicines != null && request.Prescription_Medicines.Any())
            {
                foreach (var med in request.Prescription_Medicines)
                {
                    var pm = new PrescriptionMedicine
                    {
                        PrescriptionID = prescription.PrescriptionID,
                        MedicineID = med.MedicineID,
                        Description = med.Description,

                        Prescription = null!,
                        Medicine = null!
                    };

                    _context.Add(pm);
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new { Message = "Reçete ve ilaçlar başarıyla kaydedildi!" });
        }

        [HttpPut("UpdateProfile/{doctorId}")]
        public async Task<IActionResult> UpdateProfile(int doctorId, [FromBody] DoctorUpdateDTO dto)
        {
            var doctor = await _context.Doctor.FirstOrDefaultAsync(d => d.UserID == doctorId);
            if (doctor == null)
                return NotFound(new { Message = "Doktor bulunamadı." });

            doctor.Email = dto.Email;
            doctor.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Profil güncellendi." });
        }
    }
}