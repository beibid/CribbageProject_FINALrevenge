using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class ProfilePage : Page
    {
        private const string SamplePlayerUsername = "XxDavidxx";
        private const string SamplePlayerEmail = "david@correo.com";
        private const int SampleMemberSinceYear = 2026;
        private const int SampleMemberSinceMonth = 9;
        private const int SampleMemberSinceDay = 11;
        private const int SampleMatchesPlayed = 150;
        private const int SampleMatchesWon = 90;
        private const int SampleMatchesLost = 60;
        private const double SampleWinRate = 0.6;
        private const int SampleBestHand = 24;

        public ProfilePage()
        {
            InitializeComponent();
            ShowIdentity();
            ShowStatistics();
        }

        private void ShowIdentity()
        {
            usernameTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Profile_UsernameFormat,
                SamplePlayerUsername);

            emailTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Profile_EmailFormat,
                SamplePlayerEmail);

            memberSinceTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Profile_MemberSinceFormat,
                GetMemberSinceText());
        }

        private string GetMemberSinceText()
        {
            DateTime memberSinceDate = new DateTime(
                SampleMemberSinceYear,
                SampleMemberSinceMonth,
                SampleMemberSinceDay);

            return memberSinceDate.ToString(Lang.Global_LongDateFormat, CultureInfo.CurrentCulture);
        }

        private void ShowStatistics()
        {
            matchesPlayedTextBlock.Text = SampleMatchesPlayed.ToString(CultureInfo.CurrentCulture);
            matchesWonTextBlock.Text = SampleMatchesWon.ToString(CultureInfo.CurrentCulture);
            matchesLostTextBlock.Text = SampleMatchesLost.ToString(CultureInfo.CurrentCulture);

            winRateTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Global_PercentageFormat,
                SampleWinRate);

            bestHandTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Profile_BestHandFormat,
                SampleBestHand);
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditProfilePage());
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePasswordDialogWindow dialog = new ChangePasswordDialogWindow();
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void ChangeEmailButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangeEmailPage(SamplePlayerEmail));
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