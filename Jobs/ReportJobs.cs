using StartupWebAPIs.Interfaces;

namespace StartupWebAPIs.Jobs
{
    public class ReportJobs
    {
        private readonly IPdfReportService _pdfReportService;
        private readonly ILogger<ReportJobs> _logger;
        private readonly IExcelReportService _excelReportService;
        private readonly IEmailService _emailService;
        public ReportJobs(
            IPdfReportService pdfReportService,
            ILogger<ReportJobs> logger, IExcelReportService excelReportService, IEmailService emailService)
        {
            _pdfReportService = pdfReportService;
            _logger = logger;
            _excelReportService = excelReportService;
            _emailService = emailService;
        }

        public async Task GenerateProductsPdfAsync()
        {
            _logger.LogInformation("Starting PDF report generation...");

            var filePath = await _pdfReportService.GenerateProductsPdfAsync();
            await _emailService.SendEmailWithAttachmentAsync(
                 "shukladeepika1023@gmail.com",
                 "Products Report",
                 "Please find the attached Products Report.",
                 filePath);
            _logger.LogInformation(
                "PDF report generated successfully: {FilePath}",
                filePath);
        }
        public async Task GenerateProductsExcelAsync()
        {
            _logger.LogInformation("Starting Excel report generation...");

            var filePath = await _excelReportService.GenerateProductsExcelAsync();
            await _emailService.SendEmailWithAttachmentAsync(
                 "shukladeepika1023@gmail.com",
                 "Products Report",
                 "Please find the attached Products Report.",
                 filePath);
            _logger.LogInformation(
                "PDF report generated successfully: {FilePath}",
                filePath);
            _logger.LogInformation(
                "Excel report generated successfully: {FilePath}",
                filePath);
        }

    }
}