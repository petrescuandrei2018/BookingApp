using BookingApp.Models.Dtos;
using BookingApp.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IStatisticiService _statisticiService;
        private readonly IExportService _exportService;

        public AdminController(IAuthService authService, IStatisticiService statisticiService, IExportService exportService)
        {
            _authService = authService;
            _statisticiService = statisticiService;
            _exportService = exportService;
        }

        // ✅ Obține statistici pentru dashboard-ul adminului
        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            try
            {
                var statistici = await _statisticiService.ObtineStatisticiDashboardAsync();
                return Ok(statistici);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la obținerea statisticilor: {ex.Message}" });
            }
        }

        // ✅ Export rezervări în format CSV
        [HttpGet("ExportRezervariCsv")]
        public async Task<IActionResult> ExportRezervariCsv()
        {
            try
            {
                var csvBytes = await _exportService.ExportaRezervariCsvAsync();
                return File(csvBytes, "text/csv", "rezervari.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la exportul rezervărilor: {ex.Message}" });
            }
        }

        // ✅ Export rezervări în format Excel
        [HttpGet("ExportRezervariExcel")]
        public async Task<IActionResult> ExportRezervariExcel()
        {
            try
            {
                var excelBytes = await _exportService.ExportaRezervariExcelAsync();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "rezervari.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la exportul rezervărilor: {ex.Message}" });
            }
        }

        // ✅ Actualizare utilizator
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto) // ✅ Confirmată structura corectă
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(updateUserDto.UserId); // ✅ Confirmat că `UserId` există
                if (user == null)
                {
                    return NotFound(new { Mesaj = "Utilizatorul nu a fost găsit." });
                }

                // ✅ Actualizăm câmpurile necesare
                user.UserName = updateUserDto.UserName;
                user.Email = updateUserDto.Email;
                user.PhoneNumber = updateUserDto.PhoneNumber;

                var success = await _authService.UpdateUserAsync(user);
                if (!success)
                {
                    return StatusCode(500, new { Mesaj = "A apărut o eroare la actualizarea utilizatorului." });
                }

                return Ok(new { Mesaj = "Utilizator actualizat cu succes." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la actualizarea utilizatorului: {ex.Message}" });
            }
        }

        // ✅ Setează sau elimină rolul de admin pentru un utilizator
        [HttpPut("SetAdmin")]
        public async Task<IActionResult> SetAdmin(string userId, bool isAdmin)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { Mesaj = "ID-ul utilizatorului nu este valid." });
            }

            try
            {
                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { Mesaj = "Utilizatorul nu a fost găsit." });
                }

                var roles = await _authService.GetUserRolesAsync(userId);
                if (isAdmin && !roles.Contains("admin"))
                {
                    await _authService.AtribuieRolAsync(userId, "admin");
                }
                else if (!isAdmin && roles.Contains("admin"))
                {
                    await _authService.EliminaRolAsync(userId, "admin");
                }

                return Ok(new { Mesaj = $"Rolul utilizatorului {user.UserName} a fost actualizat cu succes." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"A apărut o eroare: {ex.Message}" });
            }
        }

        // ✅ Obține toți utilizatorii
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"A apărut o eroare: {ex.Message}" });
            }
        }

        // ✅ Obține toate rolurile unui utilizator
        [HttpGet("GetUserRoles/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            try
            {
                var roles = await _authService.GetUserRolesAsync(userId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaj = $"Eroare la obținerea rolurilor utilizatorului: {ex.Message}" });
            }
        }
    }
}
