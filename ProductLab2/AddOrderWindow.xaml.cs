using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ProductLab2
{
    /// <summary>
    /// Logika interakcji dla klasy AddOrderWindow.xaml
    /// </summary>
    public partial class AddOrderWindow : Window
    {
        private bool success = false;
        private ProdContext context;
        private Order order;

        protected Category categoryFilter;
        protected Category CategoryFilter {
            get { return this.categoryFilter; }
            set {
                this.categoryFilter = value;
                if (value == null)
                {
                    addProductToOrderCombobox.ItemsSource = context.Products.ToList();
                } else
                {
                    // navigation property
                    addProductToOrderCombobox.ItemsSource = value.Products;
                }
            } }

        public AddOrderWindow(ProdContext context, Order order)
        {
            InitializeComponent();
            this.DataContext = this.order = order;
            this.context = context;

            customerComboBox.ItemsSource = context.Customers.Local.ToList();
            categoryFilterComboBox.ItemsSource = context.Categories.Local.ToList();
            CategoryFilter = null;
        }

        public bool Recall()
        {
            this.ShowDialog();
            return this.success;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.success = true;
            this.Close();
        }

        private void addProductToOrderButton_Click(object sender, RoutedEventArgs e)
        {
            int units = 0;
            if (addProductToOrderCombobox.SelectedItem != null && int.TryParse(addProductToOrderUnitsCombobox.Text, out units) && units > 0)
            {
                var item = new OrderItem();
                item.Product = (Product)addProductToOrderCombobox.SelectedItem;
                item.Order = this.order;
                item.Units = units;
                order.Items.Add(item); // navigation property saves our items when we save the Order
                orderItemsList.Items.Refresh();
                addProductToOrderCombobox.SelectedItem = null;
                addProductToOrderUnitsCombobox.Text = String.Empty;
            }
        }

        private void categoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.CategoryFilter = (Category)((ComboBox)sender).SelectedItem;
        }
    }
}
