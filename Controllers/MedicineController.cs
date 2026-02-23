using Hastane_Otomasyon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hastane_Otomasyon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicinesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MedicinesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicines()
        {
            try
            {
                var medicines = await _context.Medicine.ToListAsync();
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Veritabaný hatasý: {ex.Message}");
            }
        }
    }
}