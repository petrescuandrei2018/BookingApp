using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services.Abstractions
{
    public interface IExportService
    {
        Task<byte[]> ExportaRezervariCsvAsync();
        Task<byte[]> ExportaRezervariExcelAsync();
    }
}
