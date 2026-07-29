using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using StartupWebAPIs.Data;
using StartupWebAPIs.Interfaces;

namespace StartupWebAPIs.Services
{
    public class ExcelReportService : IExcelReportService
    {
        private readonly AppDbContext _context;

        public ExcelReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateProductsExcelAsync()
        {
            var products = await _context.Products
                .OrderBy(x => x.Id)
                .ToListAsync();

            var reportsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reports",
                "Excel");

            Directory.CreateDirectory(reportsFolder);

            var fileName = $"Products_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var filePath = Path.Combine(reportsFolder, fileName);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Products");

            // Header
            worksheet.Cell(1, 1).Value = "Id";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Price";

            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;

            foreach (var product in products)
            {
                worksheet.Cell(row, 1).Value = product.Id;
                worksheet.Cell(row, 2).Value = product.Name;
                worksheet.Cell(row, 3).Value = product.Price;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(filePath);

            return filePath;
        }
    }
}