using RemsNG.Common.Models;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IPdfService
    {
        byte[] GetBytes(string[] htmlstrings);
        byte[] GetBytes(string[] htmlString, TemplateType templateType);
    }
}
