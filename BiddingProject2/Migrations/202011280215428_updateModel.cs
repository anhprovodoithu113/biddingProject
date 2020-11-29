namespace BiddingProject2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Profile", "Name", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Profile", "Name", c => c.String());
        }
    }
}
