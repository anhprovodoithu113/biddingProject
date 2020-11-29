namespace BiddingProject2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateModel3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Category", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Category", "Name", c => c.String());
        }
    }
}
