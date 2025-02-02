using BookingApp.Models;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IStatisticiService _statisticiService;
        private readonly IExportService _exportService;
        private readonly IAuthService _authService;
        private readonly IServiciuEmail _serviciuEmail;

        public AdminController(
            IStatisticiService statisticiService,
            IExportService exportService,
            IAuthService authService,
            IServiciuEmail serviciuEmail)
        {
            _statisticiService = statisticiService;
            _exportService = exportService;
            _authService = authService;
            _serviciuEmail = serviciuEmail;
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            try
            {
                var statistici = await _statisticiService.ObtineStatisticiDashboardAsync();

                // ✅ Trimitem notificare prin e-mail administratorului
                await _serviciuEmail.TrimiteEmailAsync(
                    "contact.aquarios@gmail.com",
                    "Dashboard accesat",
                    "<p>Dashboard-ul administratorului a fost accesat.</p>"
                );

                return Ok(statistici);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la obținerea statisticilor: {ex.Message}" });
            }
        }

        [HttpGet("ExportRezervariCsv")]
        public async Task<IActionResult> ExportRezervariCsv()
        {
            var csvData = await _exportService.ExportaRezervariCsvAsync();

            // ✅ Trimitem notificare prin e-mail administratorului
            await _serviciuEmail.TrimiteEmailAsync(
                "contact.aquarios@gmail.com",
                "Export CSV finalizat",
                "<p>Un raport CSV a fost generat și descărcat.</p>"
            );

            return File(csvData, "text/csv", "rezervari.csv");
        }

        [HttpGet("ExportRezervariExcel")]
        public async Task<IActionResult> ExportRezervariExcel()
        {
            var excelData = await _exportService.ExportaRezervariExcelAsync();

            // ✅ Trimitem notificare prin e-mail administratorului
            await _serviciuEmail.TrimiteEmailAsync(
                "contact.aquarios@gmail.com",
                "Export Excel finalizat",
                "<p>Un raport Excel a fost generat și descărcat.</p>"
            );

            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rezervari.xlsx");
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            try
            {
                var success = await _authService.UpdateUserAsync(user);
                if (!success)
                {
                    return StatusCode(500, new { Mesaj = "A apărut o eroare la actualizarea utilizatorului." });
                }

                // ✅ Trimitem notificare prin e-mail administratorului
                await _serviciuEmail.TrimiteEmailAsync(
                    "contact.aquarios@gmail.com",
                    "Utilizator actualizat",
                    $"<p>Utilizatorul <strong>{user.UserName}</strong> a fost actualizat cu succes.</p>"
                );

                return Ok(new { Mesaj = "Utilizator actualizat cu succes." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la actualizarea utilizatorului: {ex.Message}" });
            }
        }
    }
}
