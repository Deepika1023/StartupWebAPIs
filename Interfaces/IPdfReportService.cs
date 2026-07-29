namespace StartupWebAPIs.Interfaces
{
    public interface IPdfReportService
    {
        Task<string> GenerateProductsPdfAsync();
    }
}