namespace ForgetttingCurveBot.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShortToIntNotificationInterval : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TelegramUser", "NotificationInterval_Id", "dbo.NotificationInterval");
            DropIndex("dbo.TelegramUser", new[] { "NotificationInterval_Id" });
            DropColumn("dbo.TelegramUser", "NotificationIntervalId");
            RenameColumn(table: "dbo.TelegramUser", name: "NotificationInterval_Id", newName: "NotificationIntervalId");
            DropPrimaryKey("dbo.NotificationInterval");
            AlterColumn("dbo.NotificationInterval", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.TelegramUser", "NotificationIntervalId", c => c.Int());
            AddPrimaryKey("dbo.NotificationInterval", "Id");
            CreateIndex("dbo.TelegramUser", "NotificationIntervalId");
            AddForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval");
            DropIndex("dbo.TelegramUser", new[] { "NotificationIntervalId" });
            DropPrimaryKey("dbo.NotificationInterval");
            AlterColumn("dbo.TelegramUser", "NotificationIntervalId", c => c.Short());
            AlterColumn("dbo.NotificationInterval", "Id", c => c.Short(nullable: false, identity: true));
            AddPrimaryKey("dbo.NotificationInterval", "Id");
            RenameColumn(table: "dbo.TelegramUser", name: "NotificationIntervalId", newName: "NotificationInterval_Id");
            AddColumn("dbo.TelegramUser", "NotificationIntervalId", c => c.Int());
            CreateIndex("dbo.TelegramUser", "NotificationInterval_Id");
            AddForeignKey("dbo.TelegramUser", "NotificationInterval_Id", "dbo.NotificationInterval", "Id");
        }
    }
}
