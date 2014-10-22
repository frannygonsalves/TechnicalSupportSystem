namespace TechnicalSupportSystemV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updated1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Components", "Quantity", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "VendorOrderNumber", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "VendorOrderNumber", c => c.Int(nullable: false));
            DropColumn("dbo.Components", "Quantity");
        }
    }
}
