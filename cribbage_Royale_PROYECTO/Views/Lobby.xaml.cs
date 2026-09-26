using System.Windows;
using System.Windows.Controls;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class Lobby : Page
    {
        public Lobby()
        {
            InitializeComponent();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Settings());
        }

        private void TutorialButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Tutorial());
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfilePage());
        }

        private void FriendsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FriendsPage());
        }
        private void ScoreboardButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ScoreboardPage());
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GamePage());
        }
    }
}