namespace ForgetttingCurveBot.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LongToShort_for_Notification : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval");
            DropIndex("dbo.TelegramUser", new[] { "NotificationIntervalId" });
            DropPrimaryKey("dbo.NotificationInterval");
            AlterColumn("dbo.NotificationInterval", "Id", c => c.Short(nullable: false, identity: true));
            AlterColumn("dbo.TelegramUser", "NotificationIntervalId", c => c.Short());
            AddPrimaryKey("dbo.NotificationInterval", "Id");
            CreateIndex("dbo.TelegramUser", "NotificationIntervalId");
            AddForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval");
            DropIndex("dbo.TelegramUser", new[] { "NotificationIntervalId" });
            DropPrimaryKey("dbo.NotificationInterval");
            AlterColumn("dbo.TelegramUser", "NotificationIntervalId", c => c.Long());
            AlterColumn("dbo.NotificationInterval", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.NotificationInterval", "Id");
            CreateIndex("dbo.TelegramUser", "NotificationIntervalId");
            AddForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval", "Id");
        }
    }
}
