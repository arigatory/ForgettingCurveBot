using ForgettingCurveBot.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.Data
{
    public interface IUserLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetUsersLookupAsync();
    }
}