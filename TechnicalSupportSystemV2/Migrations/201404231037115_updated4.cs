namespace TechnicalSupportSystemV2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updated4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Projects", "Module", c => c.String(nullable: false));
            AlterColumn("dbo.Projects", "ProjectName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Projects", "ProjectName", c => c.String());
            AlterColumn("dbo.Projects", "Module", c => c.String());
        }
    }
}
