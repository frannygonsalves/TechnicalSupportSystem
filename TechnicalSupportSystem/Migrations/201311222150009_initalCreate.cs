namespace TechnicalSupportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initalCreate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "VendorOrderNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "vendorOrderNumber", c => c.Int(nullable: false));
        }
    }
}
