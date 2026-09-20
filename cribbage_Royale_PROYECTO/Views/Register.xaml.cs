using System.Windows;
using System.Windows.Controls;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class Register : Page
    {
        public Register()
        {
            InitializeComponent();
        }

        // Goes back to the sign in screen
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }
    }
}