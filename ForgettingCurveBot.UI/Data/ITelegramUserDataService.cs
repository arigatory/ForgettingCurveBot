using ForgettingCurveBot.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.Data
{
    public interface ITelegramUserDataService
    {
        Task<TelegramUser> GetByIdAsync(long userId);
        Task SaveAsync(TelegramUser telegramUser);
    }
}