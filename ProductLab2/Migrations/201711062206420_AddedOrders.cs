namespace ProductLab2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "SalesLab2.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        CustomerID = c.Int(nullable: false),
                        Customer_CompanyName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("SalesLab2.Customers", t => t.Customer_CompanyName)
                .Index(t => t.Customer_CompanyName);
            
            CreateTable(
                "SalesLab2.OrderItems",
                c => new
                    {
                        OrderID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Units = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrderID, t.ProductID })
                .ForeignKey("SalesLab2.Orders", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("SalesLab2.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.ProductID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("SalesLab2.OrderItems", "ProductID", "SalesLab2.Products");
            DropForeignKey("SalesLab2.OrderItems", "OrderID", "SalesLab2.Orders");
            DropForeignKey("SalesLab2.Orders", "Customer_CompanyName", "SalesLab2.Customers");
            DropIndex("SalesLab2.OrderItems", new[] { "ProductID" });
            DropIndex("SalesLab2.OrderItems", new[] { "OrderID" });
            DropIndex("SalesLab2.Orders", new[] { "Customer_CompanyName" });
            DropTable("SalesLab2.OrderItems");
            DropTable("SalesLab2.Orders");
        }
    }
}
