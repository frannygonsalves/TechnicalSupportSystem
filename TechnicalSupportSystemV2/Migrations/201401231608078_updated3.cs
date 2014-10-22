namespace TechnicalSupportSystemV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updated3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "IsChecked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "IsChecked");
        }
    }
}
