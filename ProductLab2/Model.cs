using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductLab2
{
    public class Category
    {
        public int CategoryID { get; set; }
        public String Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public Category()
        {
            Products = new List<Product>();
        }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public String Name { get; set; }
        public int UnitsInStock { get; set; }
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }
        [Column(TypeName="money")]
        public decimal Unitprice { get; set; }
    }

    public class Customer
    {
        [Key]
        public String CompanyName { get; set; }
        public String Description { get; set; }
    }

    public class OrderItem
    {
        public int ProductID { get; set; }
        public int OrderID { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public int Units { get; set; }
    }

    public class Order
    {
        public int OrderID { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }

    public class ProdContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // Usage of the Fluent API
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Sets the schema name
            modelBuilder
                .HasDefaultSchema("SalesLab2");

            // Sets the Category name max length
            modelBuilder
                .Entity<Category>()
                .Property(c => c.Name)
                .HasMaxLength(64);

            // Index in products on CategoryID
            modelBuilder
                .Entity<Product>()
                .Property(p => p.CategoryID)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));

            // Index in orderd items on ProductID and OrderID
            modelBuilder
                .Entity<OrderItem>()
                .Property(i => i.ProductID)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
            modelBuilder
                .Entity<OrderItem>()
                .Property(i => i.OrderID)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));

            // Custom primary key
            modelBuilder
                .Entity<OrderItem>()
                .HasKey(i => new { i.OrderID, i.ProductID });
        }
    }
}
