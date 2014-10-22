namespace TechnicalSupportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedBoolSignupStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "IsStudent", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserProfile", "IsTechnician", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserProfile", "IsSupervisor", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "IsSupervisor");
            DropColumn("dbo.UserProfile", "IsTechnician");
            DropColumn("dbo.UserProfile", "IsStudent");
        }
    }
}
