using cribbage_Royale_PROYECTO.Properties;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class StartMenu : Page
    {
        public StartMenu()
        {
            InitializeComponent();
        }

        // When the page is shown, the Play button starts selected
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PlayButton.Focus();
        }

        // Hovering a menu button moves the selection to it
        private void MenuButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Focus();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Settings());
        }
        // Opens the tutorial
        private void TutorialButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Tutorial());
        }
        // Opens the sign in screen
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }
    }
}