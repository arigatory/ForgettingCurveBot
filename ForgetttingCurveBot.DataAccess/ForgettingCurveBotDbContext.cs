using ForgettingCurveBot.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ForgetttingCurveBot.DataAccess
{
    public class ForgettingCurveBotDbContext : DbContext
    {
        public ForgettingCurveBotDbContext() : base("ForgettingCurveBotDb")
        {

        }

        public DbSet<TelegramUser> Users { get; set; }
        public DbSet<CardToRemember> Cards { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<NotificationInterval> NotificationIntervals { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
