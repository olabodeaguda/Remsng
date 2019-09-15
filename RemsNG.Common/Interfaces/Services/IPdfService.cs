namespace RemsNG.Common.Interfaces.Services
{
    public interface IPdfService
    {
        byte[] GetBytes(string[] htmlstrings);
    }
}
