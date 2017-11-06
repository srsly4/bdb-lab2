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

            refreshProducts();
        }

        void refreshClients()
        {
            if (clientSearchTextBox.Text != String.Empty)
            {
                clientDataGrid.ItemsSource = (from client in dbContext.Customers
                                             orderby client.CompanyName
                                             where client.CompanyName.Contains(clientSearchTextBox.Text)
                                             select client).ToList();
            } else
            {
                clientDataGrid.ItemsSource = dbContext.Customers.OrderBy(client => client.CompanyName).Select(client => client).ToList();
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
    }
}
