using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            ShowProfile();
        }

        private void ShowProfile()
        {
            usernameTextBlock.Text = string.Format(CultureInfo.CurrentCulture, Lang.Profile_UsernameFormat, "XxDavidxx");
            emailTextBlock.Text = string.Format(CultureInfo.CurrentCulture, Lang.Profile_EmailFormat, "david@correo.com");
            memberSinceTextBlock.Text = string.Format(CultureInfo.CurrentCulture, Lang.Profile_MemberSinceFormat,
                new DateTime(2026, 9, 11).ToString(Lang.Global_LongDateFormat, CultureInfo.CurrentCulture));

            matchesPlayedTextBlock.Text = 150.ToString(CultureInfo.CurrentCulture);
            matchesWonTextBlock.Text = 90.ToString(CultureInfo.CurrentCulture);
            matchesLostTextBlock.Text = 60.ToString(CultureInfo.CurrentCulture);
            winRateTextBlock.Text = string.Format(CultureInfo.CurrentCulture, Lang.Global_PercentageFormat, 0.6);
            bestHandTextBlock.Text = string.Format(CultureInfo.CurrentCulture, Lang.Profile_BestHandFormat, 24);
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditProfilePage());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Lobby());
        }
        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            Dialogs.ChangePasswordDialogWindow dialog = new Dialogs.ChangePasswordDialogWindow();
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }
}