namespace RemsNG.Common.Interfaces.Services
{
    public interface IPdfService
    {
        byte[] GetPdf(string htmlstring);
        byte[] GetPdf(string[] htmlstrings);
    }
}
