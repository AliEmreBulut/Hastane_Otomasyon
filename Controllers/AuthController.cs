using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hastane_Otomasyon.Data;
using Hastane_Otomasyon.Models;

namespace Hastane_Otomasyon.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public AuthController(ApplicationDbContext db) => _db = db;

    [HttpGet("me")]
    public async Task<IActionResult> Me([FromQuery] int userId)
    {
        var user = await _db.User.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserID == userId);
        if (user == null) return NotFound("Kullanıcı bulunamadı");

        var role = user.GetRoleName();

        string? fullName = null;
        try { fullName = user.GetFullName(); } catch { }

        
        var dto = new
        {
            userId = user.UserID,
            role,
            fullName = (string?)null,
            doctorId = (int?)null,
            email = (string?)null,
            phone = (string?)null,
            department = (string?)null,
            patientId = (int?)null,
            gender = (string?)null,
            birthDate = (DateTime?)null,
            nationalId = (string?)null,
            address = (string?)null
        };

        // Doctor ise
        if (role == "Doctor")
        {
            var d = await _db.Doctor
                .Include(x => x.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserID == user.UserID);

            return Ok(new
            {
                dto.userId,
                dto.role,
                fullName = d?.GetFullName() ?? fullName ?? user.UserName,
                doctorId = d?.UserID, 
                email = d?.Email,
                phone = d?.PhoneNumber,
                department = d?.Department?.DepartmentName,
                dto.patientId,
                dto.gender,
                dto.birthDate,
                dto.nationalId,
                dto.address
            });
        }

        // Patient ise
        if (role == "Patient")
        {
            var p = await _db.Patient
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserID == user.UserID);

            var fullNameP = (p != null ? $"{p.FirstName} {p.LastName}".Trim() : null) ?? fullName ?? user.UserName;
            var emailP = p?.Email;        
            var phoneP = p?.PhoneNumber;  

            return Ok(new
            {
                userId = user.UserID,
                role,
                fullName = fullNameP,
                doctorId = (int?)null,
                department = (string?)null,
                patientId = p?.UserID,
                gender = p?.Gender,
                birthDate = p?.BirthDate,
                nationalId = p?.NationalID,
                address = p?.Address,
                email = emailP,
                phone = phoneP,
                bloodType = p?.BloodType
            });
        }

        return Ok(new
        {
            dto.userId,
            dto.role,
            fullName = fullName ?? user.UserName,
            dto.doctorId,
            dto.email,
            dto.phone,
            dto.department,
            dto.patientId,
            dto.gender,
            dto.birthDate,
            dto.nationalId,
            dto.address
        });
    }

}
