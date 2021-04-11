using ForgettingCurveBot.Model;
using ForgetttingCurveBot.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.Data
{
    public class LookupDataService : IUserLookupDataService
    {
        private readonly Func<ForgettingCurveBotDbContext> _contexCreator;

        public LookupDataService(Func<ForgettingCurveBotDbContext> contexCreator)
        {
            _contexCreator = contexCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetUsersLookupAsync()
        {
            using (var ctx = _contexCreator())
            {
                return await ctx.Users.AsNoTracking()
                    .Select(u =>
                    new LookupItem
                    {
                        Id = u.Id,
                        DisplayMember = u.Nickname + " " + u.TelegramIdentification.ToString()
                    }).ToListAsync();
            }
        }
    }
}
