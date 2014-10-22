namespace TechnicalSupportSystemV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedOrderedBy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OrderedBy");
        }
    }
}
