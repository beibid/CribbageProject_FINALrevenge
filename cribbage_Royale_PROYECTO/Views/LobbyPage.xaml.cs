using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class Lobby : Page
    {
        private const int SamplePlayerSkillLevel = 0;

        private readonly PlayerProfileClient _profileClient;

        public Lobby()
        {
            InitializeComponent();
            _profileClient = ((App)Application.Current).PlayerProfileClient;
            ShowPlayerInformation();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!CurrentSession.IsGuest())
            {
                await ShowOwnProfileAsync();
            }
        }

        private async Task ShowOwnProfileAsync()
        {
            ProfileResult result = await _profileClient.GetOwnProfileAsync(CurrentSession.SessionToken);

            if (result.Succeeded)
            {
                playerNameTextBlock.Text = result.Profile.DisplayName;
                AvatarImage.ShowIn(avatarEllipse, result.Profile.AvatarImage);
            }
        }

        private string GetLevelText()
        {
            string levelText = Lang.Lobby_GuestLabel;

            if (!CurrentSession.IsGuest())
            {
                levelText = string.Format(
                    CultureInfo.CurrentCulture,
                    Lang.Lobby_LevelFormat,
                    SamplePlayerSkillLevel);
            }

            return levelText;
        }

        private void ShowPlayerInformation()
        {
            playerNameTextBlock.Text = CurrentSession.Username;
            playerLevelTextBlock.Text = GetLevelText();
        }

        private void ShowGuestRestriction()
        {
            MessageDialog.ShowWarning(
                Window.GetWindow(this),
                Lang.GuestAccess_FeatureBlockedMessage);
        }

        public void ShowPendingInvitation()
        {
            invitationBorder.Visibility = Visibility.Visible;
        }

        public void HidePendingInvitation()
        {
            invitationBorder.Visibility = Visibility.Collapsed;
        }

        private void NavigateToFriends()
        {
            if (CurrentSession.IsGuest())
            {
                ShowGuestRestriction();
            }
            else
            {
                NavigationService.Navigate(new FriendsPage());
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Settings());
        }

        private void TutorialButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Tutorial());
        }

        private void ScoreboardButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ScoreboardPage(CurrentSession.Username));
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToFriends();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.IsGuest())
            {
                ShowGuestRestriction();
            }
            else
            {
                NavigationService.Navigate(new ProfilePage());
            }
        }

        private void FriendsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToFriends();
        }
    }
}