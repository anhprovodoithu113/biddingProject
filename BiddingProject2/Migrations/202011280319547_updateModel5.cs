namespace BiddingProject2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateModel5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Product", "Image", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "Image", c => c.String());
        }
    }
}
