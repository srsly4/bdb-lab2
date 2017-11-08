using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProductLab2
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProdContext dbContext;

        Category currentCategory;

        public bool IsProductSelected { get { return productDataGrid.SelectedItem != null; } }
        public bool IsCategorySelected { get { return currentCategory != null; } }

        public MainWindow()
        {
            InitializeComponent();
            dbContext = new ProdContext();
            dbContext.Categories.Load();
            categoriesList.ItemsSource = dbContext.Categories.Local.ToBindingList();

            refreshClients();
            refreshProducts();
            refreshOrders();
        }

        void refreshClients()
        {
            if (clientSearchTextBox.Text != String.Empty)
            {
                clientsList.ItemsSource = (from client in dbContext.Customers
                                             orderby client.CompanyName
                                             where client.CompanyName.Contains(clientSearchTextBox.Text)
                                             select client).ToList();
            } else
            {
                clientsList.ItemsSource = dbContext.Customers
                    .OrderBy(client => client.CompanyName)
                    .Select(client => client)
                    .ToList();
            }
        }

        void refreshProducts()
        {
            if (this.currentCategory != null)
            {
                // eager loading
                productDataGrid.ItemsSource = dbContext.Products
                    .OrderBy(product => product.Name)
                    .Where(product => product.CategoryID == this.currentCategory.CategoryID)
                    .Include("Category")
                    .Select(product => product).ToList();
            } else
            {
                productDataGrid.ItemsSource = dbContext.Products
                    .OrderBy(product => product.Name)
                    .Include("Category")
                    .Select(product => product).ToList();
            }
        }

        void refreshOrders()
        {
            var query = (from order in dbContext.Orders.Include("Customer")
                         join orderItem in dbContext.OrderItems on order.OrderID equals orderItem.OrderID
                         group orderItem by order into g
                         select new
                         {
                             OrderId = g.Key.OrderID,
                             Order = g.Key,
                             Customer = g.Key.Customer,
                             ProductCount = g.Count(),
                             OrderPrice = g.Sum(oi => oi.Product.Unitprice * oi.Units),
                         });
            ordersList.Items.Clear();

            // deferred query execution
            foreach (var order in query)
            {
                ordersList.Items.Add(order);
            }
        }

        private void categoryAddButton_Click(object sender, RoutedEventArgs e)
        {
            var cat = new Category();
            cat.Name = categoryAddTextBox.Text;
            categoryAddTextBox.Text = String.Empty;
            dbContext.Categories.Add(cat);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dbContext.SaveChanges();
        }

        private void categoryAddTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void clientSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.refreshClients();
        }

        private void addProductButton_Click(object sender, RoutedEventArgs e)
        {
            var product = new Product();
            var modal = new AddProductWindow(dbContext, product);
            modal.Owner = this;
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                if (modal.Recall())
                {
                    dbContext.Products.Add(product);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    refreshProducts();
                } else
                {
                    transaction.Rollback();
                }
            }
        }

        private void editProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (productDataGrid.SelectedItem != null)
            {
                var modal = new AddProductWindow(dbContext, (Product)productDataGrid.SelectedItem);
                modal.Owner = this;
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    if (modal.Recall())
                    {
                        dbContext.SaveChanges();
                        transaction.Commit();
                        refreshProducts();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        private void categoriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.currentCategory = (Category)categoriesList.SelectedItem;
            refreshProducts();
        }

        private void addClientButton_Click(object sender, RoutedEventArgs e)
        {
            var customer = new Customer();
            var modal = new AddClientWindow(dbContext, customer);
            modal.Owner = this;
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                if (modal.Recall())
                {
                    dbContext.Customers.Add(customer);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    refreshClients();
                }
                else
                {
                    transaction.Rollback();
                }
            }
        }

        private void addOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var order = new Order();
            var modal = new AddOrderWindow(dbContext, order);
            modal.Owner = this;
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                if (modal.Recall())
                {
                    dbContext.Orders.Add(order);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    refreshOrders();
                }
                else
                {
                    transaction.Rollback();
                }
            }
        }

        private void editOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (ordersList.SelectedItem != null)
            {
                var order = (Order)ordersList.SelectedItem.GetType().GetProperty("Order").GetValue(ordersList.SelectedItem);
                var modal = new AddOrderWindow(dbContext, order);
                modal.Owner = this;
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    if (modal.Recall())
                    {
                        try
                        {
                            dbContext.SaveChanges();
                            transaction.Commit();
                            refreshOrders();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            transaction.Rollback();
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
            }
        }
    }
}
