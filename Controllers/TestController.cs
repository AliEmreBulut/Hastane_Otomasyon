using Microsoft.AspNetCore.Mvc;

namespace Hastane_Otomasyon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TestController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("dbcheck")]
        public IActionResult CheckDatabase()
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");
            return Ok(new
            {
                ConnectionString = connectionString,
                Message = "Bağlantı dizesini kontrol edin"
            });
        }
    }
}
