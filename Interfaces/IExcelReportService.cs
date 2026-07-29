namespace StartupWebAPIs.Interfaces
{
    public interface IExcelReportService
    {
        Task<string> GenerateProductsExcelAsync();
    }
}