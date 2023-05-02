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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoServis.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAdmin.xaml
    /// </summary>
    public partial class PageAdmin : Page
    {
        private Frame Frame;
        public PageAdmin(Frame frame)
        {
            InitializeComponent();
            this.Frame = frame;
        }

        private void buttonChageAdmins_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(new Pages.PageChangeAdmins(Frame));
        }

        private void buttonChageMasters_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(new Pages.PageChangeMasters(Frame));
        }

        private void buttonChageClients_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(new Pages.PageChangeClients(Frame));
        }

        private void buttonChageOrders_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
