namespace BiddingProject2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateModel4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Product", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "Name", c => c.String());
        }
    }
}
