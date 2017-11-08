namespace ProductLab2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedIdForCustomer : DbMigration
    {
        public override void Up()
        {
            DropColumn("SalesLab2.Orders", "CustomerID");
        }
        
        public override void Down()
        {
            AddColumn("SalesLab2.Orders", "CustomerID", c => c.Int(nullable: false));
        }
    }
}
