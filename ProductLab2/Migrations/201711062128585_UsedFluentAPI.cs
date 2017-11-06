namespace ProductLab2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsedFluentAPI : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "dbo.Categories", newSchema: "SalesLab2");
            MoveTable(name: "dbo.Products", newSchema: "SalesLab2");
            MoveTable(name: "dbo.Customers", newSchema: "SalesLab2");
            DropIndex("SalesLab2.Products", new[] { "CategoryID" });
            AlterColumn("SalesLab2.Categories", "Name", c => c.String(maxLength: 64));
            CreateIndex("SalesLab2.Products", "CategoryID");
        }
        
        public override void Down()
        {
            DropIndex("SalesLab2.Products", new[] { "CategoryID" });
            AlterColumn("SalesLab2.Categories", "Name", c => c.String());
            CreateIndex("SalesLab2.Products", "CategoryID");
            MoveTable(name: "SalesLab2.Customers", newSchema: "dbo");
            MoveTable(name: "SalesLab2.Products", newSchema: "dbo");
            MoveTable(name: "SalesLab2.Categories", newSchema: "dbo");
        }
    }
}
