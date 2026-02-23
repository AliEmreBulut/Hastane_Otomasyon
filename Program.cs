using Hastane_Otomasyon.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string bulunamadı! appsettings.json kontrol edin.");
}

Console.WriteLine($"Kullanılan bağlantı dizesi: {connectionString}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(11, 4, 2)),
        mysqlOptions =>
        {
            mysqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null); // null olarak ayarlandı
        }
    ));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Bağlantı testi ve veritabanı işlemleri
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Veritabanı bağlantısı deneniyor...");

        // Manuel bağlantı testi
        await using (var connection = db.Database.GetDbConnection())
        {
            await connection.OpenAsync();
            logger.LogInformation($"Veritabanı durumu: {connection.State}");
            logger.LogInformation($"Sunucu versiyonu: {connection.ServerVersion}");

            // Tabloları listele
            var command = connection.CreateCommand();
            command.CommandText = "SHOW TABLES";

            var tableNames = new List<string>();
            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    tableNames.Add(reader.GetString(0));
                }
            }

            logger.LogInformation($"Mevcut tablolar: {string.Join(", ", tableNames)}");

            if (tableNames.Any())
            {
                command.CommandText = $"SELECT COUNT(*) FROM `{tableNames.First()}`";
                var count = await command.ExecuteScalarAsync();
                logger.LogInformation($"'{tableNames.First()}' tablosunda {count} kayıt bulundu");
            }
        }
    }
    catch (DbException dbEx)
    {
        logger.LogError($"Veritabanı hatası ({dbEx.GetType().Name}): {dbEx.Message}");
        logger.LogError($"SQL hatası: {dbEx.ErrorCode}");
        logger.LogError($"Stack Trace: {dbEx.StackTrace}");
    }
    catch (Exception ex)
    {
        logger.LogError($"Genel hata: {ex.Message}");
        logger.LogError($"Stack Trace: {ex.StackTrace}");
    }
}

    // Middleware'ler...
    app.UseDefaultFiles(); 
    app.UseStaticFiles();  

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.MapFallbackToFile("/giris.html");
    app.Run();