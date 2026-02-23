using BCrypt.Net;
using Hastane_Otomasyon.Data;
using Hastane_Otomasyon.DTOs;
using Hastane_Otomasyon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] PatientRegisterDTO dto)
    {

        var existingPatient = await _context.Patient
            .FirstOrDefaultAsync(p => p.NationalID == dto.NationalID);
        if (existingPatient != null)
            return BadRequest("Bu TC No ile zaten kayıtlı bir hasta var.");

        var patient = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            NationalID = dto.NationalID,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            BloodType = dto.BloodType,
            Gender = dto.Gender,
            UserName = dto.UserName,
            BirthDate = dto.BirthDate, // bu sıkıntı çıkarıyor

            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleID = 3,
            Appointments = new List<Appointment>()
        };

        _context.Patient.Add(patient);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Kayıt başarılı!", Patient = patient });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] Patient login)
    {
        var patient = await _context.Patient
            .FirstOrDefaultAsync(p => p.NationalID == login.NationalID);

        if (patient == null || !BCrypt.Net.BCrypt.Verify(login.Password, patient.Password))
            return Unauthorized("TcNo veya şifre yanlış.");

        return Ok(new { Message = "Giriş başarılı!", PatientId = patient.UserID });
    }

    [HttpGet("MyProfile/{patientId}")]
    public async Task<IActionResult> GetMyProfile(int patientId)
    {
        var patient = await _context.Patient.FindAsync(patientId);
        if (patient == null)
            return NotFound();

        return Ok(patient);
    }

    [HttpGet("appointments/{patientId}")]
    public async Task<IActionResult> GetAppointmentsByPatient(int patientId)
    {
        var appointments = await _context.Appointment
            .Where(a => a.PatientID == patientId)
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Department)
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
                doctor = new
                {
                    fullName = a.Doctor.FirstName + " " + a.Doctor.LastName,
                    department = a.Doctor.Department.DepartmentName
                },
                prescription = a.Prescription != null ? new
                {
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
    [HttpPost("TakeAppointment")]
    public async Task<IActionResult> TakeAppointment([FromBody] AppointmentCreateDTO request)
    {
        try
        {
            // Validation
            if (request == null)
                return BadRequest("İstek verisi boş olamaz.");

            if (request.PatientId <= 0 || request.DoctorId <= 0)
                return BadRequest("Hasta ve doktor ID geçerli olmalı.");

            
            var doctor = await _context.Doctor
                .FirstOrDefaultAsync(d => d.UserID == request.DoctorId);

            if (doctor == null)
            {
                return NotFound(new
                {
                    Message = "Doktor bulunamadı",
                    DoctorUserId = request.DoctorId,
                    Debug = "Doctor tablosunda bu UserID'ye sahip kayıt yok"
                });
            }

            var patient = await _context.Patient
                .FirstOrDefaultAsync(p => p.UserID == request.PatientId);

            if (patient == null)
            {
                return NotFound(new
                {
                    Message = "Hasta bulunamadı",
                    PatientUserId = request.PatientId,
                    Debug = "Patient tablosunda bu UserID'ye sahip kayıt yok"
                });
            }

            // Save UserIDs to appointment table
            int doctorIdForAppointment = doctor.UserID;
            int patientIdForAppointment = patient.UserID;

            // Debug log
            Console.WriteLine("=== RANDEVU OLUŞTURMA DEBUG ===");
            Console.WriteLine($"Frontend'den gelen DoctorId: {request.DoctorId}");
            Console.WriteLine($"Frontend'den gelen PatientId: {request.PatientId}");
            Console.WriteLine($"Appointment'a kaydedilecek DoctorID: {doctorIdForAppointment}");
            Console.WriteLine($"Appointment'a kaydedilecek PatientID: {patientIdForAppointment}");
            Console.WriteLine($"Date: {request.Date:yyyy-MM-dd}");
            Console.WriteLine($"Time: {request.Time}");

            // conflict control
            var conflict = await _context.Appointment
                .AnyAsync(a =>
                    a.DoctorID == doctorIdForAppointment &&
                    a.Date == request.Date.Date &&
                    a.Time == request.Time &&
                    a.Status != "Cancelled");

            if (conflict)
            {
                Console.WriteLine("ÇAKIŞMA VAR!");
                return BadRequest("Bu saat dolu. Lütfen başka bir zaman seçin.");
            }

            // Create appointmen
            var appointment = new Appointment
            {
                PatientID = patientIdForAppointment,
                DoctorID = doctorIdForAppointment,
                Date = request.Date.Date,
                Time = request.Time,
                Status = "Onaylandı"
            };

            Console.WriteLine($"Oluşturulan Appointment: PatientID={appointment.PatientID}, DoctorID={appointment.DoctorID}");

            _context.Appointment.Add(appointment);

            try
            {
                Console.WriteLine("SaveChangesAsync çağrılıyor...");
                await _context.SaveChangesAsync();
                Console.WriteLine("SaveChanges başarılı!");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine("=== DATABASE HATASI ===");
                Console.WriteLine($"Mesaj: {dbEx.Message}");
                Console.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");

                var innerMsg = dbEx.InnerException?.Message ?? dbEx.Message;

                // Foreign key conrtorl
                if (innerMsg.Contains("Cannot add or update a child row"))
                {
                    if (innerMsg.Contains("DoctorID"))
                        return BadRequest($"DoctorID={doctorIdForAppointment} doctor tablosunda bulunamadı. Foreign key hatası.");
                    if (innerMsg.Contains("PatientID"))
                        return BadRequest($"PatientID={patientIdForAppointment} patient tablosunda bulunamadı. Foreign key hatası.");
                }

                return BadRequest(new
                {
                    Message = "Veritabanı hatası",
                    Error = innerMsg,
                    DoctorId = doctorIdForAppointment,
                    PatientId = patientIdForAppointment
                });
            }

            return Ok(new
            {
                Message = "Randevunuz başarıyla alındı!",
                AppointmentID = appointment.AppointmentID,
                Status = "Onaylandı"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HATA] {ex.Message}");
            return StatusCode(500, new
            {
                Message = "Beklenmeyen hata oluştu.",
                Error = ex.Message
            });
        }
    }
    [HttpGet("AvailableSlots/{doctorId:int}/{date}")]
    public async Task<IActionResult> AvailableSlots(int doctorId, string date)
    {
        if (!DateTime.TryParse(date, out var day))
            return BadRequest("Tarih formatı yyyy-MM-dd olmalı");

        var start = new TimeSpan(9, 0, 0);
        var end = new TimeSpan(17, 0, 0);
        var step = TimeSpan.FromMinutes(30);

        // Booked hours
        var booked = await _context.Appointment
            .Where(a => a.DoctorID == doctorId &&
                        a.Date == day.Date &&
                        a.Status != "Cancelled")
            .Select(a => a.Time)
            .ToListAsync();

        var slots = new List<string>();
        for (var t = start; t < end; t += step)
            if (!booked.Contains(t))
                slots.Add($"{t.Hours:D2}:{t.Minutes:D2}");

        return Ok(slots);
    }

    [HttpPut("CancelAppointment/{appointmentId}")]
    public async Task<IActionResult> CancelAppointment(int appointmentId)
    {
        var appointment = await _context.Appointment.FindAsync(appointmentId);

        if (appointment == null)
            return NotFound("Randevu bulunamadı.");

        if (appointment.Status == "Tamamlandı")
            return BadRequest("Tamamlanmış bir randevuyu iptal edemezsiniz.");

        // Durumu güncelliyoruz
        appointment.Status = "İptal Edildi";

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Randevunuz başarıyla iptal edildi." });
    }
 
    [HttpPut("UpdateProfile/{patientId}")]
    public async Task<IActionResult> UpdateProfile(int patientId, [FromBody] Hastane_Otomasyon.DTOs.PatientUpdateDTO dto)
    {
        var patient = await _context.Patient.FirstOrDefaultAsync(p => p.UserID == patientId);

        if (patient == null)
            return NotFound(new { Message = "Hasta bulunamadı." });

        patient.Email = dto.Email;
        patient.PhoneNumber = dto.PhoneNumber;
        patient.Address = dto.Address;
        patient.BloodType = dto.BloodType;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Profil başarıyla güncellendi." });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Hata: " + ex.Message);
            return StatusCode(500, "Veritabanı güncelleme hatası: " + ex.Message);
        }
    }
}