using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.ViewModel
{
    public interface ITelegramUserDetailViewModel
    {
        Task LoadAsync(long userId);
        bool HasChanges { get; }
    }
}