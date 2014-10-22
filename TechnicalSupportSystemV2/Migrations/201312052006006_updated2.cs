namespace TechnicalSupportSystemV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updated2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "IsApprovedBefore");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "IsApprovedBefore", c => c.Boolean(nullable: false));
        }
    }
}
