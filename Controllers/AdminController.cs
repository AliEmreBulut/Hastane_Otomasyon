using Microsoft.AspNetCore.Mvc;
using Hastane_Otomasyon.Data;
using Hastane_Otomasyon.Models;
using Microsoft.EntityFrameworkCore;
using Hastane_Otomasyon.DTOs;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;


namespace Hastane_Otomasyon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [HttpPost("setup-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateDefaultAdmin()
        {
            var adminExists = await _context.User.AnyAsync(u => u.UserName == "admin");
            if (adminExists)
            {
                return BadRequest("Admin kullanıcısı zaten veritabanında mevcut!");
            }

            var defaultAdmin = new Admin
            {
                UserName = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                RoleID = 1 
            };

            _context.User.Add(defaultAdmin);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Sistem Yöneticisi başarıyla oluşturuldu!",
                KullaniciAdi = "admin",
                Sifre = "admin123"
            });
        }

        // Get all users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.User
                .Include(u => u.Role)
                .ToListAsync();
            return Ok(users);
        }

        // Get all patients
        [HttpGet("patients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _context.Patient
                .Include(p => p.Role) 
                .ToListAsync();
            return Ok(patients);
        }
        // Get patient by ID
        [HttpGet("patient/{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }
            return Ok(patient);
        }


        //Hupdate Patient
        [HttpPut("patient/{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient updatedPatient)
        {
            if (id != updatedPatient.UserID)
                return BadRequest("ID uyuşmuyor.");

            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
                return NotFound();

            patient.FirstName = updatedPatient.FirstName;
            patient.LastName = updatedPatient.LastName;
            patient.NationalID = updatedPatient.NationalID;
           

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Hasta bilgileri güncellendi.", Patient = patient });
        }

        [HttpDelete("patient/{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
                return NotFound();

            _context.Patient.Remove(patient);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Hasta kaydı silindi." });
        }

        // Get all doctors
        [HttpGet("doctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _context.Doctor
                .Include(d => d.Department)
                .Include(d => d.Role)
                .ToListAsync();
            return Ok(doctors);
        }

        // Get doctor by ID
        [HttpGet("doctor/{id}")]
        public async Task<IActionResult> GetDoctor(int id)
        {
            var doctor = await _context.Doctor
                .Include(d => d.Department)
                .Include(d => d.Role)
                .FirstOrDefaultAsync(d => d.UserID == id);

            if (doctor == null)
            {
                return NotFound("Doctor not found");
            }

            return Ok(doctor);
        }

        // Get all appointments
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _context.Appointment
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Department)
                .ToListAsync();

            return Ok(appointments);
        }

        // Create new doctor
        [HttpPost("doctor")]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            

            // Check if username already exists
            if (await _context.User.AnyAsync(u => u.UserName == dto.UserName))
            {
                return BadRequest("Username already exists");
            }

           

            // Get doctor role
            var doctorRole = await _context.Role.FindAsync(2);
            if (doctorRole == null)
            {
                return StatusCode(500, "Doctor role not found in database");
            }

            var doctor = new Doctor
            {
                UserName = dto.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DepartmentID = dto.DepartmentID,
                Role = doctorRole,
                RoleID = doctorRole.RoleID,
                
            };

            _context.Doctor.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.UserID }, doctor);
        }

        // Delete Doctor by ID
        [HttpDelete("doctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            
            var doctor = await _context.Doctor.FirstOrDefaultAsync(d => d.UserID == id);
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            

            var futureAppointments = await _context.Appointment
                .Where(a => a.DoctorID == id && a.Date >= DateTime.Today && a.Status == "Onaylandı")
                .ToListAsync();

            foreach (var appt in futureAppointments)
            {
                appt.Status = "İptal Edildi";

            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Doktor başarıyla pasife çekildi ve bekleyen randevuları iptal edildi." });
        }

        [HttpPut("doctor/{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorUpdateByAdminDTO dto)
        {
            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor == null) return NotFound("Doktor bulunamadı.");

            doctor.FirstName = dto.FirstName;
            doctor.LastName = dto.LastName;
            doctor.Email = dto.Email;
            doctor.PhoneNumber = dto.PhoneNumber;
            doctor.DepartmentID = dto.DepartmentID;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Doktor bilgileri Admin tarafından güncellendi." });
        }





        // Delete appointment
        [HttpDelete("appointment/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();
            return Ok("Appointment deleted successfully");
        }

        [HttpGet("generate-hash/{password}")]
        [AllowAnonymous]
        public IActionResult GenerateHash(string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            return Ok(new { hash });
        }


    }
}