using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface IPdfCreator
    {
        Task<bool> CreateDailySheet();
    }
}
