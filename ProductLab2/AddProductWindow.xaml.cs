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
using System.Windows.Shapes;

namespace ProductLab2
{
    /// <summary>
    /// Logika interakcji dla klasy AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private DbContext context;
        private Product product;
        private bool success = false;

        public AddProductWindow(ProdContext context, Product product)
        {
            InitializeComponent();
            this.context = context;
            this.DataContext = product;

            categoryComboBox.ItemsSource = context.Categories.Local.ToBindingList();
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
            success = true;
            this.Close();
        }
    }
}
