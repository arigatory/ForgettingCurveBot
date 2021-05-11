namespace ForgetttingCurveBot.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Back_to_ints : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval");
            DropIndex("dbo.TelegramUser", new[] { "NotificationIntervalId" });
            AddColumn("dbo.TelegramUser", "NotificationInterval_Id", c => c.Short());
            AlterColumn("dbo.TelegramUser", "NotificationIntervalId", c => c.Int());
            CreateIndex("dbo.TelegramUser", "NotificationInterval_Id");
            AddForeignKey("dbo.TelegramUser", "NotificationInterval_Id", "dbo.NotificationInterval", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TelegramUser", "NotificationInterval_Id", "dbo.NotificationInterval");
            DropIndex("dbo.TelegramUser", new[] { "NotificationInterval_Id" });
            AlterColumn("dbo.TelegramUser", "NotificationIntervalId", c => c.Short());
            DropColumn("dbo.TelegramUser", "NotificationInterval_Id");
            CreateIndex("dbo.TelegramUser", "NotificationIntervalId");
            AddForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval", "Id");
        }
    }
}
