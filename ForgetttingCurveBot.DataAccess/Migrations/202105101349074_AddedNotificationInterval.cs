namespace ForgetttingCurveBot.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNotificationInterval : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationInterval",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IntervalMinutes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TelegramUser", "NotificationIntervalId", c => c.Int());
            CreateIndex("dbo.TelegramUser", "NotificationIntervalId");
            AddForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TelegramUser", "NotificationIntervalId", "dbo.NotificationInterval");
            DropIndex("dbo.TelegramUser", new[] { "NotificationIntervalId" });
            DropColumn("dbo.TelegramUser", "NotificationIntervalId");
            DropTable("dbo.NotificationInterval");
        }
    }
}
