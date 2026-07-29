using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StartupWebAPIs.Data;
using StartupWebAPIs.Interfaces;

namespace StartupWebAPIs.Services
{
    public class PdfReportService : IPdfReportService
    {
        private readonly AppDbContext _context;

        public PdfReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateProductsPdfAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var products = await _context.Products
                .OrderBy(x => x.Id)
                .ToListAsync();

            var reportsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reports",
                "PDF");

            Directory.CreateDirectory(reportsFolder);

            var fileName =
                $"Products_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            var filePath = Path.Combine(reportsFolder, fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("Products Report")
                        .FontSize(22)
                        .Bold();

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(60);
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Id").Bold();
                                header.Cell().Text("Name").Bold();
                                header.Cell().Text("Price").Bold();
                            });

                            foreach (var product in products)
                            {
                                table.Cell().Text(product.Id.ToString());
                                table.Cell().Text(product.Name);
                                table.Cell().Text(product.Price.ToString("0.00"));
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on ");
                            x.Span(DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                        });
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }
    }
}