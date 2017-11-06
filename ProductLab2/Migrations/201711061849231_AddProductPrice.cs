namespace ProductLab2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Unitprice", c => c.Decimal(nullable: false, storeType: "money"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Unitprice");
        }
    }
}
