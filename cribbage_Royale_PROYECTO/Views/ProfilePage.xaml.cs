using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CribbageRoyale.Contracts.DataContracts;
using cribbage_Royale_PROYECTO.Localization;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class ProfilePage : Page
    {
        private readonly PlayerProfileClient _profileClient;

        private OwnProfile _profile;

        public ProfilePage()
        {
            InitializeComponent();
            _profileClient = ((App)Application.Current).PlayerProfileClient;
            _profile = new OwnProfile();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ProfileResult result = await _profileClient.GetOwnProfileAsync(CurrentSession.SessionToken);

            if (result.Succeeded)
            {
                _profile = result.Profile;
                ShowIdentity();
                ShowStatistics();
                AvatarImage.ShowIn(avatarEllipse, _profile.AvatarImage);
            }
            else
            {
                MessageDialog.ShowError(Window.GetWindow(this), ProfileMessages.Resolve(result));
            }
        }

        private void ShowIdentity()
        {
            displayNameTextBlock.Text = _profile.DisplayName;

            usernameTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Profile_UsernameFormat,
                _profile.UserName);

            emailTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Profile_EmailFormat,
                _profile.Email);

            memberSinceTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Profile_MemberSinceFormat,
                _profile.RegisteredAtUtc.ToLocalTime().ToString(Lang.Global_LongDateFormat, CultureInfo.CurrentCulture));
        }

        private void ShowStatistics()
        {
            matchesPlayedTextBlock.Text = _profile.MatchesPlayed.ToString(CultureInfo.CurrentCulture);
            matchesWonTextBlock.Text = _profile.Wins.ToString(CultureInfo.CurrentCulture);
            matchesLostTextBlock.Text = _profile.Losses.ToString(CultureInfo.CurrentCulture);
            currentStreakTextBlock.Text = _profile.CurrentStreak.ToString(CultureInfo.CurrentCulture);

            winRateTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Global_PercentageFormat,
                CalculateWinRate());
        }

        private double CalculateWinRate()
        {
            double winRate = 0;

            if (_profile.MatchesPlayed > 0)
            {
                winRate = (double)_profile.Wins / _profile.MatchesPlayed;
            }

            return winRate;
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditProfilePage(_profile));
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordDialogWindow dialog = new ChangePasswordDialogWindow();
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void ChangeEmailButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangeEmailPage(_profile.Email));
        }

        private void MatchHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MatchHistoryPage());
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DeleteAccountPage());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Lobby());
        }
    }
}