namespace TechnicalSupportSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updated : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Components", "IsNoMore");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Components", "IsNoMore", c => c.Boolean(nullable: false));
        }
    }
}
