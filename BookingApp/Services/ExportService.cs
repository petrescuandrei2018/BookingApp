using BookingApp.Data;
using BookingApp.Models;
using BookingApp.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace BookingApp.Services
{
    public class ExportService : IExportService
    {
        private readonly AppDbContext _context;

        public ExportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> ExportaRezervariCsvAsync()
        {
            var rezervari = await _context.Rezervari
                .Include(r => r.User)
                .Include(r => r.PretCamera)
                .ThenInclude(pc => pc.TipCamera)
                .ThenInclude(tc => tc.Hotel)
                .ToListAsync();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("RezervareId,User,Hotel,Camera,CheckIn,CheckOut,Pret,SumaAchitata,StarePlata");

            foreach (var rezervare in rezervari)
            {
                csvBuilder.AppendLine($"{rezervare.RezervareId},{rezervare.User.UserName},{rezervare.PretCamera.TipCamera.Hotel.Name}," +
                    $"{rezervare.PretCamera.TipCamera.Name},{rezervare.CheckIn.ToString("yyyy-MM-dd")},{rezervare.CheckOut.ToString("yyyy-MM-dd")}," +
                    $"{rezervare.SumaTotala.ToString(CultureInfo.InvariantCulture)},{rezervare.SumaAchitata.ToString(CultureInfo.InvariantCulture)}," +
                    $"{rezervare.StarePlata}");
            }

            return Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        public async Task<byte[]> ExportaRezervariExcelAsync()
        {
            var rezervari = await _context.Rezervari
                .Include(r => r.User)
                .Include(r => r.PretCamera)
                .ThenInclude(pc => pc.TipCamera)
                .ThenInclude(tc => tc.Hotel)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // 🔹 Adăugăm această linie!

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Rezervari");

            worksheet.Cells[1, 1].Value = "RezervareId";
            worksheet.Cells[1, 2].Value = "User";
            worksheet.Cells[1, 3].Value = "Hotel";
            worksheet.Cells[1, 4].Value = "Camera";
            worksheet.Cells[1, 5].Value = "CheckIn";
            worksheet.Cells[1, 6].Value = "CheckOut";
            worksheet.Cells[1, 7].Value = "Pret";
            worksheet.Cells[1, 8].Value = "SumaAchitata";
            worksheet.Cells[1, 9].Value = "StarePlata";

            int row = 2;
            foreach (var rezervare in rezervari)
            {
                worksheet.Cells[row, 1].Value = rezervare.RezervareId;
                worksheet.Cells[row, 2].Value = rezervare.User.UserName;
                worksheet.Cells[row, 3].Value = rezervare.PretCamera.TipCamera.Hotel.Name;
                worksheet.Cells[row, 4].Value = rezervare.PretCamera.TipCamera.Name;
                worksheet.Cells[row, 5].Value = rezervare.CheckIn.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 6].Value = rezervare.CheckOut.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 7].Value = rezervare.SumaTotala;
                worksheet.Cells[row, 8].Value = rezervare.SumaAchitata;
                worksheet.Cells[row, 9].Value = rezervare.StarePlata;
                row++;
            }

            return package.GetAsByteArray();
        }
    }
}
