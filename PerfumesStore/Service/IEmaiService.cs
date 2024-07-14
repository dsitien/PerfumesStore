namespace PerfumesStore.Service
{
    public interface IEmaiService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
