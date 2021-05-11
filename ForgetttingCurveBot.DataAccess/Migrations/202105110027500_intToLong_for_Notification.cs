namespace ForgetttingCurveBot.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class intToLong_for_Notification : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval");
            DropIndex("dbo.TelegramUser", new[] { "NotificationIntervalId" });
            DropPrimaryKey("dbo.NotificationInterval");
            AlterColumn("dbo.NotificationInterval", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.TelegramUser", "NotificationIntervalId", c => c.Long());
            AddPrimaryKey("dbo.NotificationInterval", "Id");
            CreateIndex("dbo.TelegramUser", "NotificationIntervalId");
            AddForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval");
            DropIndex("dbo.TelegramUser", new[] { "NotificationIntervalId" });
            DropPrimaryKey("dbo.NotificationInterval");
            AlterColumn("dbo.TelegramUser", "NotificationIntervalId", c => c.Int());
            AlterColumn("dbo.NotificationInterval", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.NotificationInterval", "Id");
            CreateIndex("dbo.TelegramUser", "NotificationIntervalId");
            AddForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval", "Id");
        }
    }
}
