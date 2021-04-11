using ForgettingCurveBot.Model;
using ForgetttingCurveBot.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.Data
{
    public class TelegramUserDataService : ITelegramUserDataService
    {
        private readonly Func<ForgettingCurveBotDbContext> _contextCreator;

        public TelegramUserDataService(Func<ForgettingCurveBotDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<TelegramUser> GetByIdAsync(long userId)
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Users.AsNoTracking().SingleAsync(u => u.Id == userId);
            }
        }

        public async Task SaveAsync(TelegramUser telegramUser)
        {
            using (var ctx = _contextCreator())
            {
                ctx.Users.Attach(telegramUser);
                ctx.Entry(telegramUser).State = EntityState.Modified;
                await ctx.SaveChangesAsync();
            }
        }
    }
}
