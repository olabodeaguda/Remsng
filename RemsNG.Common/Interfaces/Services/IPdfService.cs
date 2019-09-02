namespace RemsNG.Common.Interfaces.Services
{
    public interface IPdfService
    {
        byte[] DemandNotice(string[] htmlstrings);
        byte[] DemandNotice(string htmlString);
        byte[] Reminder(string htmlString);
    }
}
