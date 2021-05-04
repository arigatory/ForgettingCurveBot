using ForgettingCurveBot.Model;
using ForgetttingCurveBot.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.Data.Repositories
{
    public class TelegramUserRepository : ITelegramUserRepository
    {
        private readonly ForgettingCurveBotDbContext _context;

        public TelegramUserRepository(ForgettingCurveBotDbContext context)
        {
            _context = context;
        }

        public void Add(TelegramUser user)
        {
            _context.Users.Add(user);
        }

        public async Task<TelegramUser> GetByIdAsync(long userId)
        {
            return await _context.Users.SingleAsync(u => u.Id == userId);
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public void Remove(TelegramUser model)
        {
            _context.Users.Remove(model);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
