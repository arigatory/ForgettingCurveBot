using ForgettingCurveBot.Model;
using ForgetttingCurveBot.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace ForgettingCurveBot.UI.Data
{
    public class TelegramUserDataService : ITelegramUserDataService
    {
        public TelegramUserDataService()
        {
        }

        public IEnumerable<TelegramUser> GetAll()
        {
            using (var ctx = new ForgettingCurveBotDbContext())
            {
                return ctx.Users.AsNoTracking().ToList();
            }
        }
    }
}
