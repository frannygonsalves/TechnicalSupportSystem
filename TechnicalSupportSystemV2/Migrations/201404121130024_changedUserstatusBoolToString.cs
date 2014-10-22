namespace TechnicalSupportSystemV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedUserstatusBoolToString : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "UserType", c => c.String());
            DropColumn("dbo.UserProfile", "IsStudent");
            DropColumn("dbo.UserProfile", "IsTechnician");
            DropColumn("dbo.UserProfile", "IsSupervisor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfile", "IsSupervisor", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserProfile", "IsTechnician", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserProfile", "IsStudent", c => c.Boolean(nullable: false));
            DropColumn("dbo.UserProfile", "UserType");
        }
    }
}
