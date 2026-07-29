using Hangfire;
using Microsoft.AspNetCore.Mvc;
using StartupWebAPIs.Jobs;

namespace StartupWebAPIs.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJob;

        public ReportsController(IBackgroundJobClient backgroundJob)
        {
            _backgroundJob = backgroundJob;
        }

        [HttpPost("products/pdf")]
        public IActionResult GenerateProductsPdf()
        {
            _backgroundJob.Enqueue<ReportJobs>(
                job => job.GenerateProductsPdfAsync());

            return Ok(new
            {
                Success = true,
                Message = "PDF generation started in the background."
            });
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadPdf(string fileName)
        {
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reports",
                "PDF",
                fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var bytes = System.IO.File.ReadAllBytes(filePath);

            return File(
                bytes,
                "application/pdf",
                fileName);
        }
        [HttpGet("download/pdf/{fileName}")]
        public IActionResult DownloadPdfByFile(string fileName)
        {
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reports",
                "PDF",
                fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "File not found."
                });
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);

            return File(
                bytes,
                "application/pdf",
                fileName);
        }

        [HttpPost("products/excel")]
        public IActionResult GenerateProductsExcel()
        {
            _backgroundJob.Enqueue<ReportJobs>(
                job => job.GenerateProductsExcelAsync());

            return Ok(new
            {
                Success = true,
                Message = "Excel generation started in the background."
            });
        }
        [HttpGet("download/excel/{fileName}")]
        public IActionResult DownloadExcel(string fileName)
        {
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reports",
                "Excel",
                fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "File not found."
                });
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
       
    }
}