namespace TechnicalSupportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedVendorName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "VendorName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "VendorName");
        }
    }
}
