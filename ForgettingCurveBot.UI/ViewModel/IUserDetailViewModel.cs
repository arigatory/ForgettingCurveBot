using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.ViewModel
{
    public interface IUserDetailViewModel
    {
        Task LoadAsync(long userId);
    }
}