using ForgettingCurveBot.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.Data.Lookups
{
    public interface INotificationIntervalsLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetNotificationIntervalsLookupAsync();
    }
}