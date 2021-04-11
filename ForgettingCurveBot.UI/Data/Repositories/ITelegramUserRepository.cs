using ForgettingCurveBot.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.Data.Repositories
{
    public interface ITelegramUserRepository
    {
        Task<TelegramUser> GetByIdAsync(long userId);
        Task SaveAsync();
        bool HasChanges();
    }
}