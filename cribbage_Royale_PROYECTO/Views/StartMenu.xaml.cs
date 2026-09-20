using System.Windows;
using System.Windows.Controls;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class StartMenu : Page
    {
        public StartMenu()
        {
            InitializeComponent();
        }

        // Opens the sign in screen
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }

        // Opens the tutorial
        private void TutorialButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Tutorial());
        }

        // Opens the settings screen
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Settings());
        }
    }
}