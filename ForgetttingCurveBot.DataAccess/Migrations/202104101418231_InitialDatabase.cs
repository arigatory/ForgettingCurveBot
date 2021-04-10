namespace ForgetttingCurveBot.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attempt",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTimeOffset(nullable: false, precision: 7),
                        Correct = c.Boolean(nullable: false),
                        CardToRemember_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CardToRemember", t => t.CardToRemember_Id)
                .Index(t => t.CardToRemember_Id);
            
            CreateTable(
                "dbo.CardToRemember",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Data = c.String(),
                        Type = c.Int(nullable: false),
                        Learned = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        TelegramUser_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TelegramUser", t => t.TelegramUser_Id)
                .Index(t => t.TelegramUser_Id);
            
            CreateTable(
                "dbo.TelegramUser",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TelegramIdentification = c.Long(nullable: false),
                        Nickname = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CardToRemember", "TelegramUser_Id", "dbo.TelegramUser");
            DropForeignKey("dbo.Attempt", "CardToRemember_Id", "dbo.CardToRemember");
            DropIndex("dbo.CardToRemember", new[] { "TelegramUser_Id" });
            DropIndex("dbo.Attempt", new[] { "CardToRemember_Id" });
            DropTable("dbo.TelegramUser");
            DropTable("dbo.CardToRemember");
            DropTable("dbo.Attempt");
        }
    }
}
