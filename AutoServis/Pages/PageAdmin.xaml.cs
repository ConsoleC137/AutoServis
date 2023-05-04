using System.Windows;
using System.Windows.Controls;

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
            Frame = frame;
        }

        private void buttonChageAdmins_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Pages.PageChangeAdmins(Frame));
        }

        private void buttonChageMasters_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Pages.PageChangeMasters(Frame));
        }

        private void buttonChageClients_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Pages.PageChangeClients(Frame));
        }

        private void buttonChageOrders_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new Pages.PageChandeOrders(Frame));
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
