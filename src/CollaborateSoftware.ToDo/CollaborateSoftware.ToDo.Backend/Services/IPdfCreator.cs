using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface IPdfCreator
    {
        Task<bool> CreateDailySheet(List<ToDoListEntry> todos);
    }
}
