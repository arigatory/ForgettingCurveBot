using ForgettingCurveBot.Model;
using System.Collections.Generic;

namespace ForgettingCurveBot.UI.Data
{
    public interface ITelegramUserDataService
    {
        IEnumerable<TelegramUser> GetAll();
    }
}